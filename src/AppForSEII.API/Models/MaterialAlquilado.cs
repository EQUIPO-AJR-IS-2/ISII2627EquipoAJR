namespace AppForSEII.API.Models
{
    public class MaterialAlquilado
    {
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad mínima es 1.")]
        public int Cantidad { get; set; }
 
        [StringLength(200)]
        public string? Descripcion { get; set; }
 
        // Precio unitario 
        [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
        [Precision(5, 2)]
        public decimal Precio { get; set; }

        public required Material Material { get; set; }
        public int IdMaterial { get; set; }
 
        /*public required Alquiler Alquiler { get; set; }
        public int IdAlquiler { get; set; }*/
    }
}