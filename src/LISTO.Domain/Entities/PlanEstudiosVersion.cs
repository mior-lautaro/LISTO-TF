using LISTO.Domain.Enums;

namespace LISTO.Domain.Entities;

public class PlanEstudiosVersion
{
    private readonly List<PlanCurso> _cursos = new();

    public long PlanEstudiosVersionId { get; private set; }
    public long PlanEstudiosId { get; private set; }

    public int NumeroVersion { get; private set; }

    public DateOnly? FechaVigenciaDesde { get; private set; }
    public DateOnly? FechaVigenciaHasta { get; private set; }

    public EstadoPlanEstudiosVersion Estado { get; private set; }

    public string? Observaciones { get; private set; }

    public IReadOnlyCollection<PlanCurso> Cursos =>
        _cursos.AsReadOnly();

    private PlanEstudiosVersion()
    {
        // Constructor requerido por Entity Framework Core.
    }

    internal PlanEstudiosVersion(
        long planEstudiosId,
        int numeroVersion,
        DateOnly? fechaVigenciaDesde,
        string? observaciones)
    {
        if (numeroVersion <= 0)
            throw new ArgumentException(
                "El número de versión debe ser mayor que cero.");

        ValidarObservaciones(observaciones);

        PlanEstudiosId = planEstudiosId;
        NumeroVersion = numeroVersion;
        FechaVigenciaDesde = fechaVigenciaDesde;
        Observaciones = NormalizarObservaciones(observaciones);
        Estado = EstadoPlanEstudiosVersion.Borrador;
    }

    public PlanCurso AgregarCurso(
        Curso curso,
        int anioNivel,
        PeriodoRecomendado periodoRecomendado,
        decimal cargaHoraria)
    {
        ValidarQueSeaBorrador();

        ArgumentNullException.ThrowIfNull(curso);

        if (!curso.EstaActivo)
            throw new InvalidOperationException(
                "No se pueden incorporar cursos inactivos al plan.");

        if (_cursos.Any(c =>
             ReferenceEquals(c.Curso, curso) ||
             (curso.CursoId != 0 && c.CursoId == curso.CursoId)))
         {
            throw new InvalidOperationException(
                "El curso ya se encuentra incluido en esta versión del plan.");
         }

        var planCurso = new PlanCurso(
            PlanEstudiosVersionId,
            curso,
            anioNivel,
            periodoRecomendado,
            cargaHoraria);

        _cursos.Add(planCurso);

        return planCurso;
    }

    public void ModificarCurso(
        long cursoId,
        int anioNivel,
        PeriodoRecomendado periodoRecomendado,
        decimal cargaHoraria)
    {
        ValidarQueSeaBorrador();

        var planCurso = _cursos.FirstOrDefault(
            c => c.CursoId == cursoId);

        if (planCurso is null)
            throw new InvalidOperationException(
                "El curso no pertenece a esta versión del plan.");

        planCurso.Modificar(
            anioNivel,
            periodoRecomendado,
            cargaHoraria);
    }

    public void QuitarCurso(long cursoId)
    {
        ValidarQueSeaBorrador();

        var planCurso = _cursos.FirstOrDefault(
            c => c.CursoId == cursoId);

        if (planCurso is null)
            throw new InvalidOperationException(
                "El curso no pertenece a esta versión del plan.");

        _cursos.Remove(planCurso);
    }

    public void ModificarObservaciones(string? observaciones)
    {
        ValidarQueSeaBorrador();
        ValidarObservaciones(observaciones);

        Observaciones = NormalizarObservaciones(observaciones);
    }

    internal void Publicar()
    {
        ValidarQueSeaBorrador();

        if (_cursos.Count == 0)
            throw new InvalidOperationException(
                "No se puede publicar una versión sin cursos.");

        if (FechaVigenciaDesde is null)
            throw new InvalidOperationException(
                "Debe establecerse la fecha de inicio de vigencia.");

        Estado = EstadoPlanEstudiosVersion.Vigente;
    }

    internal void FinalizarVigencia(DateOnly fechaVigenciaHasta)
    {
        if (Estado != EstadoPlanEstudiosVersion.Vigente)
            throw new InvalidOperationException(
                "Solo se puede finalizar la vigencia de una versión vigente.");

        if (FechaVigenciaDesde.HasValue &&
            fechaVigenciaHasta < FechaVigenciaDesde.Value)
            throw new ArgumentException(
                "La fecha de finalización no puede ser anterior al inicio de vigencia.");

        FechaVigenciaHasta = fechaVigenciaHasta;
        Estado = EstadoPlanEstudiosVersion.NoVigente;
    }

    private void ValidarQueSeaBorrador()
    {
        if (Estado != EstadoPlanEstudiosVersion.Borrador)
            throw new InvalidOperationException(
                "Solo se pueden modificar versiones en estado Borrador.");
    }

    private static void ValidarObservaciones(string? observaciones)
    {
        if (observaciones?.Trim().Length > 1000)
            throw new ArgumentException(
                "Las observaciones no pueden superar los 1000 caracteres.");
    }

    private static string? NormalizarObservaciones(string? observaciones)
    {
        return string.IsNullOrWhiteSpace(observaciones)
            ? null
            : observaciones.Trim();
    }
}