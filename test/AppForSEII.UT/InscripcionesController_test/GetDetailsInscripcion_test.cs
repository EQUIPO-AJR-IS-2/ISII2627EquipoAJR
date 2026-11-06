using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.InscripcionCompeticionDTO;

namespace AppForSEII.UT.InscripcionesController_test
{
    public class GetDetailsInscripcion_test : AppForSEII4SqliteUT
    {
        
        public GetDetailsInscripcion_test()
        {
            var tiposDeporte = new List<TipoDeporte>() {
                new TipoDeporte(0, "Fútbol", null, 2),
                new TipoDeporte(0, "Tenis", null, 3),
            };

            var competiciones = new List<Competicion>() {
                new Competicion("Liga Local de Futbol Sala", tiposDeporte[0], new DateTime(2026, 11, 15), "Pabellon Municipal", 20, 15.00m),
                new Competicion("Open de Tenis de Otono", tiposDeporte[1], new DateTime(2026, 12, 5), "Pistas de Tenis", 16, 20.50m),
            };

            ApplicationUser user = new ApplicationUser("1", "Elena", "Navarro Martínez", "elena@uclm.es", "12345678A", 30, "F");
            
            user.PhoneNumber = "600123456";

            var inscripcion = new Inscripcion(new DateTime(2026, 11, 6), MetodoPago.Tarjeta,
                "4111111111111111", user, new List<CompeticionInscrita>());

            
            inscripcion.CompeticionesInscritas.Add(
                new CompeticionInscrita(competiciones[0], inscripcion, "Molestias en la rodilla"));
            inscripcion.CompeticionesInscritas.Add(
                new CompeticionInscrita(competiciones[1], inscripcion, null));

            
            inscripcion.PrecioTotal = inscripcion.CompeticionesInscritas.Sum(ci => ci.Competicion.Precio);

            _context.Add(user);
            _context.AddRange(tiposDeporte);
            _context.AddRange(competiciones);
            _context.Add(inscripcion);
            _context.SaveChanges();
        }

        
        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetDetailsInscripcion_NotFound_test()
        {
            // Arrange
            var mock = new Mock<ILogger<InscripcionesController>>();
            ILogger<InscripcionesController> logger = mock.Object;

            var controller = new InscripcionesController(_context, logger);

            // Act
            var result = await controller.GetDetailsInscripcion(0);

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }

       
        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task GetDetailsInscripcion_Found_test()
        {
            // Arrange
            var mock = new Mock<ILogger<InscripcionesController>>();
            ILogger<InscripcionesController> logger = mock.Object;
            var controller = new InscripcionesController(_context, logger);

            // este es el resultado esperado 
            var expectedInscripcion = new InscripcionDetailDTO(1, new DateTime(2026, 11, 6),
                "12345678A", "Elena", "Navarro Martínez", "elena@uclm.es", "600123456",
                MetodoPago.Tarjeta, 35.50m,
                new List<CompeticionInscritaItemDTO>());

            expectedInscripcion.CompeticionesInscritas.Add(
                new CompeticionInscritaItemDTO(1, "Liga Local de Futbol Sala", "Fútbol",
                    new DateTime(2026, 11, 15), 15.00m, "Molestias en la rodilla"));
            expectedInscripcion.CompeticionesInscritas.Add(
                new CompeticionInscritaItemDTO(2, "Open de Tenis de Otono", "Tenis",
                    new DateTime(2026, 12, 5), 20.50m, null));

            // Act 
            var result = await controller.GetDetailsInscripcion(1);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var inscripcionDTOActual = Assert.IsType<InscripcionDetailDTO>(okResult.Value);
            //comparo los objetos enteros con el Equals del DTO
            Assert.Equal(expectedInscripcion, inscripcionDTOActual);
        }
    }
}