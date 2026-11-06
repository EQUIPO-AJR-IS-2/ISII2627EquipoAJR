using System.Net;
using AppForSEII.API.Models;
using AppForSEII.API.DTOs.InscripcionCompeticionDTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InscripcionesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<InscripcionesController> _logger;

        public InscripcionesController(
            ApplicationDbContext context,
            ILogger<InscripcionesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(
            typeof(InscripcionDetailDTO),
            (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetDetailsInscripcion(int id)
        {
            var inscripcion = await _context.Inscripciones
                .Where(i => i.Id == id)
                .Select(i => new InscripcionDetailDTO(
                    i.Id,
                    i.FechaInscripcion,
                    i.ApplicationUser.Dni ?? "",
                    i.ApplicationUser.Name ?? "",
                    i.ApplicationUser.Surname ?? "",
                    i.ApplicationUser.Email ?? "",
                    i.ApplicationUser.PhoneNumber ?? "",
                    i.MetodoPago,
                    i.PrecioTotal,
                    i.CompeticionesInscritas
                        .Select(ci => new CompeticionInscritaItemDTO(
                            ci.CompeticionId,
                            ci.Competicion.Nombre,
                            ci.Competicion.TipoDeporte.Nombre,
                            ci.Competicion.Fecha,
                            ci.Competicion.Precio,
                            ci.ProblemasFisicos
                        ))
                        .ToList()
                ))
                .FirstOrDefaultAsync();

            if (inscripcion == null)
            {
                _logger.LogError(
                    $"La inscripción con id {id} no existe");

                return NotFound();
            }

            return Ok(inscripcion);
        }

        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(InscripcionDetailDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult> CreateInscripcion(InscripcionForCreateDTO inscripcionForCreate)
        {
           
            var cliente = await _context.ApplicationUsers
                .FirstOrDefaultAsync(u => u.UserName == inscripcionForCreate.NombreUsuario);

            if (cliente == null)
                ModelState.AddModelError("NombreUsuario", "El nombre de usuario no está registrado.");

          
            if (inscripcionForCreate.CompeticionesInscritas.Count == 0)
                ModelState.AddModelError("CompeticionesInscritas", "Debes seleccionar al menos una competición.");

            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));

            var idsCompeticiones = inscripcionForCreate.CompeticionesInscritas
                .Select(ci => ci.CompeticionId).ToList();

            if (idsCompeticiones.Distinct().Count() != idsCompeticiones.Count)
                ModelState.AddModelError("CompeticionesInscritas", "No puedes seleccionar la misma competición dos veces.");

            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));

           
            var competiciones = await _context.Competiciones
                .Include(c => c.TipoDeporte)
                .Include(c => c.CompeticionesInscritas)
                .Where(c => idsCompeticiones.Contains(c.Id))
                .ToListAsync();

            foreach (var item in inscripcionForCreate.CompeticionesInscritas)
            {
                var competicion = competiciones.FirstOrDefault(c => c.Id == item.CompeticionId);

                if (competicion == null)
                    ModelState.AddModelError("CompeticionesInscritas", $"La competición con id {item.CompeticionId} no existe.");
                else if (competicion.CompeticionesInscritas.Count >= competicion.Plazas)
                    ModelState.AddModelError("CompeticionesInscritas", $"La competición {competicion.Nombre} no tiene plazas libres.");
            }

            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));

            var inscripcion = new Inscripcion(
                DateTime.Now, inscripcionForCreate.MetodoPago, inscripcionForCreate.DatosPago,
                cliente!, new List<CompeticionInscrita>());

            foreach (var item in inscripcionForCreate.CompeticionesInscritas)
            {
                var competicion = competiciones.First(c => c.Id == item.CompeticionId);

                var competicionInscrita = new CompeticionInscrita(
                    competicion, inscripcion, item.ProblemasFisicos);

                inscripcion.CompeticionesInscritas.Add(competicionInscrita);
            }

           
            inscripcion.PrecioTotal = inscripcion.CompeticionesInscritas
                .Sum(ci => ci.Competicion.Precio);

            _context.Inscripciones.Add(inscripcion);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar la inscripción a competiciones.");
                return Conflict("No se ha podido registrar la inscripción.");
            }

            var competicionesInscritasDTOs = inscripcion.CompeticionesInscritas
                .Select(ci => new CompeticionInscritaItemDTO(
                    ci.CompeticionId, ci.Competicion.Nombre,
                    ci.Competicion.TipoDeporte.Nombre, ci.Competicion.Fecha,
                    ci.Competicion.Precio, ci.ProblemasFisicos))
                .ToList();

            var inscripcionDetail = new InscripcionDetailDTO(
                inscripcion.Id, inscripcion.FechaInscripcion,
                cliente!.Dni ?? "", cliente.Name ?? "", cliente.Surname ?? "",
                cliente.Email ?? "", cliente.PhoneNumber ?? "",
                inscripcion.MetodoPago, inscripcion.PrecioTotal, competicionesInscritasDTOs);

            return CreatedAtAction(nameof(GetDetailsInscripcion), new { id = inscripcion.Id }, inscripcionDetail);
        }
    
    }
}