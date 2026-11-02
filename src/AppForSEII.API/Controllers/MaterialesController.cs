using AppForSEII.API.DTOs.MaterialDTOs;

namespace AppForSEII.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialesController : ControllerBase
    {
        private readonly ApplicationDbContext _context; //Conexion a la base de datos para hacer consultas
        private readonly ILogger<MaterialesController> _logger; 

        public MaterialesController(ApplicationDbContext context, ILogger<MaterialesController> logger)
        {
            _context = context;
            _logger = logger;
        }


        [HttpGet] // GET: api/Materiales/GetMaterialForRental
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<MaterialForRentalDTO>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> GetMaterialForRental(string? tipoMaterial, decimal? precioMaximo) //Las ? cubren el flujo alternativo 0 donde los filtros son opcionales.
        {
            if (precioMaximo != null && precioMaximo < 0) //Precio maximo negativo no existe, en lugar de devolver una lista vacia salta error
            {
                ModelState.AddModelError("precioMaximo", "precioMaximo must be greater than or equal to 0");
                _logger.LogError($"{DateTime.Now} Error: precioMaximo must be greater than or equal to 0");
                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            IList<MaterialForRentalDTO> selectMateriales = await _context.Materiales

                //Join Material with TipoMaterial and TipoDeporte
                .Include(m => m.TipoMaterial)
                .Include(m => m.TipoDeporte)

                .Where(m => m.Cantidad > 0 // only materials in stock
                    && (tipoMaterial == null || m.TipoMaterial.NombreTipoMaterial.Equals(tipoMaterial)) //Aplica el filtro solo si se pasa un TipoMaterial
                    && (precioMaximo == null || m.Precio <= precioMaximo))

                .OrderBy(m => m.NombreMaterial)

                .Select(m => new MaterialForRentalDTO(m.IdMaterial, m.NombreMaterial,
                    m.TipoMaterial.NombreTipoMaterial, m.TipoDeporte.Nombre, m.Precio, m.Cantidad))
                .ToListAsync();

            return Ok(selectMateriales);
        }
    }
}