
using AppForSEII.API.Models;

namespace AppForSEII.API.DTOs.InscripcionClaseDeportivaDTOs
{
    public class InscripcionClaseDeportivaDetailDTO
    {
        public InscripcionClaseDeportivaDetailDTO(
            int id,
            DateTime fechaInscripcion,
            string dni,
            string nombre,
            string apellidos,
            string email,
            string numeroTelefono,
            MetodoPago metodoPago,
            decimal precioTotal,
            IList<ClaseInscritaItemDTO> clasesInscritas)
        {
            Id = id;
            FechaInscripcion = fechaInscripcion;
            Dni = dni;
            Nombre = nombre;
            Apellidos = apellidos;
            Email = email;
            NumeroTelefono = numeroTelefono;
            MetodoPago = metodoPago;
            PrecioTotal = precioTotal;
            ClasesInscritas = clasesInscritas;
        }

        public int Id { get; set; }
        public DateTime FechaInscripcion { get; set; }
        public string Dni { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Email { get; set; }
        public string NumeroTelefono { get; set; }
        public MetodoPago MetodoPago { get; set; }
        public decimal PrecioTotal { get; set; }
        public IList<ClaseInscritaItemDTO> ClasesInscritas { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is InscripcionClaseDeportivaDetailDTO dto &&
                   Id == dto.Id &&
                   FechaInscripcion == dto.FechaInscripcion &&
                   Dni == dto.Dni &&
                   Nombre == dto.Nombre &&
                   Apellidos == dto.Apellidos &&
                   Email == dto.Email &&
                   NumeroTelefono == dto.NumeroTelefono &&
                   MetodoPago == dto.MetodoPago &&
                   PrecioTotal == dto.PrecioTotal &&
                   ClasesInscritas.SequenceEqual(dto.ClasesInscritas);
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(Id);
            hash.Add(FechaInscripcion);
            hash.Add(Dni);
            hash.Add(Nombre);
            hash.Add(Apellidos);
            hash.Add(Email);
            hash.Add(NumeroTelefono);
            hash.Add(MetodoPago);
            hash.Add(PrecioTotal);

            foreach (var clase in ClasesInscritas)
            {
                hash.Add(clase);
            }

            return hash.ToHashCode();
        }
    }
}
