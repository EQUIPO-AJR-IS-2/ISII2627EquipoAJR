using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.AlquilerDTOs;

namespace AppForSEII.UT.AlquileresController_test
{
    public class CreateAlquiler_test : AppForSEII4SqliteUT
    {
        private const string _userName = "elena@uclm.es";

        private const string _material1Nombre = "Balón de fútbol reglamentario";
        private const string _material1Tipo = "Balón";
        private const string _material1Deporte = "Fútbol";
        private const string _material2Nombre = "Raqueta de tenis";
        private const string _material2Tipo = "Raqueta";
        private const string _material2Deporte = "Tenis";

        //PREPARACIÓN DE LA BASE DE DATOS PARA LOS TESTS
        //Se crea un usuario y dos materiales, que son los que se usan en los tests
        public CreateAlquiler_test()
        {
            var tiposDeporte = new List<TipoDeporte>() {
                new TipoDeporte(0, _material1Deporte, null, 2),
                new TipoDeporte(0, _material2Deporte, null, 3),
            };

            var tiposMaterial = new List<TipoMaterial>() {
                new TipoMaterial(0, _material1Tipo, new List<Material>()),
                new TipoMaterial(0, _material2Tipo, new List<Material>()),
            };

            var materiales = new List<Material>() {
                new Material(0, _material1Nombre, 3.50m, 10, tiposMaterial[0], tiposDeporte[0], new List<MaterialAlquilado>()),
                new Material(0, _material2Nombre, 4.00m, 5, tiposMaterial[1], tiposDeporte[1], new List<MaterialAlquilado>()),
            };

            ApplicationUser user = new ApplicationUser("1", "Elena", "Navarro Martínez",
                _userName, "12345678B", 30, "Mujer");

            var alquiler = new Alquiler(0, new DateTime(2026, 11, 10), 7.00m,
                new List<MaterialAlquilado>(), MetodoPago.Tarjeta, user);
            alquiler.MaterialesAlquilados.Add(
                new MaterialAlquilado(2, "Partido del sábado", materiales[0].Precio, materiales[0], alquiler));

            _context.ApplicationUsers.Add(user);
            _context.AddRange(tiposDeporte);
            _context.AddRange(tiposMaterial);
            _context.AddRange(materiales);
            _context.Add(alquiler);
            _context.SaveChanges();
        }

        // Se usa MemberData para pasar varios casos de prueba al mismo test, y comprobar que devuelve el error esperado en cada caso
        public static IEnumerable<object[]> TestCasesPara_CreateAlquiler()
        {
            var alquilerItems = new List<AlquilerItemDTO>() {
                new AlquilerItemDTO(2, _material2Nombre, _material2Tipo, _material2Deporte, 1, 4.00m, "Clase de tenis") }; //El material 2 existe, el material 3 no existe

            var alquilerApplicationUser = new AlquilerForCreateDTO("nadie@uclm.es",
                new DateTime(2026, 11, 12), MetodoPago.Tarjeta, alquilerItems); //El usuario no existe, el material 2 sí existe

            var alquilerMaterialNoExiste = new AlquilerForCreateDTO(_userName,
                new DateTime(2026, 11, 12), MetodoPago.Tarjeta,
                new List<AlquilerItemDTO>() { new AlquilerItemDTO(3, "Material inexistente", "Balón", "Fútbol", 1, 1.00m) }); //El usuario existe, el material 3 no existe

            var allTests = new List<object[]>
            {
                new object[] { alquilerApplicationUser, "Error! El usuario no está registrado", }, 
                new object[] { alquilerMaterialNoExiste, "Error! El material 'Material inexistente' no existe", },
            };

            return allTests;
        }

        [Theory]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        [MemberData(nameof(TestCasesPara_CreateAlquiler))]
        public async Task CreateAlquiler_Error_test(AlquilerForCreateDTO alquilerDTO, string errorExpected)
        {
            // Arrange
            var mock = new Mock<ILogger<AlquileresController>>();
            ILogger<AlquileresController> logger = mock.Object;

            var controller = new AlquileresController(_context, logger);

            // Act
            var result = await controller.CreateAlquiler(alquilerDTO);

            //Assert
            //Comprueba que devuelve BadRequest con el error esperado
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var problemDetails = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);

            var errorActual = problemDetails.Errors.First().Value[0];

            Assert.StartsWith(errorExpected, errorActual);
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task CreateAlquiler_Success_test()
        {
            // Arrange
            //Se crea un mock del logger para pasarlo al controller, ya que el controller lo necesita en el constructor
            var mock = new Mock<ILogger<AlquileresController>>();
            ILogger<AlquileresController> logger = mock.Object;

            var controller = new AlquileresController(_context, logger);

            DateTime fecha = new DateTime(2026, 11, 12);

            //El alquiler que se va a crear, con un material que sí existe y un usuario que sí existe
            var alquilerDTO = new AlquilerForCreateDTO(_userName, fecha, MetodoPago.Tarjeta,
                new List<AlquilerItemDTO>()
                { new AlquilerItemDTO(2, _material2Nombre, _material2Tipo, _material2Deporte, 1, 4.00m, "Clase de tenis") });
            //El alquiler que se espera que devuelva el controller, con el IdAlquiler asignado por la base de datos
            var expectedAlquilerDetailDTO = new AlquilerDetailDTO(2, fecha,
                "12345678B", "Elena", "Navarro Martínez", "",
                MetodoPago.Tarjeta, 4.00m, new List<AlquilerItemDTO>()
                { new AlquilerItemDTO(2, _material2Nombre, _material2Tipo, _material2Deporte, 1, 4.00m, "Clase de tenis") });

            // Act
            //Se llama al controller para crear el alquiler
            var result = await controller.CreateAlquiler(alquilerDTO);
            

            //Assert
            //Comprueba que devuelve CreatedAtActionResult con el alquiler creado, y que el alquiler creado es igual al esperado
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var actualAlquilerDetailDTO = Assert.IsType<AlquilerDetailDTO>(createdResult.Value);

            Assert.Equal(expectedAlquilerDetailDTO, actualAlquilerDetailDTO);
        }
    }
}