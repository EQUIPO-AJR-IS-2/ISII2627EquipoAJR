namespace AppForSEII.API.DTOs.CompeticionDTOs
{
    public class CompeticionParaInscripcionDTO
    {

        public CompeticionParaInscripcionDTO(int id, string nombre, string tipoDeporte,
                                             DateTime fecha, string lugar, decimal precio)
        {
            Id = id;
            Nombre = nombre;
            TipoDeporte = tipoDeporte;
            Fecha = fecha;
            Lugar = lugar;
            Precio = precio;
        }

        public int Id { get; set; }

        [StringLength(100, ErrorMessage = "El nombre no puede tener mas de 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "El tipo de deporte no puede tener mas de 50 caracteres", MinimumLength = 3)]
        public string TipoDeporte { get; set; } = string.Empty;

        [DataType(System.ComponentModel.DataAnnotations.DataType.Date)]
        public DateTime Fecha { get; set; } = DateTime.Today;

        [StringLength(100, ErrorMessage = "El lugar no puede tener mas de 100 caracteres")]
        public string Lugar { get; set; } = string.Empty;

        [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
        [Range(0, 1000, ErrorMessage = "El precio debe estar entre 0 y 1000")]
        public decimal Precio { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is CompeticionParaInscripcionDTO dTO &&
                   Id == dTO.Id &&
                   Nombre == dTO.Nombre &&
                   TipoDeporte == dTO.TipoDeporte &&
                   Fecha == dTO.Fecha &&
                   Lugar == dTO.Lugar &&
                   Precio == dTO.Precio;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Nombre, TipoDeporte, Fecha, Lugar, Precio);
        }
    }
}