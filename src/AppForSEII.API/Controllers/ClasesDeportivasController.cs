namespace AppForSEII.API.Controllers
{
    using AppForSEII.API.Models;
    using Microsoft.AspNetCore.Mvc;
    using AppForSEII.API.DTOs.InscripcionClaseDeportivaDTOs;

    [ApiController]
    [Route("api/[controller]")]
    public class ClasesDeportivasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ClasesDeportivasController> _logger;

        public ClasesDeportivasController(ApplicationDbContext context, 
        ILogger<ClasesDeportivasController> logger)
        {
            _context = context;
            _logger = logger;
        }
    

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<ClaseDeportivaDTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetClasesDeportivas(
            string? tipoDeporte = null,
            string? monitor = null,
            DateTime? fecha = null)
        {
            DateTime? dia = fecha?.Date;

            IList<ClaseDeportivaDTO> clases = await _context.ClasesDeportivas
                .Where(c => c.PlazasDisponibles > 0 &&
                    (tipoDeporte == null || c.TipoDeporte.Nombre.Contains(tipoDeporte)) &&
                    (monitor == null || c.Monitor.Contains(monitor)) &&
                    (dia == null || c.FechaHora.Date == dia))
                .Select(c => new ClaseDeportivaDTO(
                    c.Id,
                    c.Descripcion,
                    c.TipoDeporte.Nombre,
                    c.Monitor,
                    c.Nivel,
                    c.FechaHora,
                    c.Lugar,
                    c.PlazasDisponibles,
                    c.PrecioUnitario
                ))
                .ToListAsync();

            return Ok(clases);
        }




    }
}