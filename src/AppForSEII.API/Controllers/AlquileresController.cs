using AppForSEII.API.DTOs.AlquilerDTOs;

namespace AppForSEII.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlquileresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AlquileresController> _logger;

        public AlquileresController(ApplicationDbContext context, ILogger<AlquileresController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(AlquilerDetailDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetAlquiler(int id)
        {
            var alquiler = await _context.Alquileres
                .Where(a => a.IdAlquiler == id)
                    .Include(a => a.User) //join table ApplicationUser
                    .Include(a => a.MaterialesAlquilados) //join table MaterialAlquilado
                        .ThenInclude(ma => ma.Material) //then join table Material
                            .ThenInclude(m => m.TipoMaterial) //then join table TipoMaterial
                .Select(a => new AlquilerDetailDTO(a.IdAlquiler, a.FechaAlquiler,
            a.User.Dni ?? "", a.User.Name ?? "", a.User.Surname ?? "",
            a.User.PhoneNumber ?? "",
            a.MetodoPago, a.PrecioTotal,
            a.MaterialesAlquilados
                .Select(ma => new AlquilerItemDTO(ma.Material.IdMaterial,
                    ma.Material.NombreMaterial, ma.Material.TipoMaterial.NombreTipoMaterial,
                    ma.Material.TipoDeporte.Nombre,
                    ma.Cantidad, ma.Precio, ma.Descripcion)).ToList()))
                .FirstOrDefaultAsync();

            if (alquiler == null)
            {
                _logger.LogError($"Error: Alquiler with id {id} does not exist");
                return NotFound();
            }

            return Ok(alquiler);
        }

        
    }
}