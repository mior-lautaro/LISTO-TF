
using LISTO.Domain.Enums;

namespace LISTO.Domain.Entities;

public class Inscripcion
{
    public long InscripcionId { get; private set; }

    public long EstudianteId { get; private set; }
    public long PlanCursoId { get; private set; }

    public DateTime FechaInscripcion { get; private set; }

    public EstadoInscripcion Estado { get; private set; }

    public DateTime? FechaResolucion { get; private set; }

    public Estudiante Estudiante { get; private set; } = null!;
    public PlanCurso PlanCurso { get; private set; } = null!;

    private Inscripcion()
    {
        // Constructor requerido por Entity Framework Core.
    }

    public Inscripcion(Estudiante estudiante, PlanCurso planCurso)
    {
        ArgumentNullException.ThrowIfNull(estudiante);
        ArgumentNullException.ThrowIfNull(planCurso);

        if (!estudiante.EstaActivo)
            throw new InvalidOperationException(
                "No se puede inscribir a un estudiante inactivo.");

        Estudiante = estudiante;
        EstudianteId = estudiante.EstudianteId;

        PlanCurso = planCurso;
        PlanCursoId = planCurso.PlanCursoId;

        FechaInscripcion = DateTime.UtcNow;
        Estado = EstadoInscripcion.Activa;
    }

    public void Aprobar()
    {
        Finalizar(EstadoInscripcion.Aprobada);
    }

    public void Desaprobar()
    {
        Finalizar(EstadoInscripcion.Desaprobada);
    }

    public void Abandonar()
    {
        Finalizar(EstadoInscripcion.Abandonada);
    }

    public void Anular()
    {
        Finalizar(EstadoInscripcion.Anulada);
    }

    private void Finalizar(EstadoInscripcion estadoFinal)
    {
        if (Estado != EstadoInscripcion.Activa)
            throw new InvalidOperationException(
                "Solo se pueden finalizar inscripciones activas.");

        if (estadoFinal == EstadoInscripcion.Activa ||
            !Enum.IsDefined(estadoFinal))
        {
            throw new ArgumentException(
                "El estado final indicado no es válido.",
                nameof(estadoFinal));
        }

        Estado = estadoFinal;
        FechaResolucion = DateTime.UtcNow;
    }
}
