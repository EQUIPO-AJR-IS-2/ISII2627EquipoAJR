namespace AppForSEII.API.Models
{
    public class TipoMaterial
    {
       [Key] //Se pone porque el nombre no sigue el convenio EF Core
        public int IdTipoMaterial { get; set; }

        [StringLength(50, MinimumLength = 1, ErrorMessage = "El nombre no puede superar los 50 caracteres.")]
        public string NombreTipoMaterial { get; set; }

        
    }
}