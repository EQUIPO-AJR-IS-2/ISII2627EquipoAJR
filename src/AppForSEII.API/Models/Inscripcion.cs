namespace AppForSEII.API.Models
{
    public class Inscripcion
    {
        public Inscripcion()
        {
        }

        public Inscripcion(string nombreUsuario, string apellidosUsuario, string dni, string telefono,
                           string email, DateTime fechaInscripcion, MetodoPago metodoPago, string datosPago,
                           ApplicationUser applicationUser)
        {
            NombreUsuario = nombreUsuario;
            ApellidosUsuario = apellidosUsuario;
            DNI = dni;
            Telefono = telefono;
            Email = email;
            FechaInscripcion = fechaInscripcion;
            MetodoPago = metodoPago;
            DatosPago = datosPago;
            ApplicationUser = applicationUser;
            
        }

        public int Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre no puede tener mas de 50 caracteres")]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Los apellidos son obligatorios")]
        [StringLength(100, ErrorMessage = "Los apellidos no pueden tener mas de 100 caracteres")]
        public string ApellidosUsuario { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = false, ErrorMessage = "El DNI es obligatorio")]
        [StringLength(9, MinimumLength = 9, ErrorMessage = "El DNI debe tener 9 caracteres")]
        public string DNI { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = false, ErrorMessage = "El telefono es obligatorio")]
        [Phone]
        [StringLength(15, MinimumLength = 9, ErrorMessage = "El telefono debe tener entre 9 y 15 caracteres")]
        public string Telefono { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = false, ErrorMessage = "El correo electronico es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del correo electronico no es valido")]
        [StringLength(100, ErrorMessage = "El correo no puede tener mas de 100 caracteres")]
        public string Email { get; set; } = string.Empty;

        [DataType(System.ComponentModel.DataAnnotations.DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaInscripcion { get; set; }

        public MetodoPago MetodoPago { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Los datos del pago son obligatorios")]
        [StringLength(100, ErrorMessage = "Los datos del pago no pueden tener mas de 100 caracteres")]
        public string DatosPago { get; set; } = string.Empty;

        [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
        [Precision(10, 2)]
        public decimal PrecioTotal { get; set; }

        // Usuario conectado que realiza la inscripcion
        [DeleteBehavior(DeleteBehavior.NoAction)]
        public ApplicationUser ApplicationUser { get; set; } = null!;

        
    }


}