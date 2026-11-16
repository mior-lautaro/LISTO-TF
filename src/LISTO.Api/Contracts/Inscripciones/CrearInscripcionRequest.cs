
using System.ComponentModel.DataAnnotations;

namespace LISTO.Api.Contracts.Inscripciones;

public class CrearInscripcionRequest
{
    [Required]
    public long EstudianteId { get; set; }

    [Required]
    public long PlanCursoId { get; set; }
}
