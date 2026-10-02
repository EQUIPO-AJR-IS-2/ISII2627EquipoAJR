namespace AppForSEII.API.Models
{
    [PrimaryKey(nameof(ClaseDeportivaId), nameof(InscripcionClaseDeportivaId))]
    public class ClaseInscrita
    {
        public ClaseDeportiva ClaseDeportiva { get; set; } = null!;

        public int ClaseDeportivaId { get; set; }


        public InscripcionClaseDeportiva InscripcionClaseDeportiva { get; set; } = null!;

        public int InscripcionClaseDeportivaId { get; set; }


        public string? Observaciones { get; set; }

        public int PlazasReservadas { get; set; }

        [Precision(10, 2)]
        public decimal Precio { get; set; }
    }
}