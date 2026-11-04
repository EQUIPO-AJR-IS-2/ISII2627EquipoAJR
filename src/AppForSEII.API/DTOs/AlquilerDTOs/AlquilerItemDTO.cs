namespace AppForSEII.API.DTOs.AlquilerDTOs
{
    // Una línea de un alquiler en el paso 7.
    public class AlquilerItemDTO
    {
        public AlquilerItemDTO(int idMaterial, string nombreMaterial, string tipoMaterial, string tipoDeporte,
            int cantidad, decimal precio, string? descripcion = null)
        {
            IdMaterial = idMaterial;
            NombreMaterial = nombreMaterial;
            TipoMaterial = tipoMaterial;
            TipoDeporte = tipoDeporte;
            Cantidad = cantidad;
            Precio = precio;
            Descripcion = descripcion;
        }

        public int IdMaterial { get; set; }
        public string NombreMaterial { get; set; }
        public string TipoMaterial { get; set; }
        public string TipoDeporte { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; } //En base a Cantidad 
        // La descripción es opcional, ya que no todos los materiales tienen descripción.
        [StringLength(200, ErrorMessage = "La descripción no puede superar los 200 caracteres.")]
        public string? Descripcion { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is AlquilerItemDTO dto
                && IdMaterial == dto.IdMaterial
                && NombreMaterial == dto.NombreMaterial
                && TipoMaterial == dto.TipoMaterial
                && TipoDeporte == dto.TipoDeporte
                && Cantidad == dto.Cantidad
                && Precio == dto.Precio
                && Descripcion == dto.Descripcion;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(IdMaterial, NombreMaterial, TipoMaterial, Cantidad, Precio);
        }
    }
}