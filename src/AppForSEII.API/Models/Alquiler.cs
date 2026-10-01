namespace AppForSEII.API.Models
{
    public class Alquiler
    {
        [Key] //Igual, este Key no haría falta si el nombre del diagrama fuese otro pero al ser así...
        public int IdAlquiler { get; set; }

        [StringLength(50, MinimumLength = 1, ErrorMessage = "El nombre no puede superar los 50 caracteres.")]
        public required string NombreUsuario { get; set; }

        [StringLength(100, MinimumLength = 1, ErrorMessage = "Los apellidos no pueden superar los 100 caracteres.")]
        public required string ApellidosUsuario { get; set; }

        [StringLength(9, MinimumLength = 9, ErrorMessage = "El DNI debe tener 9 caracteres.")]
        public required string DNI { get; set; }

        [DataType(System.ComponentModel.DataAnnotations.DataType.PhoneNumber)]
        public required string NumeroTelefono { get; set; }

         // Fecha para la que el cliente reserva el material
        [DataType(System.ComponentModel.DataAnnotations.DataType.Date)]
        public DateTime FechaAlquiler { get; set; }

        [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
        [Precision(7, 2)]
        public decimal PrecioTotal { get; set; }

        //Relacion con la clase MaterialAlquilado, que representa los materiales dentro de este alquiler
        public IList<MaterialAlquilado> MaterialesAlquilados { get; set; } = new List<MaterialAlquilado>();

        public required MetodoPago MetodoPago { get; set; }
    }
}