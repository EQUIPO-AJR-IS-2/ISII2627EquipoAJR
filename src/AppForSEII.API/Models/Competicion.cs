namespace AppForSEII.API.Models
{
    // El nombre de la competicion es unico
    
    public class Competicion
    {
        public Competicion()
        {
        }

        public Competicion(string nombre, TipoDeporte tipoDeporte, DateTime fecha, string lugar, int plazas, decimal precio)
        {
            Nombre = nombre;
            TipoDeporte = tipoDeporte;
            Fecha = fecha;
            Lugar = lugar;
            Plazas = plazas;
            Precio = precio;
        }

        public int Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "El nombre de la competicion es obligatorio")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres")]
       
        public string Nombre { get; set; }

        [DataType(System.ComponentModel.DataAnnotations.DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    
        public DateTime Fecha { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "El lugar de celebracion es obligatorio")]
        [StringLength(100, ErrorMessage = "El lugar no puede tener mas de 100 caracteres")]
        
        public string Lugar { get; set; }

    
        [Range(0, int.MaxValue, ErrorMessage = "El numero de plazas no puede ser negativo")]
        public int Plazas { get; set; }

        [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
        [Range(0, 1000, ErrorMessage = "El precio debe estar entre 0 y 1000")]
       
        [Precision(10, 2)]
        public decimal Precio { get; set; }

        public TipoDeporte TipoDeporte { get; set; }

        public int TipoDeporteId { get; set; }

        public IList<CompeticionInscrita> CompeticionesInscritas { get; set; } = new List<CompeticionInscrita>();
    
    }
}