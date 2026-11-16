using LISTO.Domain.Enums;

namespace LISTO.Domain.Entities;

public class PlanCurso
{
    public long PlanCursoId { get; private set; }

    public long PlanEstudiosVersionId { get; private set; }
    public long CursoId { get; private set; }

    public int AnioNivel { get; private set; }
    public PeriodoRecomendado PeriodoRecomendado { get; private set; }
    public decimal CargaHoraria { get; private set; }

    public Curso Curso { get; private set; } = null!;

    private PlanCurso()
    {
        // Constructor requerido por Entity Framework Core.
    }

    internal PlanCurso(
        long planEstudiosVersionId,
        Curso curso,
        int anioNivel,
        PeriodoRecomendado periodoRecomendado,
        decimal cargaHoraria)
    {
        ArgumentNullException.ThrowIfNull(curso);

        ValidarAnioNivel(anioNivel);
        ValidarPeriodo(periodoRecomendado);
        ValidarCargaHoraria(cargaHoraria);

        PlanEstudiosVersionId = planEstudiosVersionId;
        Curso = curso;
        CursoId = curso.CursoId;
        AnioNivel = anioNivel;
        PeriodoRecomendado = periodoRecomendado;
        CargaHoraria = cargaHoraria;
    }

    internal void Modificar(
        int anioNivel,
        PeriodoRecomendado periodoRecomendado,
        decimal cargaHoraria)
    {
        ValidarAnioNivel(anioNivel);
        ValidarPeriodo(periodoRecomendado);
        ValidarCargaHoraria(cargaHoraria);

        AnioNivel = anioNivel;
        PeriodoRecomendado = periodoRecomendado;
        CargaHoraria = cargaHoraria;
    }

    private static void ValidarAnioNivel(int anioNivel)
    {
        if (anioNivel <= 0)
            throw new ArgumentException(
                "El año o nivel debe ser mayor que cero.");
    }

    private static void ValidarPeriodo(
        PeriodoRecomendado periodoRecomendado)
    {
        if (!Enum.IsDefined(periodoRecomendado))
            throw new ArgumentException(
                "El período recomendado no es válido.");
    }

    private static void ValidarCargaHoraria(decimal cargaHoraria)
    {
        if (cargaHoraria <= 0)
            throw new ArgumentException(
                "La carga horaria total debe ser mayor que cero.");
    }
}