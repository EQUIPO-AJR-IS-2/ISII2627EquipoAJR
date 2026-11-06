using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.InscripcionCompeticionDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppForSEII.UT.InscripcionesController_test {
    public class CreateInscripcion_test : AppForSEII4SqliteUT {
        private const string _userName = "elena@uclm.es";
        private const string _dni = "12345678A";
        private const string _nombre = "Elena";
        private const string _apellidos = "Navarro Martínez";
        private const string _telefono = "600123456";
        private const string _datosPago = "4111111111111111";

        private const string _competicion1Nombre = "Liga Local de Futbol Sala";
        private const string _competicion1Deporte = "Fútbol";
        private const string _competicion2Nombre = "Open de Tenis de Otono";
        private const string _competicion2Deporte = "Tenis";
        private const string _competicion3Nombre = "Torneo Benefico de Futbol 7";

        public CreateInscripcion_test() {

            var tiposDeporte = new List<TipoDeporte>() {
                new TipoDeporte(0, _competicion1Deporte, null, 2),
                new TipoDeporte(0, _competicion2Deporte, null, 3),
            };

            var competiciones = new List<Competicion>(){
                new Competicion(_competicion1Nombre, tiposDeporte[0], new DateTime(2026, 11, 15), "Pabellon Municipal", 20, 15.00m),
                new Competicion(_competicion2Nombre, tiposDeporte[1], new DateTime(2026, 12, 5), "Pistas de Tenis", 16, 20.50m),
                new Competicion(_competicion3Nombre, tiposDeporte[0], new DateTime(2026, 12, 13), "Campo Anexo", 0, 12.00m),
            };

            ApplicationUser user = new ApplicationUser("1", _nombre, _apellidos, _userName, _dni, 30, "F");
            user.PhoneNumber = _telefono;

            var inscripcion = new Inscripcion(DateTime.Now, MetodoPago.Tarjeta, _datosPago,
                    user, new List<CompeticionInscrita>());
            inscripcion.CompeticionesInscritas.Add(
                new CompeticionInscrita(competiciones[0], inscripcion, "Molestias en la rodilla"));
            inscripcion.PrecioTotal = inscripcion.CompeticionesInscritas.Sum(ci => ci.Competicion.Precio);

            _context.ApplicationUsers.Add(user);
            _context.AddRange(tiposDeporte);
            _context.AddRange(competiciones);
            _context.Add(inscripcion);
            _context.SaveChanges();
        }

        public static IEnumerable<object[]> TestCasesFor_CreateInscripcion() {
            var competicionesInscritas = new List<CompeticionInscritaItemDTO>() {
                new CompeticionInscritaItemDTO(2, _competicion2Nombre, _competicion2Deporte, new DateTime(2026, 12, 5), 20.50m, null) };

            var inscripcionSinCompeticiones = new InscripcionForCreateDTO(_userName,
                MetodoPago.Tarjeta, _datosPago, new List<CompeticionInscritaItemDTO>());

            var inscripcionUsuarioNoRegistrado = new InscripcionForCreateDTO("victor.lopez@uclm.es",
                MetodoPago.Tarjeta, _datosPago, competicionesInscritas);

            var inscripcionDuplicada = new InscripcionForCreateDTO(_userName,
                MetodoPago.Tarjeta, _datosPago, new List<CompeticionInscritaItemDTO>() {
                    new CompeticionInscritaItemDTO(2, _competicion2Nombre, _competicion2Deporte, new DateTime(2026, 12, 5), 20.50m, null),
                    new CompeticionInscritaItemDTO(2, _competicion2Nombre, _competicion2Deporte, new DateTime(2026, 12, 5), 20.50m, null) });

            var inscripcionCompeticionInexistente = new InscripcionForCreateDTO(_userName,
                MetodoPago.Tarjeta, _datosPago, new List<CompeticionInscritaItemDTO>() {
                    new CompeticionInscritaItemDTO(99, "No existe", _competicion1Deporte, new DateTime(2026, 12, 5), 0m, null) });

            var inscripcionSinPlazas = new InscripcionForCreateDTO(_userName,
                MetodoPago.Tarjeta, _datosPago, new List<CompeticionInscritaItemDTO>() {
                    new CompeticionInscritaItemDTO(3, _competicion3Nombre, _competicion1Deporte, new DateTime(2026, 12, 13), 12.00m, null) });

            var allTests = new List<object[]>
            {
                new object[] { inscripcionSinCompeticiones, "Debes seleccionar al menos una competición.", },
                new object[] { inscripcionUsuarioNoRegistrado, "El nombre de usuario no está registrado.", },
                new object[] { inscripcionDuplicada, "No puedes seleccionar la misma competición dos veces.", },
                new object[] { inscripcionCompeticionInexistente, "La competición con id 99 no existe.", },
                new object[] { inscripcionSinPlazas, $"La competición {_competicion3Nombre} no tiene plazas libres.", },
            };

            return allTests;
        }

        [Theory]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        [MemberData(nameof(TestCasesFor_CreateInscripcion))]
        public async Task CreateInscripcion_Error_test(InscripcionForCreateDTO inscripcionDTO, string errorExpected) {
            // Arrange
            var mock = new Mock<ILogger<InscripcionesController>>();
            ILogger<InscripcionesController> logger = mock.Object;

            var controller = new InscripcionesController(_context, logger);

            // Act
            var result = await controller.CreateInscripcion(inscripcionDTO);

            //Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var problemDetails = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);

            var errorActual = problemDetails.Errors.First().Value[0];

            Assert.StartsWith(errorExpected, errorActual);

        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task CreateInscripcion_Success_test() {
            // Arrange
            var mock = new Mock<ILogger<InscripcionesController>>();
            ILogger<InscripcionesController> logger = mock.Object;

            var controller = new InscripcionesController(_context, logger);

            var inscripcionDTO = new InscripcionForCreateDTO(_userName,
                MetodoPago.Tarjeta, _datosPago, new List<CompeticionInscritaItemDTO>()
                { new CompeticionInscritaItemDTO(2, "", "", new DateTime(2026, 12, 5), 0m, "Molestias en la rodilla") });

            // Act
            var result = await controller.CreateInscripcion(inscripcionDTO);

            //Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var actualInscripcionDetailDTO = Assert.IsType<InscripcionDetailDTO>(createdResult.Value);

            Assert.InRange(actualInscripcionDetailDTO.FechaInscripcion,
                DateTime.Now.AddMinutes(-1), DateTime.Now.AddMinutes(1));

            var expectedInscripcionDetailDTO = new InscripcionDetailDTO(2, actualInscripcionDetailDTO.FechaInscripcion,
                _dni, _nombre, _apellidos, _userName, _telefono,
                MetodoPago.Tarjeta, 20.50m, new List<CompeticionInscritaItemDTO>()
                { new CompeticionInscritaItemDTO(2, _competicion2Nombre, _competicion2Deporte, new DateTime(2026, 12, 5), 20.50m, "Molestias en la rodilla") });

            Assert.Equal(expectedInscripcionDetailDTO, actualInscripcionDetailDTO);

        }

    }
}