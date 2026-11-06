using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.MaterialDTOs;

// Ajusta el namespace y el nombre de la clase base a los de tu proyecto de tests
// (el equivalente a AppForMovies.UT y AppForMovies4SqliteUT).
namespace AppForSEII.UT.MaterialesController_test
{
    public class GetMaterialForRental_test : AppForSEII4SqliteUT
    {
        //PREPARACIÓN DE LA BASE DE DATOS PARA LOS TESTS
        public GetMaterialForRental_test()
        {
            var tiposDeporte = new List<TipoDeporte>() {
                new TipoDeporte(0, "Fútbol", null, 2),
                new TipoDeporte(0, "Baloncesto", null, 1),
                new TipoDeporte(0, "Tenis", null, 3),
            };

            var tiposMaterial = new List<TipoMaterial>() {
                new TipoMaterial(0, "Balón", new List<Material>()),
                new TipoMaterial(0, "Raqueta", new List<Material>()),
                new TipoMaterial(0, "Protección", new List<Material>()),
            };

            var materiales = new List<Material>() {
                new Material(0, "Balón de fútbol", 3.50m, 10, tiposMaterial[0], tiposDeporte[0], new List<MaterialAlquilado>()),
                new Material(0, "Balón de baloncesto", 2.50m, 8, tiposMaterial[0], tiposDeporte[1], new List<MaterialAlquilado>()),
                new Material(0, "Raqueta de tenis", 4.00m, 5, tiposMaterial[1], tiposDeporte[2], new List<MaterialAlquilado>()),
                new Material(0, "Espinilleras", 1.50m, 0, tiposMaterial[2], tiposDeporte[0], new List<MaterialAlquilado>()), //Sin stock
            };

            _context.AddRange(tiposDeporte);
            _context.AddRange(tiposMaterial);
            _context.AddRange(materiales);
            _context.SaveChanges();
        }

        public static IEnumerable<object?[]> TestCasesFor_GetMaterialForRental_OK()
        {   
            //Esta lista de materiales es la que se espera que devuelva el método GetMaterialForRental en los distintos casos de prueba.
            var materialDTOs = new List<MaterialForRentalDTO>() {
                new MaterialForRentalDTO(1, "Balón de fútbol", "Balón", "Fútbol", 3.50m, 10),
                new MaterialForRentalDTO(2, "Balón de baloncesto", "Balón", "Baloncesto", 2.50m, 8),
                new MaterialForRentalDTO(3, "Raqueta de tenis", "Raqueta", "Tenis", 4.00m, 5),
            };
            //En este caso, se ordenan los materiales por nombre.
            var materialDTOsTC1 = new List<MaterialForRentalDTO>() { materialDTOs[0], materialDTOs[1], materialDTOs[2] }
                    .OrderBy(m => m.NombreMaterial).ToList();
            //Aqui se filtran los materiales por tipo "Balón", y se ordenan por nombre.
            var materialDTOsTC2 = new List<MaterialForRentalDTO>() { materialDTOs[0], materialDTOs[1] }
                    .OrderBy(m => m.NombreMaterial).ToList();
            //Aqui se filtran los materiales por tipo "Raqueta".
            var materialDTOsTC3 = new List<MaterialForRentalDTO>() { materialDTOs[2] };
            //Aqui se filtran los materiales por precio máximo 3.00.
            var materialDTOsTC4 = new List<MaterialForRentalDTO>() { materialDTOs[1] };

            var allTests = new List<object?[]>
            {
                new object?[] { null, null, materialDTOsTC1, }, //Ningún filtro
                new object?[] { "Balón", null, materialDTOsTC2, }, //Filtrado por tipo "Balón"
                new object?[] { "Raqueta", null, materialDTOsTC3, },
                new object?[] { null, 3.00m, materialDTOsTC4, }, //Filtrado por precio máximo 3.00
                new object?[] { "Balón", 3.00m, materialDTOsTC4, }, //Aplicando ambos filtros
                new object?[] { null, 2.50m, materialDTOsTC4, }, //Filtrado por precio máximo 2.50, debería devolver solo el balón de baloncesto
                new object?[] { "Protección", null, new List<MaterialForRentalDTO>(), }, //Filtrado por tipo "Protección", debería devolver una lista vacía ya que no hay stock
            };

            return allTests;
        }
        //EJECUCIÓN DE LOS TESTS
        [Theory]
        [MemberData(nameof(TestCasesFor_GetMaterialForRental_OK))]
        [Trait("Database", "WithoutFixture")] //Son etiquetas para filtrar y agrupar tests, como en tu referencia. No cambian cómo se ejecutan. WithoutFixture indica que cada test tiene su propia base de datos nueva, en vez de compartirla.
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetMaterialForRental_OK_test(string? tipoMaterial, decimal? precioMaximo,
            IList<MaterialForRentalDTO> expectedMateriales)
        {
            // Arrange
            var mock = new Mock<ILogger<MaterialesController>>();
            ILogger<MaterialesController> logger = mock.Object;
            var controller = new MaterialesController(_context, logger);

            // Act
            var result = await controller.GetMaterialForRental(tipoMaterial, precioMaximo);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var materialDTOsActual = Assert.IsType<List<MaterialForRentalDTO>>(okResult.Value);
            Assert.Equal(expectedMateriales, materialDTOsActual);
        }


        [Fact] //Test para BadRequest con precioMaximo siendo negativo.
        //Se comprueba que la respuesta es un 400, que trae los detalles del error en el formato estándar y que el primer mensaje es exactamente el que escribimos en el controlador.
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task GetMaterialForRental_badrequest_test()
        {
            // Arrange
            var mock = new Mock<ILogger<MaterialesController>>();
            ILogger<MaterialesController> logger = mock.Object;
            var controller = new MaterialesController(_context, logger);

            // Act
            var result = await controller.GetMaterialForRental(null, -1.00m);

            //Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var problemDetails = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);
            var problem = problemDetails.Errors.First().Value[0];

            Assert.Equal("precioMaximo must be greater than or equal to 0", problem);
        }

    }
}