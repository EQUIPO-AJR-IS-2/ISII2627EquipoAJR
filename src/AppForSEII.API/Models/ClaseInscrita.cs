namespace AppForSEII.API.Models
{
    public class ClaseInscrita
    {
        public int Id { get; set; }


        // Relación con ClaseDeportiva
        public int ClaseDeportivaId { get; set; }

        public ClaseDeportiva ClaseDeportiva { get; set; } = null!;


        // Relación con usuario inscrito
        public string ApplicationUserId { get; set; } = null!;

        public ApplicationUser ApplicationUser { get; set; } = null!;


        public DateTime FechaInscripcion { get; set; }

        public bool Confirmada { get; set; }
    }
}