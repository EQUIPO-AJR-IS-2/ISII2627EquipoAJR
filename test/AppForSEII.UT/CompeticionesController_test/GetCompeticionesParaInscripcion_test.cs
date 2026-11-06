using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.CompeticionDTOs;

namespace AppForSEII.UT.CompeticionesController_test
{
    public class GetCompeticionesParaInscripcion_test : AppForSEII4SqliteUT
    {
        //Preparo la base de datos 
        public GetCompeticionesParaInscripcion_test()
        {
            var tiposDeporte = new List<TipoDeporte>() {
                new TipoDeporte(0, "Fútbol", null, 2),
                new TipoDeporte(0, "Baloncesto", null, 1),
                new TipoDeporte(0, "Tenis", null, 3),
            };

            var competiciones = new List<Competicion>() {
                new Competicion("Liga Local de Futbol Sala", tiposDeporte[0], new DateTime(2026, 11, 15), "Pabellon Municipal", 20, 15.00m),
                new Competicion("Torneo 3x3 de Baloncesto", tiposDeporte[1], new DateTime(2026, 11, 22), "Pista Exterior Norte", 12, 10.10m),
                new Competicion("Open de Tenis de Otono", tiposDeporte[2], new DateTime(2026, 12, 5), "Pistas de Tenis", 16, 20.50m),

                //esta competicion no tiene plazas asi que nunca debe devolverse
                new Competicion("Torneo Benefico de Futbol 7", tiposDeporte[0], new DateTime(2026, 12, 13), "Campo Anexo", 0, 12.00m),

                //esta tiene una sola plaza y se ocupara con la inscripcion de abajo tampoco debe devolverse
                new Competicion("Torneo Relampago de Baloncesto", tiposDeporte[1], new DateTime(2026, 11, 29), "Pabellon Municipal", 1, 5.00m),
            };

            ApplicationUser user = new ApplicationUser("1", "Elena", "Navarro Martínez", "elena@uclm.es", "12345678A", 30, "F");

            var inscripcion = new Inscripcion(DateTime.Now, MetodoPago.Tarjeta, "4111111111111111",
                user, new List<CompeticionInscrita>());

            //ocupamos la unica plaza del Torneo Relampago
            inscripcion.CompeticionesInscritas.Add(
                new CompeticionInscrita(competiciones[4], inscripcion, "Molestias en la rodilla"));

            _context.Add(user);
            _context.AddRange(tiposDeporte);
            _context.AddRange(competiciones);
            _context.Add(inscripcion);
            _context.SaveChanges();
        }

        public static IEnumerable<object?[]> TestCasesFor_GetCompeticionesParaInscripcion_OK()
        {
            //competiciones que si tienen plazas libres
            var competicionDTOs = new List<CompeticionParaInscripcionDTO>() {
                new CompeticionParaInscripcionDTO(1, "Liga Local de Futbol Sala", "Fútbol", new DateTime(2026, 11, 15), "Pabellon Municipal", 15.00m),
                new CompeticionParaInscripcionDTO(2, "Torneo 3x3 de Baloncesto", "Baloncesto", new DateTime(2026, 11, 22), "Pista Exterior Norte", 10.10m),
                new CompeticionParaInscripcionDTO(3, "Open de Tenis de Otono", "Tenis", new DateTime(2026, 12, 5), "Pistas de Tenis", 20.50m),
            };

            //flujo basico: sin filtros se devuelven las tres ordenadas por fecha
            var competicionDTOsTC1 = new List<CompeticionParaInscripcionDTO>()
                    { competicionDTOs[0], competicionDTOs[1], competicionDTOs[2] }
                    .OrderBy(c => c.Fecha).ToList();

            //flujo alternativo 1: filtramos por lugar
            var competicionDTOsTC2 = new List<CompeticionParaInscripcionDTO>() { competicionDTOs[0] };

            //flujo alternativo 1: filtramos por tipo de deporte
            var competicionDTOsTC3 = new List<CompeticionParaInscripcionDTO>() { competicionDTOs[2] };

            //flujo alternativo 1: aplicamos los dos filtros a la vez
            var competicionDTOsTC4 = new List<CompeticionParaInscripcionDTO>() { competicionDTOs[1] };

            var allTests = new List<object?[]>
            {                
                new object?[] { null, null, competicionDTOsTC1, },
                new object?[] { "Pabellon", null, competicionDTOsTC2, },
                new object?[] { null, "Tenis", competicionDTOsTC3, },
                new object?[] { "Pista", "Baloncesto", competicionDTOsTC4, },
                //flujo alternativo 0: no hay ninguna competicion de ese deporte
                new object?[] { null, "Natación", new List<CompeticionParaInscripcionDTO>(), },
            };

            return allTests;
        }

        //Ejecutamos los test 
        [Theory]
        [MemberData(nameof(TestCasesFor_GetCompeticionesParaInscripcion_OK))]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetCompeticionesParaInscripcion_OK_test(string? lugar, string? tipoDeporte,
            IList<CompeticionParaInscripcionDTO> expectedCompeticiones)
        {
            // Arrange
            var mock = new Mock<ILogger<CompeticionesController>>();
            ILogger<CompeticionesController> logger = mock.Object;
            var controller = new CompeticionesController(_context, logger);

            // Act
            var result = await controller.GetCompeticionesParaInscripcion(lugar, tipoDeporte);

            //Assert
            //comprobamos que la respuesta es OK
            var okResult = Assert.IsType<OkObjectResult>(result);
            //obtenemos la lista de competiciones
            var competicionDTOsActual = Assert.IsType<List<CompeticionParaInscripcionDTO>>(okResult.Value);
            Assert.Equal(expectedCompeticiones, competicionDTOsActual);
        }
    }
}