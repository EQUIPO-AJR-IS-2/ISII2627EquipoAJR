namespace AppForSEII.API.Models
{
    // Entidad de unión: Material (1)-(N) MaterialAlquilado (N)-(1) Alquiler
    [PrimaryKey(nameof(IdMaterial), nameof(IdAlquiler))]
    public class MaterialAlquilado
    {
        public MaterialAlquilado()
        {
        }

        public MaterialAlquilado(int cantidad, string? descripcion, decimal precio, Material material, Alquiler alquiler)
        {
            Cantidad = cantidad;
            Descripcion = descripcion;
            Precio = precio;
            Material = material;
            Alquiler = alquiler;
        }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad mínima es 1.")]
        public int Cantidad { get; set; }
 
        [StringLength(200)]
        public string? Descripcion { get; set; }
 
        // Precio unitario 
        [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
        [Precision(5, 2)]
        public decimal Precio { get; set; }

        public Material Material { get; set; }
        public int IdMaterial { get; set; }
 
        public Alquiler Alquiler { get; set; }
        public int IdAlquiler { get; set; }
    }
}