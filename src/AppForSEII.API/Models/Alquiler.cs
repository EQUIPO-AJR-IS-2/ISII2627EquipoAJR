namespace AppForSEII.API.Models
{
    public class Alquiler
    {
        public Alquiler()
        {
        }
        public Alquiler(int idAlquiler, DateTime fechaAlquiler, decimal precioTotal, IList<MaterialAlquilado> materialesAlquilados, MetodoPago metodoPago, ApplicationUser cliente)
        {
            IdAlquiler = idAlquiler;
            FechaAlquiler = fechaAlquiler;
            PrecioTotal = precioTotal;
            MaterialesAlquilados = materialesAlquilados;
            MetodoPago = metodoPago;
            User = cliente;
        }

        [Key] //Igual, este Key no haría falta si el nombre del diagrama fuese otro pero al ser así...
        public int IdAlquiler { get; set; }

         // Fecha para la que el cliente reserva el material
        [DataType(System.ComponentModel.DataAnnotations.DataType.Date)]
        public DateTime FechaAlquiler { get; set; }

        [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
        [Precision(7, 2)]
        public decimal PrecioTotal { get; set; }

        //Relacion con la clase MaterialAlquilado, que representa los materiales dentro de este alquiler
        public IList<MaterialAlquilado> MaterialesAlquilados { get; set; } = new List<MaterialAlquilado>();

        public  MetodoPago MetodoPago { get; set; }

        //Relacion con la clase Cliente, que representa al cliente que realiza el alquiler
        public ApplicationUser User { get; set; }
    }
}