namespace AppForSEII.API.DTOs.AlquilerDTOs
{
    // Resumen de un alquiler (paso 7): datos del cliente, método de pago, materiales alquilados y precio total.
    public class AlquilerDetailDTO
    {
        public AlquilerDetailDTO(int idAlquiler, DateTime fechaAlquiler, string dni, string nombre,
            string apellidos, string numeroTelefono, MetodoPago metodoPago, decimal precioTotal,
            IList<AlquilerItemDTO> materialesAlquilados)
        {
            IdAlquiler = idAlquiler;
            FechaAlquiler = fechaAlquiler;
            Dni = dni;
            Nombre = nombre;
            Apellidos = apellidos;
            NumeroTelefono = numeroTelefono;
            MetodoPago = metodoPago;
            PrecioTotal = precioTotal;
            MaterialesAlquilados = materialesAlquilados;
        }

        public int IdAlquiler { get; set; }
        public DateTime FechaAlquiler { get; set; }
        //Al igual que en AppForMovies esto viene del ApplicationUser.
        public string Dni { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string NumeroTelefono { get; set; }
        public MetodoPago MetodoPago { get; set; }
        public decimal PrecioTotal { get; set; }
        public IList<AlquilerItemDTO> MaterialesAlquilados { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is AlquilerDetailDTO dto
                && IdAlquiler == dto.IdAlquiler
                && FechaAlquiler == dto.FechaAlquiler
                && Dni == dto.Dni
                && Nombre == dto.Nombre
                && Apellidos == dto.Apellidos
                && NumeroTelefono == dto.NumeroTelefono
                && MetodoPago == dto.MetodoPago
                && PrecioTotal == dto.PrecioTotal
                && MaterialesAlquilados.SequenceEqual(dto.MaterialesAlquilados);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(IdAlquiler, FechaAlquiler, Dni, Nombre, Apellidos,
                MetodoPago, PrecioTotal);
        }
    }
}