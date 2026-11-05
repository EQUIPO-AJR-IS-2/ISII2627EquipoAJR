namespace AppForSEII.API.DTOs.AlquilerDTOs
{
    // Datos que envía el cliente al crear un alquiler (pasos 5 y 6), excepto los datos del cliente que se obtienen de ApplicationUser.
    public class AlquilerForCreateDTO
    {
        public AlquilerForCreateDTO()
        {
            UserName = "";
            FechaAlquiler = DateTime.Today;
            MaterialesAlquilados = new List<AlquilerItemDTO>();
        }

        public AlquilerForCreateDTO(string userName, DateTime fechaAlquiler, MetodoPago metodoPago,
            IList<AlquilerItemDTO> materialesAlquilados)
        {
            UserName = userName;
            FechaAlquiler = fechaAlquiler;
            MetodoPago = metodoPago;
            MaterialesAlquilados = materialesAlquilados;
        }

        [Required(ErrorMessage = "El usuario es obligatorio.")]
        [EmailAddress(ErrorMessage = "El usuario debe ser un email válido.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "La fecha del alquiler es obligatoria.")]
        public DateTime FechaAlquiler { get; set; }

        [Required(ErrorMessage = "El método de pago es obligatorio.")]
        public MetodoPago MetodoPago { get; set; }

        [Required(ErrorMessage = "Debe incluir al menos un material.")]
        public IList<AlquilerItemDTO> MaterialesAlquilados { get; set; }

        // No sirve para nada tener esto aquí. El controller calcula alquiler.PrecioTotal por su cuenta, y el cliente no puede enviar un precio total que no coincida con la suma de los precios de los materiales.
        // Como el cliente envía precio: 0 en cada línea, aquí valdría siempre 0 al llegar al POST. Solo tendría un valor útil en un test, si le pasas líneas con precio.
        // Es cierto que en AppForMovies tiene TotalPrice, pero aquí ns si debe ser así.
        public decimal PrecioTotal => MaterialesAlquilados.Sum(i => i.Precio * i.Cantidad); 

        public override bool Equals(object? obj)
        {
            return obj is AlquilerForCreateDTO dto
                && UserName == dto.UserName
                && FechaAlquiler == dto.FechaAlquiler
                && MetodoPago == dto.MetodoPago
                && MaterialesAlquilados.SequenceEqual(dto.MaterialesAlquilados);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(UserName, FechaAlquiler, MetodoPago);
        }
    }
}