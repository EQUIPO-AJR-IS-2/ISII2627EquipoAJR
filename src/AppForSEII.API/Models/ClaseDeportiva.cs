using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII.API.Models
{
    public class ClaseDeportiva
    {
        public ClaseDeportiva()
        {
        }

        public ClaseDeportiva(int id, string descripcion, DateTime fechaHora, string? lugar, string monitor, string nivel, int plazasDisponibles, decimal precioUnitario, TipoDeporte tipoDeporte, int tipoDeporteId, IList<ClaseInscrita> clasesInscritas)
        {
            Id = id;
            Descripcion = descripcion;
            FechaHora = fechaHora;
            Lugar = lugar;
            Monitor = monitor;
            Nivel = nivel;
            PlazasDisponibles = plazasDisponibles;
            PrecioUnitario = precioUnitario;
            TipoDeporte = tipoDeporte;
            TipoDeporteId = tipoDeporteId;
            ClasesInscritas = clasesInscritas;
        }

        public int Id { get; set; }

        [Required]
        public string Descripcion { get; set; } = null!;

        public DateTime FechaHora { get; set; }

        public string? Lugar { get; set; }

        [Required]
        public string Monitor { get; set; } = null!;

        [Required]
        public string Nivel { get; set; } = null!;

        [Range(0, 30)]
        public int PlazasDisponibles { get; set; }

        [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
        [Precision(10, 2)]
        public decimal PrecioUnitario { get; set; }

        public TipoDeporte TipoDeporte { get; set; } = null!;

        public int TipoDeporteId { get; set; }

        public IList<ClaseInscrita> ClasesInscritas { get; set; }
            = new List<ClaseInscrita>();
    }
}