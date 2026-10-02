namespace AppForSEII.API.Models
{
    [PrimaryKey(nameof(ClaseDeportivaId), nameof(InscripcionClaseDeportivaId))]
    public class ClaseInscrita
    {
        public ClaseInscrita()
        {
        }
        public ClaseInscrita(ClaseDeportiva claseDeportiva, int claseDeportivaId, InscripcionClaseDeportiva inscripcionClaseDeportiva, int inscripcionClaseDeportivaId, string? observaciones, int plazasReservadas, decimal precio)
        {
            ClaseDeportiva = claseDeportiva;
            ClaseDeportivaId = claseDeportivaId;
            InscripcionClaseDeportiva = inscripcionClaseDeportiva;
            InscripcionClaseDeportivaId = inscripcionClaseDeportivaId;
            Observaciones = observaciones;
            PlazasReservadas = plazasReservadas;
            Precio = precio;
        }

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