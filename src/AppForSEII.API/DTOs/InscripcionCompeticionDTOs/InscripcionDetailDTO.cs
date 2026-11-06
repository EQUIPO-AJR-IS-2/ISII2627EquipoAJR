using AppForSEII.API.Models;

namespace AppForSEII.API.DTOs.InscripcionCompeticionDTO
{
    public class InscripcionDetailDTO
    {
        public InscripcionDetailDTO(
            int id,
            DateTime fechaInscripcion,
            string dni,
            string nombre,
            string apellidos,
            string email,
            string numeroTelefono,
            MetodoPago metodoPago,
            decimal precioTotal,
            IList<CompeticionInscritaItemDTO> competicionesInscritas)
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
            CompeticionesInscritas = competicionesInscritas;
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
        public IList<CompeticionInscritaItemDTO> CompeticionesInscritas { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is InscripcionDetailDTO dto &&
                   Id == dto.Id &&
                   FechaInscripcion == dto.FechaInscripcion &&
                   Dni == dto.Dni &&
                   Nombre == dto.Nombre &&
                   Apellidos == dto.Apellidos &&
                   Email == dto.Email &&
                   NumeroTelefono == dto.NumeroTelefono &&
                   MetodoPago == dto.MetodoPago &&
                   PrecioTotal == dto.PrecioTotal &&
                   CompeticionesInscritas.SequenceEqual(dto.CompeticionesInscritas);
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

            foreach (var competicion in CompeticionesInscritas)
            {
                hash.Add(competicion);
            }

            return hash.ToHashCode();
        }
    }
}