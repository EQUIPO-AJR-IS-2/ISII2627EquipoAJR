using AppForSEII.API.DTOs.CompeticionDTOs;

namespace AppForSEII.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompeticionesController : ControllerBase
    {
       
        private readonly ApplicationDbContext _context;
        
        private readonly ILogger<CompeticionesController> _logger;

        public CompeticionesController(ApplicationDbContext context, ILogger<CompeticionesController> logger)
        {
            _context = context;
            _logger = logger;
        }

       
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<CompeticionParaInscripcionDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetCompeticionesParaInscripcion(string? lugar, string? tipoDeporte)
        {
            IList<CompeticionParaInscripcionDTO> competiciones = await _context.Competiciones

            
                .Include(c => c.TipoDeporte)

                .Where(c =>
                    
                    c.CompeticionesInscritas.Count() < c.Plazas

                    
                    && (lugar == null || c.Lugar.Contains(lugar))

                   
                    && (tipoDeporte == null || c.TipoDeporte.Nombre.Equals(tipoDeporte))
                )

                .OrderBy(c => c.Fecha)

                .Select(c => new CompeticionParaInscripcionDTO(
                    c.Id, c.Nombre, c.TipoDeporte.Nombre, c.Fecha, c.Lugar, c.Precio))

                .ToListAsync();

            return Ok(competiciones);
        }
    }
}