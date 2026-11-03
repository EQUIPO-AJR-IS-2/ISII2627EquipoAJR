namespace AppForSEII.API.DTOs.MaterialDTOs
{
    public class MaterialForRentalDTO
    {
        public MaterialForRentalDTO(int idMaterial, string nombreMaterial, string tipoMaterial,
            string tipoDeporte, decimal precio, int cantidad)
        {
            IdMaterial = idMaterial;
            NombreMaterial = nombreMaterial;
            TipoMaterial = tipoMaterial;
            TipoDeporte = tipoDeporte;
            Precio = precio;
            Cantidad = cantidad;
        }

        public int IdMaterial { get; set; }

        [StringLength(50, MinimumLength = 1, ErrorMessage = "El nombre no puede superar los 50 caracteres.")]
        public string NombreMaterial { get; set; }
        public string TipoMaterial { get; set; }
        public string TipoDeporte { get; set; }
        
        [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
        [Precision(5, 2)]
        public decimal Precio { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "La cantidad no puede ser negativa.")]
        public int Cantidad { get; set; }

        // Los tests de referencia comparan listas de DTOs con Assert.Equal, así que hace falta igualdad por valor.
        public override bool Equals(object? obj)
        {
            return obj is MaterialForRentalDTO other
                && IdMaterial == other.IdMaterial
                && NombreMaterial == other.NombreMaterial
                && TipoMaterial == other.TipoMaterial
                && TipoDeporte == other.TipoDeporte
                && Precio == other.Precio
                && Cantidad == other.Cantidad;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(IdMaterial, NombreMaterial, TipoMaterial, TipoDeporte, Precio, Cantidad);
        }
    }
}