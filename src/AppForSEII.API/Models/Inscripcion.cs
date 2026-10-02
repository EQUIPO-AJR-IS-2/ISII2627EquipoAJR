namespace AppForSEII.API.Models
{
    public class Inscripcion
    {
        public Inscripcion()
        {
        }

        public Inscripcion(DateTime fechaInscripcion, MetodoPago metodoPago, string datosPago,
                           ApplicationUser applicationUser, IList<CompeticionInscrita> competicionesInscritas)
        {
            FechaInscripcion = fechaInscripcion;
            MetodoPago = metodoPago;
            DatosPago = datosPago;
            ApplicationUser = applicationUser;
            CompeticionesInscritas = competicionesInscritas;
            PrecioTotal = competicionesInscritas.Sum(ci => ci.Competicion.Precio);
        }

        public int Id { get; set; }

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

        // Los datos personales del cliente (nombre, apellidos, correo y telefono)
        // se obtienen de ApplicationUser, no se duplican en esta clase.
        [DeleteBehavior(DeleteBehavior.NoAction)]
        public ApplicationUser ApplicationUser { get; set; } = null!;

        public IList<CompeticionInscrita> CompeticionesInscritas { get; set; } = new List<CompeticionInscrita>();
    }
}