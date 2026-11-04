namespace AppForSEII.API.DTOs.AlquilerDTOs
{
    // Una línea de un alquiler en el paso 7.
    public class AlquilerItemDTO
    {
        public AlquilerItemDTO(int idMaterial, string nombreMaterial, string tipoMaterial,
            int cantidad, decimal precio)
        {
            IdMaterial = idMaterial;
            NombreMaterial = nombreMaterial;
            TipoMaterial = tipoMaterial;
            Cantidad = cantidad;
            Precio = precio;
        }

        public int IdMaterial { get; set; }
        public string NombreMaterial { get; set; }
        public string TipoMaterial { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; } //En base a Cantidad 

        public override bool Equals(object? obj)
        {
            return obj is AlquilerItemDTO dto
                && IdMaterial == dto.IdMaterial
                && NombreMaterial == dto.NombreMaterial
                && TipoMaterial == dto.TipoMaterial
                && Cantidad == dto.Cantidad
                && Precio == dto.Precio;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(IdMaterial, NombreMaterial, TipoMaterial, Cantidad, Precio);
        }
    }
}