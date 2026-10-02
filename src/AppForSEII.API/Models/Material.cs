namespace AppForSEII.API.Models
{
    public class Material
    {
         public Material()
        {
        }

        public Material(int idMaterial, string nombreMaterial, decimal precio, int cantidad, TipoMaterial tipoMaterial, TipoDeporte tipoDeporte, IList<MaterialAlquilado> materialesAlquilados)
        {
            IdMaterial = idMaterial;
            NombreMaterial = nombreMaterial;
            Precio = precio;
            Cantidad = cantidad;
            TipoMaterial = tipoMaterial;
            TipoDeporte = tipoDeporte;
            MaterialesAlquilados = materialesAlquilados;
        }

        [Key] //Igual que antes, se pone porque el nombre no sigue el convenio EF Core
        public int IdMaterial { get; set; }

        [StringLength(50, MinimumLength = 1, ErrorMessage = "El nombre no puede superar los 50 caracteres.")]
        public  string NombreMaterial { get; set; }

        [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
        [Precision(5, 2)]
        public decimal Precio { get; set; }
 
        // Stock disponible de este material
        [Range(0, int.MaxValue, ErrorMessage = "La cantidad no puede ser negativa.")]
        public int Cantidad { get; set; }

        // Relación con TipoMaterial: de uno a muchos (un tipo de material puede tener varios materiales).
        public TipoMaterial TipoMaterial { get; set; }

        // Relación con TipoDeporte: de uno a muchos (un tipo de deporte puede tener varios materiales).
        public TipoDeporte TipoDeporte { get; set; }

        // Relación con MaterialAlquilado: de uno a muchos (un material puede estar en varios alquileres).
        public IList<MaterialAlquilado> MaterialesAlquilados { get; set; } = new List<MaterialAlquilado>();
        
    }
}