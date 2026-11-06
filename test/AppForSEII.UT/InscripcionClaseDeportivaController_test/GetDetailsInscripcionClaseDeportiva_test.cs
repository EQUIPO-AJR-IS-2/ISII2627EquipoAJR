
using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.InscripcionClaseDeportivaDTOs;
using AppForSEII.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace AppForSEII.UT.InscripcionClaseDeportivaController_test
{
    public class GetDetailsInscripcionClaseDeportiva_test : AppForSEII4SqliteUT
    {
        private readonly DateTime _fechaInscripcion = new DateTime(2026, 11, 5, 14, 45, 0);
        private readonly DateTime _fechaClase = new DateTime(2026, 11, 10, 18, 0, 0);

        public GetDetailsInscripcionClaseDeportiva_test()
        {
            var tipoDeporte = new TipoDeporte(1, "Fútbol", null, 2);

            var clase = new ClaseDeportiva(
                1, "Entrenamiento de técnica y posesión", _fechaClase,
                "Pista 1", "Carlos Ruiz", "Intermedio", 20, 7.50m,
                tipoDeporte, 1, new List<ClaseInscrita>());

            var cliente = new ApplicationUser(
                "1", "Peter", "Jackson", "peter@uclm.es",
                "12345678B", 20, "H");

            cliente.PhoneNumber = "600123456";

            var inscripcion = new InscripcionClaseDeportiva(
                1, new List<ClaseInscrita>(), cliente,
                "Pago de prueba", _fechaInscripcion,
                MetodoPago.Bizum, 7.50m);

            inscripcion.ClasesInscritas.Add(new ClaseInscrita(
                clase, clase.Id, inscripcion, inscripcion.Id,
                "Inscripción de prueba", 1, 7.50m));

            _context.Add(inscripcion);
            _context.SaveChanges();
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task GetDetailsInscripcionClaseDeportiva_NotFound_test()
        {
            // Arrange
            var mock = new Mock<ILogger<InscripcionClaseDeportivaController>>();
            ILogger<InscripcionClaseDeportivaController> logger = mock.Object;
            var controller = new InscripcionClaseDeportivaController(_context, logger);

            // Act
            var result = await controller.GetDetailsInscripcionClaseDeportiva(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task GetDetailsInscripcionClaseDeportiva_Found_test()
        {
            // Arrange
            var mock = new Mock<ILogger<InscripcionClaseDeportivaController>>();
            ILogger<InscripcionClaseDeportivaController> logger = mock.Object;
            var controller = new InscripcionClaseDeportivaController(_context, logger);

            var expectedInscripcion = new InscripcionClaseDeportivaDetailDTO(
                1, _fechaInscripcion, "12345678B",
                "Peter", "Jackson", "peter@uclm.es",
                "600123456", MetodoPago.Bizum, 7.50m,
                new List<ClaseInscritaItemDTO>()
                {
                    new ClaseInscritaItemDTO(
                        1, "Entrenamiento de técnica y posesión",
                        "Fútbol", _fechaClase, 1, 7.50m,
                        "Inscripción de prueba")
                });

            // Act
            var result = await controller.GetDetailsInscripcionClaseDeportiva(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualInscripcion = Assert.IsType<InscripcionClaseDeportivaDetailDTO>(okResult.Value);

            Assert.True(expectedInscripcion.Equals(actualInscripcion));
        }
    }
}
