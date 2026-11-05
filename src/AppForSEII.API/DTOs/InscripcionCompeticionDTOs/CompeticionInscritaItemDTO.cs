namespace AppForSEII.API.DTOs.InscripcionCompeticionDTO
{
    public class CompeticionInscritaItemDTO
    {
        public CompeticionInscritaItemDTO(
            int competicionId,
            string nombre,
            string tipoDeporte,
            DateTime fecha,
            decimal precio,
            string? problemasFisicos)
        {
            CompeticionId = competicionId;
            Nombre = nombre;
            TipoDeporte = tipoDeporte;
            Fecha = fecha;
            Precio = precio;
            ProblemasFisicos = problemasFisicos;
        }

        public int CompeticionId { get; set; }
        public string Nombre { get; set; }
        public string TipoDeporte { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Precio { get; set; }
        public string? ProblemasFisicos { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is CompeticionInscritaItemDTO dto &&
                   CompeticionId == dto.CompeticionId &&
                   Nombre == dto.Nombre &&
                   TipoDeporte == dto.TipoDeporte &&
                   Fecha == dto.Fecha &&
                   Precio == dto.Precio &&
                   ProblemasFisicos == dto.ProblemasFisicos;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                CompeticionId,
                Nombre,
                TipoDeporte,
                Fecha,
                Precio,
                ProblemasFisicos
            );
        }
    }
}