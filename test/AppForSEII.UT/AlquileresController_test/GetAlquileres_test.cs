using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.AlquilerDTOs;

// Ajusta el namespace y la clase base a los de tu proyecto de tests
namespace AppForSEII.UT.AlquileresController_test
{
    public class GetAlquiler_test : AppForSEII4SqliteUT
    {
        //PREPARACIÓN DE LA BASE DE DATOS PARA LOS TESTS
        //Se crea un alquiler con id 1, que es el que se pide en Get
        public GetAlquiler_test()
        {
            var tiposDeporte = new List<TipoDeporte>() {
                new TipoDeporte(0, "Fútbol", null, 2),
                new TipoDeporte(0, "Tenis", null, 3),
            };

            var tiposMaterial = new List<TipoMaterial>() {
                new TipoMaterial(0, "Balón", new List<Material>()),
                new TipoMaterial(0, "Raqueta", new List<Material>()),
            };

            var materiales = new List<Material>() {
                new Material(0, "Balón de fútbol reglamentario", 3.50m, 10, tiposMaterial[0], tiposDeporte[0], new List<MaterialAlquilado>()),
                new Material(0, "Raqueta de tenis", 4.00m, 5, tiposMaterial[1], tiposDeporte[1], new List<MaterialAlquilado>()),
            };

            ApplicationUser user = new ApplicationUser("1", "Elena", "Navarro Martínez",
                "elena@uclm.es", "12345678B", 30, "Mujer");

            var alquiler = new Alquiler(0, new DateTime(2026, 11, 10), 7.00m, new List<MaterialAlquilado>(), MetodoPago.Tarjeta, user);
            alquiler.MaterialesAlquilados.Add(new MaterialAlquilado(2, "Partido del sábado", materiales[0].Precio, materiales[0], alquiler));

            _context.ApplicationUsers.Add(user);
            _context.AddRange(tiposDeporte);
            _context.AddRange(tiposMaterial);
            _context.AddRange(materiales);
            _context.Add(alquiler);
            _context.SaveChanges();
        }

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetAlquiler_NotFound_test() //Pide un alquiler con id 0, que no existe, y comprueba que devuelve NotFound
        {
            // Arrange
            var mock = new Mock<ILogger<AlquileresController>>();
            ILogger<AlquileresController> logger = mock.Object;
            var controller = new AlquileresController(_context, logger);

            // Act
            var result = await controller.GetAlquiler(0);

            //Assert
            Assert.IsType<NotFoundResult>(result); //Compruebo que devuelve NotFound
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task GetAlquiler_Found_test() //Pide un alquiler con id 1, que sí existe, y comprueba que devuelve Ok con el alquiler correcto
        {
            // Arrange
            var mock = new Mock<ILogger<AlquileresController>>();
            ILogger<AlquileresController> logger = mock.Object;
            var controller = new AlquileresController(_context, logger);

            var expectedAlquiler = new AlquilerDetailDTO(1, new DateTime(2026, 11, 10),
                "12345678B", "Elena", "Navarro Martínez", "",
                MetodoPago.Tarjeta, 7.00m, new List<AlquilerItemDTO>()); //Como no tiene nº teléfono, se deja vacío. El precio total es 7.00, que es el precio del balón (3.50) por la cantidad (2)
            expectedAlquiler.MaterialesAlquilados.Add(
                new AlquilerItemDTO(1, "Balón de fútbol reglamentario", "Balón", "Fútbol", 2, 3.50m, "Partido del sábado"));

            // Act
            var result = await controller.GetAlquiler(1);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result); 
            var alquilerDTOActual = Assert.IsType<AlquilerDetailDTO>(okResult.Value);
            Assert.Equal(expectedAlquiler, alquilerDTOActual); //Haces la comprobación de que el alquiler devuelto es igual al esperado,
            // usando la igualdad por valor definida en AlquilerDetailDTO (aunque no se porque no cubre el test el Equals de AlquilerDetailDTO, pero bueno, funciona)
        }
    }
}