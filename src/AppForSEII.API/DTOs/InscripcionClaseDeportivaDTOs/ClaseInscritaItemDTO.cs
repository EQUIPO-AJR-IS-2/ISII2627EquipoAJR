
namespace AppForSEII.API.DTOs.InscripcionClaseDeportivaDTOs
{
    public class ClaseInscritaItemDTO
    {
        public ClaseInscritaItemDTO(
            int claseDeportivaId,
            string descripcion,
            string tipoDeporte,
            DateTime fechaHora,
            int plazasReservadas,
            decimal precio,
            string? observaciones)
        {
            ClaseDeportivaId = claseDeportivaId;
            Descripcion = descripcion;
            TipoDeporte = tipoDeporte;
            FechaHora = fechaHora;
            PlazasReservadas = plazasReservadas;
            Precio = precio;
            Observaciones = observaciones;
        }

        public int ClaseDeportivaId { get; set; }
        public string Descripcion { get; set; }
        public string TipoDeporte { get; set; }
        public DateTime FechaHora { get; set; }
        public int PlazasReservadas { get; set; }
        public decimal Precio { get; set; }
        public string? Observaciones { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is ClaseInscritaItemDTO dto &&
                   ClaseDeportivaId == dto.ClaseDeportivaId &&
                   Descripcion == dto.Descripcion &&
                   TipoDeporte == dto.TipoDeporte &&
                   FechaHora == dto.FechaHora &&
                   PlazasReservadas == dto.PlazasReservadas &&
                   Precio == dto.Precio &&
                   Observaciones == dto.Observaciones;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                ClaseDeportivaId,
                Descripcion,
                TipoDeporte,
                FechaHora,
                PlazasReservadas,
                Precio,
                Observaciones
            );
        }
    }
}
