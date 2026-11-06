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
                .Where(a => a.IdAlquiler == id) //Filtramos el alquiler con ese id
                    .Include(a => a.User)  //Trae tmb los datos del usuario que hizo el alquiler
                    .Include(a => a.MaterialesAlquilados) //Trae tmb los datos de los materiales alquilados
                        .ThenInclude(ma => ma.Material) //Así como los datos de cada material alquilado
                            .ThenInclude(m => m.TipoMaterial) //Y su tipo
                .Select(a => new AlquilerDetailDTO(a.IdAlquiler, a.FechaAlquiler, //El Select convierte el Alquiler en un AlquilerDetailDTO, que es lo que devuelve el controller al cliente
            a.User.Dni ?? "", a.User.Name ?? "", a.User.Surname ?? "",
            a.User.PhoneNumber ?? "",
            a.MetodoPago, a.PrecioTotal,
            a.MaterialesAlquilados
                .Select(ma => new AlquilerItemDTO(ma.Material.IdMaterial,   //Este otro hace lo mismo pero con AlquilerItemDTO, que es la lista de materiales alquilados
                    ma.Material.NombreMaterial, ma.Material.TipoMaterial.NombreTipoMaterial,
                    ma.Material.TipoDeporte.Nombre,
                    ma.Cantidad, ma.Precio, ma.Descripcion)).ToList()))
                .FirstOrDefaultAsync();

            if (alquiler == null) //Si no existe un alquiler con ese id, devuelve NotFound o 404
            {
                _logger.LogError($"Error: Alquiler with id {id} does not exist");
                return NotFound();
            }
            //Si existe, devuelve Ok o 200 con el alquiler
            return Ok(alquiler);
        }


        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(AlquilerDetailDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult> CreateAlquiler(AlquilerForCreateDTO alquilerForCreate)
        {
            //Lo primero es validar que el usuario exista en la base de datos
            var user = await _context.ApplicationUsers
                .FirstOrDefaultAsync(au => au.UserName == alquilerForCreate.UserName);
            if (user == null)
                ModelState.AddModelError("AlquilerApplicationUser", "Error! El usuario no está registrado");

            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));
            
            var nombresMateriales = alquilerForCreate.MaterialesAlquilados.Select(i => i.NombreMaterial).ToList(); //Hace una única consulta que trae todos los materiales pedidos.

            var materiales = await _context.Materiales //Trae todos los materiales que coinciden con los nombres de la lista de materiales alquilados
                .Include(m => m.TipoMaterial)
                .Include(m => m.TipoDeporte)
                .Where(m => nombresMateriales.Contains(m.NombreMaterial))
                .ToListAsync();

            // Crea un nuevo alquiler con id 0, precio total 0 y una lista vacía de materiales alquilados. Se indica que el usuario no es null!
            Alquiler alquiler = new Alquiler(0, alquilerForCreate.FechaAlquiler, 0m,
                new List<MaterialAlquilado>(), alquilerForCreate.MetodoPago, user!);

            //Por cada línea de material alquilado, busca el material en la lista de materiales traída de la base de datos. Si no lo encuentra, añade un error al ModelState.
            //Si lo encuentra, añade un MaterialAlquilado a la lista del alquiler y actualiza los datos del DTO con los datos del material.
            foreach (var i in alquilerForCreate.MaterialesAlquilados)
            {
                Material? material = materiales.FirstOrDefault(m => m.NombreMaterial == i.NombreMaterial);

                // NombreMaterial no está en la lista de materiales
                if (material == null)
                {
                    ModelState.AddModelError("MaterialesAlquilados", $"Error! El material '{i.NombreMaterial}' no existe");
                }
                else
                {
                    alquiler.MaterialesAlquilados.Add(
                        new MaterialAlquilado(i.Cantidad, i.Descripcion, material.Precio, material, alquiler));

                    i.IdMaterial = material.IdMaterial;
                    i.TipoMaterial = material.TipoMaterial.NombreTipoMaterial;
                    i.TipoDeporte = material.TipoDeporte.Nombre;
                    i.Precio = material.Precio;
                }
            }

            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));

            alquiler.PrecioTotal = alquiler.MaterialesAlquilados.Sum(ma => ma.Precio * ma.Cantidad); //Esto es lo que hace el controller, no el cliente. El cliente envía precio: 0 en cada línea, y el controller calcula el precio total al guardar.
            //En el AlquilerForCreateDTO no tiene sentido por lo dicho antes el cliente envia precio 0 en cada línea, tu lo calculas aquí y lo guardas en la base de datos. 
            _context.Add(alquiler); //Añade el alquiler a la base de datos, pero aún no lo guarda. Se guarda en el try-catch de abajo.

            try
            {
                // Se guarda en la base de datos, y se asigna un IdAlquiler a alquiler
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                ModelState.AddModelError("Alquiler", "Error! Hubo un error al guardar su alquiler, inténtelo de nuevo más tarde");
                return Conflict("Error" + ex.Message);
            }

            // Devuelve un AlquilerDetailDTO con los datos del alquiler recién creado, incluyendo el IdAlquiler asignado por la base de datos
            var alquilerDetail = new AlquilerDetailDTO(alquiler.IdAlquiler, alquiler.FechaAlquiler,
                user!.Dni ?? "", user.Name ?? "", user.Surname ?? "", user.PhoneNumber ?? "",
                alquiler.MetodoPago, alquiler.PrecioTotal,
                alquilerForCreate.MaterialesAlquilados);

            return CreatedAtAction("GetAlquiler", new { id = alquiler.IdAlquiler }, alquilerDetail);
        }
    }
}