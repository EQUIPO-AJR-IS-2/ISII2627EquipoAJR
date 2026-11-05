
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

        public InscripcionClaseDeportivaController(
            ApplicationDbContext context,
            ILogger<InscripcionClaseDeportivaController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(
            typeof(InscripcionClaseDeportivaDetailDTO),
            (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetDetailsInscripcionClaseDeportiva(int id)
        {
            var inscripcion = await _context.Set<InscripcionClaseDeportiva>()
                .Where(i => i.Id == id)
                .Select(i => new InscripcionClaseDeportivaDetailDTO(
                    i.Id,
                    i.FechaInscripcion,
                    i.Cliente.Dni ?? "",
                    i.Cliente.Name ?? "",
                    i.Cliente.Surname ?? "",
                    i.Cliente.Email ?? "",
                    i.Cliente.PhoneNumber ?? "",
                    i.MetodoPago,
                    i.PrecioTotal,
                    i.ClasesInscritas
                        .Select(ci => new ClaseInscritaItemDTO(
                            ci.ClaseDeportivaId,
                            ci.ClaseDeportiva.Descripcion,
                            ci.ClaseDeportiva.TipoDeporte.Nombre,
                            ci.ClaseDeportiva.FechaHora,
                            ci.PlazasReservadas,
                            ci.Precio,
                            ci.Observaciones
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
