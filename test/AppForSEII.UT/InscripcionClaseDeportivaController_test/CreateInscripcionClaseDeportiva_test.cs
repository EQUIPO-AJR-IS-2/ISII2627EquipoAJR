using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.InscripcionClaseDeportivaDTOs;
using AppForSEII.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace AppForSEII.UT.InscripcionClaseDeportivaController_test
{
    public class CreateInscripcionClaseDeportiva_test : AppForSEII4SqliteUT
    {
        private const string _userName = "peter@uclm.es";

        private static readonly DateTime _fechaClase1 = new DateTime(2026, 11, 10, 18, 0, 0);
        private static readonly DateTime _fechaClase2 = new DateTime(2026, 11, 11, 19, 0, 0);
        private static readonly DateTime _fechaClase3 = new DateTime(2026, 11, 12, 17, 30, 0);

        public CreateInscripcionClaseDeportiva_test()
        {
            var tipoDeporte1 = new TipoDeporte(1, "Fútbol", "Deporte de equipo", 2);
            var tipoDeporte2 = new TipoDeporte(2, "Baloncesto", "Deporte de equipo", 2);

            _context.AddRange(tipoDeporte1, tipoDeporte2);

            var cliente = new ApplicationUser(
                "1", "Peter", "Jackson", _userName,
                "12345678B", 20, "H");

            cliente.PhoneNumber = "600123456";

            _context.Add(cliente);

            var clases = new List<ClaseDeportiva>() {
                new ClaseDeportiva(1, "Entrenamiento de técnica y posesión", _fechaClase1, "Pista 1", "Carlos Ruiz", "Intermedio", 10, 7.50m, tipoDeporte1, 1, new List<ClaseInscrita>()),
                new ClaseDeportiva(2, "Iniciación al baloncesto", _fechaClase2, "Pista 2", "Marta López", "Iniciación", 8, 6.00m, tipoDeporte2, 2, new List<ClaseInscrita>()),
                new ClaseDeportiva(3, "Perfeccionamiento de saque y volea", _fechaClase3, "Pista 3", "Ana Martín", "Avanzado", 1, 5.00m, tipoDeporte1, 1, new List<ClaseInscrita>())
            };

            _context.AddRange(clases);
            _context.SaveChanges();
        }

        private static ClaseInscritaItemDTO CrearItem(int claseDeportivaId, int plazasReservadas, string? observaciones = null)
        {
            return new ClaseInscritaItemDTO(
                claseDeportivaId, "Datos de prueba", "Fútbol",
                _fechaClase1, plazasReservadas, 0m, observaciones);
        }

        private static InscripcionClaseDeportivaForCreateDTO CrearInscripcion(
            string nombreUsuario, params ClaseInscritaItemDTO[] clasesInscritas)
        {
            return new InscripcionClaseDeportivaForCreateDTO(
                nombreUsuario, MetodoPago.Bizum, "Pago de prueba",
                clasesInscritas.ToList());
        }

        public static IEnumerable<object[]> TestCasesFor_CreateInscripcionClaseDeportiva()
        {
            var inscripcionClasesNulas = CrearInscripcion(_userName);
            inscripcionClasesNulas.ClasesInscritas = null!;

            return new List<object[]>
            {
                new object[] { CrearInscripcion("desconocido@uclm.es", CrearItem(1, 1)), "El nombre de usuario no está registrado." },
                new object[] { CrearInscripcion(_userName), "Debes seleccionar al menos una clase deportiva." },
                new object[] { inscripcionClasesNulas, "Debes seleccionar al menos una clase deportiva." },
                new object[] { CrearInscripcion(_userName, CrearItem(1, 1), CrearItem(1, 1)), "No puedes seleccionar la misma clase dos veces." },
                new object[] { CrearInscripcion(_userName, CrearItem(1, 0)), "Debes reservar entre 1 y 3 plazas por clase." },
                new object[] { CrearInscripcion(_userName, CrearItem(1, 4)), "Debes reservar entre 1 y 3 plazas por clase." },
                new object[] { CrearInscripcion(_userName, CrearItem(999, 1)), "La clase con id 999 no existe." },
                new object[] { CrearInscripcion(_userName, CrearItem(3, 2)), "No hay suficientes plazas en la clase 3." }
            };
        }

        [Theory]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        [MemberData(nameof(TestCasesFor_CreateInscripcionClaseDeportiva))]
        public async Task CreateInscripcionClaseDeportiva_Error_test(
            InscripcionClaseDeportivaForCreateDTO inscripcionForCreate, string errorExpected)
        {
            // Arrange
            var mock = new Mock<ILogger<InscripcionClaseDeportivaController>>();
            ILogger<InscripcionClaseDeportivaController> logger = mock.Object;
            var controller = new InscripcionClaseDeportivaController(_context, logger);

            // Act
            var result = await controller.CreateInscripcionClaseDeportiva(inscripcionForCreate);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var problemDetails = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);
            var errorActual = problemDetails.Errors.First().Value[0];
            Assert.StartsWith(errorExpected, errorActual);

            Assert.Empty(await _context.InscripcionesClasesDeportivas.ToListAsync());

            Assert.Equal(10, (await _context.ClasesDeportivas.FindAsync(1))!.PlazasDisponibles);
            Assert.Equal(8, (await _context.ClasesDeportivas.FindAsync(2))!.PlazasDisponibles);
            Assert.Equal(1, (await _context.ClasesDeportivas.FindAsync(3))!.PlazasDisponibles);
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task CreateInscripcionClaseDeportiva_Success_test()
        {
            // Arrange
            var mock = new Mock<ILogger<InscripcionClaseDeportivaController>>();
            ILogger<InscripcionClaseDeportivaController> logger = mock.Object;
            var controller = new InscripcionClaseDeportivaController(_context, logger);

            var inscripcionForCreate = CrearInscripcion(
                _userName,
                CrearItem(1, 2, "Inscripción de prueba"),
                CrearItem(2, 1));

            var fechaAntes = DateTime.Now;

            // Act
            var result = await controller.CreateInscripcionClaseDeportiva(inscripcionForCreate);

            var fechaDespues = DateTime.Now;

            // Assert
            var createdAtResult = Assert.IsType<CreatedAtActionResult>(result);
            var actualInscripcion = Assert.IsType<InscripcionClaseDeportivaDetailDTO>(createdAtResult.Value);

            Assert.Equal(nameof(InscripcionClaseDeportivaController.GetDetailsInscripcionClaseDeportiva), createdAtResult.ActionName);
            Assert.InRange(actualInscripcion.FechaInscripcion, fechaAntes, fechaDespues);

            var expectedInscripcion = new InscripcionClaseDeportivaDetailDTO(
                actualInscripcion.Id,
                actualInscripcion.FechaInscripcion,
                "12345678B",
                "Peter",
                "Jackson",
                _userName,
                "600123456",
                MetodoPago.Bizum,
                21.00m,
                new List<ClaseInscritaItemDTO>()
                {
                    new ClaseInscritaItemDTO(
                        1, "Entrenamiento de técnica y posesión",
                        "Fútbol", _fechaClase1, 2, 15.00m,
                        "Inscripción de prueba"),

                    new ClaseInscritaItemDTO(
                        2, "Iniciación al baloncesto",
                        "Baloncesto", _fechaClase2, 1, 6.00m,
                        null)
                });

            Assert.Equal(expectedInscripcion, actualInscripcion);

            var inscripcionGuardada = await _context.InscripcionesClasesDeportivas
                .Include(i => i.ClasesInscritas)
                .FirstOrDefaultAsync(i => i.Id == actualInscripcion.Id);

            Assert.NotNull(inscripcionGuardada);
            Assert.Equal(21.00m, inscripcionGuardada.PrecioTotal);
            Assert.Equal(2, inscripcionGuardada.ClasesInscritas.Count);

            Assert.Equal(15.00m, inscripcionGuardada.ClasesInscritas.Single(ci => ci.ClaseDeportivaId == 1).Precio);
            Assert.Equal(6.00m, inscripcionGuardada.ClasesInscritas.Single(ci => ci.ClaseDeportivaId == 2).Precio);

            Assert.Equal(8, (await _context.ClasesDeportivas.FindAsync(1))!.PlazasDisponibles);
            Assert.Equal(7, (await _context.ClasesDeportivas.FindAsync(2))!.PlazasDisponibles);
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task CreateInscripcionClaseDeportiva_ClienteConDatosNulos_test()
        {
            // Arrange
            var cliente = await _context.ApplicationUsers
                .FirstAsync(u => u.UserName == _userName);

            cliente.Dni = null;
            cliente.Name = null;
            cliente.Surname = null;
            cliente.Email = null;
            cliente.PhoneNumber = null;
            await _context.SaveChangesAsync();

            var mock = new Mock<ILogger<InscripcionClaseDeportivaController>>();
            var controller = new InscripcionClaseDeportivaController(_context, mock.Object);
            var inscripcion = CrearInscripcion(_userName, CrearItem(1, 1));

            // Act
            var result = await controller.CreateInscripcionClaseDeportiva(inscripcion);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var dto = Assert.IsType<InscripcionClaseDeportivaDetailDTO>(createdResult.Value);

            Assert.Equal(string.Empty, dto.Dni);
            Assert.Equal(string.Empty, dto.Nombre);
            Assert.Equal(string.Empty, dto.Apellidos);
            Assert.Equal(string.Empty, dto.Email);
            Assert.Equal(string.Empty, dto.NumeroTelefono);
        }
    }
}