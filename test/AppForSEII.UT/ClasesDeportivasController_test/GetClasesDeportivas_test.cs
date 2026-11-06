
using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.ClaseDeportivaDTO;
using AppForSEII.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace AppForSEII.UT.ClasesDeportivasController_test
{
    public class GetClasesDeportivas_test : AppForSEII4SqliteUT
    {
        private static readonly DateTime _fechaClase1 = new DateTime(2026, 11, 10, 18, 0, 0);
        private static readonly DateTime _fechaClase2 = new DateTime(2026, 11, 11, 19, 0, 0);
        private static readonly DateTime _fechaClase3 = new DateTime(2026, 11, 10, 20, 0, 0);
        private static readonly DateTime _fechaClase4 = new DateTime(2026, 11, 12, 17, 30, 0);
        private static readonly DateTime _fechaFiltro = new DateTime(2026, 11, 10);

        public GetClasesDeportivas_test()
        {
            var tiposDeporte = new List<TipoDeporte>() {
                new TipoDeporte(1, "Fútbol", "Deporte de equipo", 2),
                new TipoDeporte(2, "Baloncesto", "Deporte de equipo", 2),
                new TipoDeporte(3, "Natación", "Deporte acuático", 2)
            };

            _context.AddRange(tiposDeporte);
            _context.SaveChanges();

            var clases = new List<ClaseDeportiva>() {
                new ClaseDeportiva(1, "Entrenamiento de técnica y posesión", _fechaClase1, "Pista 1", "Carlos Ruiz", "Intermedio", 20, 7.50m, tiposDeporte[0], 1, new List<ClaseInscrita>()),
                new ClaseDeportiva(2, "Entrenamiento táctico", _fechaClase2, null, "Marta López", "Avanzado", 15, 8.50m, tiposDeporte[0], 1, new List<ClaseInscrita>()),
                new ClaseDeportiva(3, "Iniciación al baloncesto", _fechaClase3, "Pista 3", "Carlos Ruiz", "Iniciación", 30, 6.00m, tiposDeporte[1], 2, new List<ClaseInscrita>()),
                // Esta clase no debe aparecer porque no tiene plazas disponibles
                new ClaseDeportiva(4, "Natación para adultos", _fechaClase4, "Piscina 1", "Ana Martín", "Iniciación", 0, 9.00m, tiposDeporte[2], 3, new List<ClaseInscrita>())
            };

            _context.AddRange(clases);
            _context.SaveChanges();
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task GetClasesDeportivas_NULL_FILTERS_test()
        {
            // Arrange
            var expectedClases = new List<ClaseDeportivaDTO>() {
                new ClaseDeportivaDTO(1, "Entrenamiento de técnica y posesión", "Fútbol", "Carlos Ruiz", "Intermedio", _fechaClase1, "Pista 1", 20, 7.50m),
                new ClaseDeportivaDTO(2, "Entrenamiento táctico", "Fútbol", "Marta López", "Avanzado", _fechaClase2, null, 15, 8.50m),
                new ClaseDeportivaDTO(3, "Iniciación al baloncesto", "Baloncesto", "Carlos Ruiz", "Iniciación", _fechaClase3, "Pista 3", 30, 6.00m)
            };

            var mock = new Mock<ILogger<ClasesDeportivasController>>();
            ILogger<ClasesDeportivasController> logger = mock.Object;
            var controller = new ClasesDeportivasController(_context, logger);

            // Act
            var result = await controller.GetClasesDeportivas(null, null, null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var clasesDTOActual = Assert.IsType<List<ClaseDeportivaDTO>>(okResult.Value);

            Assert.Equal(expectedClases.OrderBy(c => c.Id).ToList(), clasesDTOActual.OrderBy(c => c.Id).ToList());
        }

        public static IEnumerable<object?[]> TestCasesFor_GetClasesDeportivas_OK()
        {
            var clasesDTO = new List<ClaseDeportivaDTO>() {
                new ClaseDeportivaDTO(1, "Entrenamiento de técnica y posesión", "Fútbol", "Carlos Ruiz", "Intermedio", _fechaClase1, "Pista 1", 20, 7.50m),
                new ClaseDeportivaDTO(2, "Entrenamiento táctico", "Fútbol", "Marta López", "Avanzado", _fechaClase2, null, 15, 8.50m),
                new ClaseDeportivaDTO(3, "Iniciación al baloncesto", "Baloncesto", "Carlos Ruiz", "Iniciación", _fechaClase3, "Pista 3", 30, 6.00m)
            };

            var clasesTC1 = new List<ClaseDeportivaDTO>() { clasesDTO[0], clasesDTO[1] };
            var clasesTC2 = new List<ClaseDeportivaDTO>() { clasesDTO[0], clasesDTO[2] };
            var clasesTC3 = new List<ClaseDeportivaDTO>() { clasesDTO[0], clasesDTO[2] };
            var clasesTC4 = new List<ClaseDeportivaDTO>() { clasesDTO[0] };
            var clasesTC5 = new List<ClaseDeportivaDTO>();

            var allTests = new List<object?[]>
            {
                // Filtro por tipo de deporte
                new object?[] { "Fútbol", null, null, clasesTC1 },

                // Filtro por monitor
                new object?[] { null, "Carlos Ruiz", null, clasesTC2 },

                // Filtro por fecha
                new object?[] { null, null, _fechaFiltro, clasesTC3 },

                // Combinación de los tres filtros
                new object?[] { "Fútbol", "Carlos Ruiz", _fechaFiltro, clasesTC4 },

                // No hay clases disponibles de este deporte
                new object?[] { "Natación", null, null, clasesTC5 }
            };

            return allTests;
        }

        [Theory]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        [MemberData(nameof(TestCasesFor_GetClasesDeportivas_OK))]
        public async Task GetClasesDeportivas_filter_test(
            string? tipoDeporte,
            string? monitor,
            DateTime? fecha,
            List<ClaseDeportivaDTO> expectedClases)
        {
            // Arrange
            var mock = new Mock<ILogger<ClasesDeportivasController>>();
            ILogger<ClasesDeportivasController> logger = mock.Object;
            var controller = new ClasesDeportivasController(_context, logger);

            // Act
            var result = await controller.GetClasesDeportivas(tipoDeporte, monitor, fecha);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var clasesDTOActual = Assert.IsType<List<ClaseDeportivaDTO>>(okResult.Value);

            Assert.Equal(expectedClases.OrderBy(c => c.Id).ToList(), clasesDTOActual.OrderBy(c => c.Id).ToList());
        }
    }
}
