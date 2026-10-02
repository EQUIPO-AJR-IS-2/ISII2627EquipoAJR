namespace AppForSEII.API.Models
{
    public class TipoMaterial
    {
        public TipoMaterial(){}
        public TipoMaterial(int idTipoMaterial, string nombreTipoMaterial, IList<Material> materiales)
        {
            IdTipoMaterial = idTipoMaterial;
            NombreTipoMaterial = nombreTipoMaterial;
            Materiales = materiales;
        }

        [Key] //Se pone porque el nombre no sigue el convenio EF Core
        public int IdTipoMaterial { get; set; }

        [StringLength(50, MinimumLength = 1, ErrorMessage = "El nombre no puede superar los 50 caracteres.")]
        public string NombreTipoMaterial { get; set; }

        // Relación con Materiales: de uno a muchos (un tipo de deporte puede tener varios materiales).
        public IList<Material> Materiales { get; set; } = new List<Material>();
    }
}