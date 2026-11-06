using System.ComponentModel.DataAnnotations;
using AppForSEII.API.Models;

namespace AppForSEII.API.DTOs.InscripcionCompeticionDTO
{
    public class InscripcionForCreateDTO
    {
        public InscripcionForCreateDTO(
            string nombreUsuario,
            MetodoPago metodoPago,
            string datosPago,
            IList<CompeticionInscritaItemDTO> competicionesInscritas)
        {
            NombreUsuario = nombreUsuario ??
                throw new ArgumentNullException(nameof(nombreUsuario));

            MetodoPago = metodoPago;

            DatosPago = datosPago ??
                throw new ArgumentNullException(nameof(datosPago));

            CompeticionesInscritas = competicionesInscritas ??
                throw new ArgumentNullException(nameof(competicionesInscritas));
        }

        public InscripcionForCreateDTO()
        {
            CompeticionesInscritas = new List<CompeticionInscritaItemDTO>();
        }

        [Required]
        [EmailAddress]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required]
        [EnumDataType(typeof(MetodoPago))]
        public MetodoPago MetodoPago { get; set; }

        [Required]
        [StringLength(100)]
        public string DatosPago { get; set; } = string.Empty;

        [Required]
        [MinLength(1)]
        public IList<CompeticionInscritaItemDTO> CompeticionesInscritas { get; set; }
            = new List<CompeticionInscritaItemDTO>();

        public override bool Equals(object? obj)
        {
            return obj is InscripcionForCreateDTO dto &&
                   NombreUsuario == dto.NombreUsuario &&
                   MetodoPago == dto.MetodoPago &&
                   DatosPago == dto.DatosPago &&
                   CompeticionesInscritas.SequenceEqual(dto.CompeticionesInscritas);
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();

            hash.Add(NombreUsuario);
            hash.Add(MetodoPago);
            hash.Add(DatosPago);

            foreach (var competicion in CompeticionesInscritas)
            {
                hash.Add(competicion);
            }

            return hash.ToHashCode();
        }
    }
}