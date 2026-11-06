
using System.ComponentModel.DataAnnotations;
using AppForSEII.API.Models;

namespace AppForSEII.API.DTOs.InscripcionClaseDeportivaDTOs
{
    public class InscripcionClaseDeportivaForCreateDTO
    {
        public InscripcionClaseDeportivaForCreateDTO(
            string nombreUsuario,
            MetodoPago metodoPago,
            string datosPago,
            IList<ClaseInscritaItemDTO> clasesInscritas)
        {
            NombreUsuario = nombreUsuario ??
                throw new ArgumentNullException(nameof(nombreUsuario));

            MetodoPago = metodoPago;

            DatosPago = datosPago ??
                throw new ArgumentNullException(nameof(datosPago));

            ClasesInscritas = clasesInscritas ??
                throw new ArgumentNullException(nameof(clasesInscritas));
        }

        public InscripcionClaseDeportivaForCreateDTO()
        {
            ClasesInscritas = new List<ClaseInscritaItemDTO>();
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
        public IList<ClaseInscritaItemDTO> ClasesInscritas { get; set; }
            = new List<ClaseInscritaItemDTO>();

        public override bool Equals(object? obj)
        {
            return obj is InscripcionClaseDeportivaForCreateDTO dto &&
                   NombreUsuario == dto.NombreUsuario &&
                   MetodoPago == dto.MetodoPago &&
                   DatosPago == dto.DatosPago &&
                   ClasesInscritas.SequenceEqual(dto.ClasesInscritas);
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();

            hash.Add(NombreUsuario);
            hash.Add(MetodoPago);
            hash.Add(DatosPago);

            foreach (var clase in ClasesInscritas)
            {
                hash.Add(clase);
            }

            return hash.ToHashCode();
        }
    }
}
