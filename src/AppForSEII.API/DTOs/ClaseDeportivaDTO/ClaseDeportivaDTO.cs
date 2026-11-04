
using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.DTOs.ClaseDeportivaDTO
{
    public class ClaseDeportivaDTO
    {
        public ClaseDeportivaDTO(
            int id,
            string descripcion,
            string tipoDeporte,
            string monitor,
            string nivel,
            DateTime fechaHora,
            string? lugar,
            int plazasDisponibles,
            decimal precioUnitario)
        {
            Id = id;
            Descripcion = descripcion;
            TipoDeporte = tipoDeporte;
            Monitor = monitor;
            Nivel = nivel;
            FechaHora = fechaHora;
            Lugar = lugar;
            PlazasDisponibles = plazasDisponibles;
            PrecioUnitario = precioUnitario;
        }

        public int Id { get; set; }

        [Required]
        [StringLength(250)]
        public string Descripcion { get; set; } = string.Empty;

        [Required]
        public string TipoDeporte { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Monitor { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Nivel { get; set; } = string.Empty;

        public DateTime FechaHora { get; set; }

        public string? Lugar { get; set; }

        [Range(0, 30)]
        public int PlazasDisponibles { get; set; }

        public decimal PrecioUnitario { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is ClaseDeportivaDTO other &&
                   Id == other.Id &&
                   Descripcion == other.Descripcion &&
                   TipoDeporte == other.TipoDeporte &&
                   Monitor == other.Monitor &&
                   Nivel == other.Nivel &&
                   FechaHora == other.FechaHora &&
                   Lugar == other.Lugar &&
                   PlazasDisponibles == other.PlazasDisponibles &&
                   PrecioUnitario == other.PrecioUnitario;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                HashCode.Combine(
                    Id,
                    Descripcion,
                    TipoDeporte,
                    Monitor,
                    Nivel,
                    FechaHora,
                    Lugar,
                    PlazasDisponibles
                ),
                PrecioUnitario
            );
        }
    }
}
