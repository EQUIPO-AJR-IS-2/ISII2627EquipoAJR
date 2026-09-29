namespace AppForSEII.API.Models
{
    // El nombre del tipo de deporte es unico
    [Index(nameof(Nombre), IsUnique = true)]
    public class TipoDeporte
    {
        public TipoDeporte()
        {
        }

        public TipoDeporte(string nombre)
        {
            Nombre = nombre;
        }

        public int Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "El nombre del tipo de deporte es obligatorio")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 50 caracteres")]
        public string Nombre { get; set; }

        
    }
}