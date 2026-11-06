
using System.Net;
using AppForSEII.API.Models;
using AppForSEII.API.DTOs.InscripcionClaseDeportivaDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InscripcionClaseDeportivaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<InscripcionClaseDeportivaController> _logger;

        public InscripcionClaseDeportivaController(ApplicationDbContext context, ILogger<InscripcionClaseDeportivaController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(InscripcionClaseDeportivaDetailDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetDetailsInscripcionClaseDeportiva(int id)
        {
            var inscripcion = await _context.Set<InscripcionClaseDeportiva>()
                .Where(i => i.Id == id)
                .Select(i => new InscripcionClaseDeportivaDetailDTO(
                    i.Id, i.FechaInscripcion,
                    i.Cliente.Dni ?? "", i.Cliente.Name ?? "", i.Cliente.Surname ?? "",
                    i.Cliente.Email ?? "", i.Cliente.PhoneNumber ?? "",
                    i.MetodoPago, i.PrecioTotal,
                    i.ClasesInscritas
                        .Select(ci => new ClaseInscritaItemDTO(
                            ci.ClaseDeportivaId, ci.ClaseDeportiva.Descripcion,
                            ci.ClaseDeportiva.TipoDeporte.Nombre, ci.ClaseDeportiva.FechaHora,
                            ci.PlazasReservadas, ci.Precio, ci.Observaciones))
                        .ToList()))
                .FirstOrDefaultAsync();

            if (inscripcion == null)
            {
                _logger.LogError($"La inscripción con id {id} no existe");
                return NotFound();
            }

            return Ok(inscripcion);
        }

        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(InscripcionClaseDeportivaDetailDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult> CreateInscripcionClaseDeportiva(InscripcionClaseDeportivaForCreateDTO inscripcionForCreate)
        {
            var cliente = await _context.ApplicationUsers
                .FirstOrDefaultAsync(u => u.UserName == inscripcionForCreate.NombreUsuario);

            if (cliente == null)
                ModelState.AddModelError("NombreUsuario", "El nombre de usuario no está registrado.");

            if (inscripcionForCreate.ClasesInscritas == null || inscripcionForCreate.ClasesInscritas.Count == 0)
                ModelState.AddModelError("ClasesInscritas", "Debes seleccionar al menos una clase deportiva.");

            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));

            var idsClases = inscripcionForCreate.ClasesInscritas.Select(ci => ci.ClaseDeportivaId).ToList();

            if (idsClases.Distinct().Count() != idsClases.Count)
                ModelState.AddModelError("ClasesInscritas", "No puedes seleccionar la misma clase dos veces.");

            foreach (var item in inscripcionForCreate.ClasesInscritas)
            {
                if (item.PlazasReservadas < 1 || item.PlazasReservadas > 3)
                    ModelState.AddModelError("ClasesInscritas", "Debes reservar entre 1 y 3 plazas por clase.");
            }

            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));

            var clases = await _context.ClasesDeportivas
                .Include(c => c.TipoDeporte)
                .Where(c => idsClases.Contains(c.Id))
                .ToListAsync();

            foreach (var item in inscripcionForCreate.ClasesInscritas)
            {
                var clase = clases.FirstOrDefault(c => c.Id == item.ClaseDeportivaId);

                if (clase == null)
                    ModelState.AddModelError("ClasesInscritas", $"La clase con id {item.ClaseDeportivaId} no existe.");
                else if (clase.PlazasDisponibles < item.PlazasReservadas)
                    ModelState.AddModelError("ClasesInscritas", $"No hay suficientes plazas en la clase {clase.Id}.");
            }

            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));

            decimal precioTotal = inscripcionForCreate.ClasesInscritas.Sum(item =>
                clases.First(c => c.Id == item.ClaseDeportivaId).PrecioUnitario * item.PlazasReservadas);

            var inscripcion = new InscripcionClaseDeportiva(
                0, new List<ClaseInscrita>(), cliente!, inscripcionForCreate.DatosPago,
                DateTime.Now, inscripcionForCreate.MetodoPago, precioTotal);

            foreach (var item in inscripcionForCreate.ClasesInscritas)
            {
                var clase = clases.First(c => c.Id == item.ClaseDeportivaId);
                decimal precio = clase.PrecioUnitario * item.PlazasReservadas;

                var claseInscrita = new ClaseInscrita(
                    clase, clase.Id, inscripcion, 0, item.Observaciones, item.PlazasReservadas, precio);

                inscripcion.ClasesInscritas.Add(claseInscrita);
                clase.PlazasDisponibles -= item.PlazasReservadas;
            }

            _context.InscripcionesClasesDeportivas.Add(inscripcion);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar la inscripción a clases deportivas.");
                return Conflict("No se ha podido registrar la inscripción.");
            }

            var clasesInscritasDTOs = inscripcion.ClasesInscritas
                .Select(ci => new ClaseInscritaItemDTO(
                    ci.ClaseDeportivaId, ci.ClaseDeportiva.Descripcion,
                    ci.ClaseDeportiva.TipoDeporte.Nombre, ci.ClaseDeportiva.FechaHora,
                    ci.PlazasReservadas, ci.Precio, ci.Observaciones))
                .ToList();

            var inscripcionDetail = new InscripcionClaseDeportivaDetailDTO(
                inscripcion.Id, inscripcion.FechaInscripcion,
                cliente!.Dni ?? "", cliente.Name ?? "", cliente.Surname ?? "",
                cliente.Email ?? "", cliente.PhoneNumber ?? "",
                inscripcion.MetodoPago, inscripcion.PrecioTotal, clasesInscritasDTOs);

            return CreatedAtAction(nameof(GetDetailsInscripcionClaseDeportiva), new { id = inscripcion.Id }, inscripcionDetail);
        }
    }
}
