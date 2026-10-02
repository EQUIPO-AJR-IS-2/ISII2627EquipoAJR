namespace AppForSEII.API.Models
{

    [PrimaryKey(nameof(CompeticionId), nameof(InscripcionId))]
public class CompeticionInscrita
    {
        public CompeticionInscrita()
        {
        }
 
        public CompeticionInscrita(Competicion competicion, Inscripcion inscripcion)
        {
            Competicion = competicion;
            CompeticionId = competicion.Id;
            Inscripcion = inscripcion;
            InscripcionId = inscripcion.Id;
        }
 
        public CompeticionInscrita(Competicion competicion, Inscripcion inscripcion, string? problemasFisicos)
            : this(competicion, inscripcion)
        {
            ProblemasFisicos = problemasFisicos;
        }
 
        public Competicion Competicion { get; set; } = null!;
 
        public int CompeticionId { get; set; }
 
        public Inscripcion Inscripcion { get; set; } = null!;
 
        public int InscripcionId { get; set; }
 
        [StringLength(250, ErrorMessage = "Las observaciones no pueden tener mas de 250 caracteres")]
        [DataType(System.ComponentModel.DataAnnotations.DataType.MultilineText)]
        public string? ProblemasFisicos { get; set; }
        public IList<CompeticionInscrita> CompeticionesInscritas { get; set; } = new List<CompeticionInscrita>();
    }
}