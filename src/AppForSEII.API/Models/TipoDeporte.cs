namespace AppForSEII.API.Models
{
   public class TipoDeporte
   {
      public int Id { get; set; }

      [Required(AllowEmptyStrings = false, ErrorMessage = "El nombre del tipo de deporte es obligatorio")]
      [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 50 caracteres")]
      public string Nombre { get; set; }

      public string? Descripcion { get; set; }

      [StringLength(200)]
      public string? Competiciones { get; set; }

      [Range(0, int.MaxValue, ErrorMessage = "El número de pistas no puede ser negativo.")]
      public int Pistas { get; set; }

      // Relación con Materiales: de uno a muchos (un tipo de deporte puede tener varios materiales).
      public IList<Material> Materiales { get; set; } = new List<Material>();
   }
}