namespace AppForSEII.API.Models
{
    public class InscripcionClaseDeportiva
    {
        public int Id { get; set; }

        public IList<ClaseInscrita> ClasesInscritas { get; set; }
            = new List<ClaseInscrita>();

        public ApplicationUser Cliente { get; set; }

        [Required]
        [StringLength(100)]
        public string DatosPago { get; set; }

        [DataType(System.ComponentModel.DataAnnotations.DataType.Date)]
        public DateTime FechaInscripcion { get; set; }

        public MetodoPago MetodoPago { get; set; }

        [Precision(10, 2)]
        public decimal PrecioTotal { get; set; }
    }
}