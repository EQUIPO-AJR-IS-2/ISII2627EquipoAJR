using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using AppForSEII2526.API.Models;

namespace AppForSEII.API.Models
{
    public class ClaseDeportiva
    {
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

        public Deporte TipoDeporte { get; set; }
    }
}