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
    }
}