
using System;
using LISTO.Domain.Entities;
using LISTO.Domain.Enums;
using Xunit;

namespace LISTO.Tests.Domain;

public class PlanEstudiosVersionTests
{
    private static PlanEstudios CrearPlan()
    {
        return new PlanEstudios(
            "PLA-000001",
            "Plan de Inglés");
    }

    private static Curso CrearCurso(
        string codigo = "CUR-000001",
        string nombre = "Inglés I")
    {
        return new Curso(codigo, nombre);
    }

    private static PlanEstudiosVersion CrearVersion()
    {
        return CrearPlan().CrearVersion(
            1,
            new DateOnly(2026, 3, 1));
    }

    [Fact]
    public void CrearVersion_EstadoInicial_EsBorrador()
    {
        var version = CrearVersion();

        Assert.Equal(
            EstadoPlanEstudiosVersion.Borrador,
            version.Estado);

        Assert.Equal(1, version.NumeroVersion);
        Assert.Empty(version.Cursos);
    }

    [Fact]
    public void AgregarCurso_CursoValido_IncorporaCurso()
    {
        var version = CrearVersion();
        var curso = CrearCurso();

        var planCurso = version.AgregarCurso(
            curso,
            1,
            PeriodoRecomendado.Anual,
            120);

        Assert.Single(version.Cursos);
        Assert.Same(curso, planCurso.Curso);
        Assert.Equal(1, planCurso.AnioNivel);
        Assert.Equal(120, planCurso.CargaHoraria);
    }

    [Fact]
    public void AgregarCurso_MismoCursoDosVeces_LanzaExcepcion()
    {
        var version = CrearVersion();
        var curso = CrearCurso();

        version.AgregarCurso(
            curso,
            1,
            PeriodoRecomendado.Anual,
            120);

        Assert.Throws<InvalidOperationException>(
            () => version.AgregarCurso(
                curso,
                1,
                PeriodoRecomendado.Anual,
                120));
    }

    [Fact]
    public void AgregarCurso_CursoInactivo_LanzaExcepcion()
    {
        var version = CrearVersion();
        var curso = CrearCurso();

        curso.Desactivar();

        Assert.Throws<InvalidOperationException>(
            () => version.AgregarCurso(
                curso,
                1,
                PeriodoRecomendado.Anual,
                120));
    }

    [Fact]
    public void AgregarCurso_CursoNulo_LanzaExcepcion()
    {
        var version = CrearVersion();

        Assert.Throws<ArgumentNullException>(
            () => version.AgregarCurso(
                null!,
                1,
                PeriodoRecomendado.Anual,
                120));
    }

    [Fact]
    public void AgregarCurso_AnioNivelNoPositivo_LanzaExcepcion()
    {
        var version = CrearVersion();

        Assert.Throws<ArgumentException>(
            () => version.AgregarCurso(
                CrearCurso(),
                0,
                PeriodoRecomendado.Anual,
                120));
    }

    [Fact]
    public void AgregarCurso_PeriodoInvalido_LanzaExcepcion()
    {
        var version = CrearVersion();

        Assert.Throws<ArgumentException>(
            () => version.AgregarCurso(
                CrearCurso(),
                1,
                (PeriodoRecomendado)99,
                120));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void AgregarCurso_CargaHorariaNoPositiva_LanzaExcepcion(
        decimal cargaHoraria)
    {
        var version = CrearVersion();

        Assert.Throws<ArgumentException>(
            () => version.AgregarCurso(
                CrearCurso(),
                1,
                PeriodoRecomendado.Anual,
                cargaHoraria));
    }

    [Fact]
    public void ModificarCurso_CursoExistente_ActualizaDatos()
    {
        var version = CrearVersion();
        var curso = CrearCurso();

        version.AgregarCurso(
            curso,
            1,
            PeriodoRecomendado.Anual,
            120);

        version.ModificarCurso(
            curso.CursoId,
            2,
            PeriodoRecomendado.PrimerSemestre,
            80);

        var planCurso = Assert.Single(version.Cursos);

        Assert.Equal(2, planCurso.AnioNivel);
        Assert.Equal(
            PeriodoRecomendado.PrimerSemestre,
            planCurso.PeriodoRecomendado);
        Assert.Equal(80, planCurso.CargaHoraria);
    }

    [Fact]
    public void ModificarCurso_CursoInexistente_LanzaExcepcion()
    {
        var version = CrearVersion();

        Assert.Throws<InvalidOperationException>(
            () => version.ModificarCurso(
                999,
                1,
                PeriodoRecomendado.Anual,
                120));
    }

    [Fact]
    public void QuitarCurso_CursoExistente_EliminaCurso()
    {
        var version = CrearVersion();
        var curso = CrearCurso();

        version.AgregarCurso(
            curso,
            1,
            PeriodoRecomendado.Anual,
            120);

        version.QuitarCurso(curso.CursoId);

        Assert.Empty(version.Cursos);
    }

    [Fact]
    public void QuitarCurso_CursoInexistente_LanzaExcepcion()
    {
        var version = CrearVersion();

        Assert.Throws<InvalidOperationException>(
            () => version.QuitarCurso(999));
    }

    [Fact]
    public void ModificarObservaciones_TextoValido_ActualizaObservaciones()
    {
        var version = CrearVersion();

        version.ModificarObservaciones("Observaciones nuevas");

        Assert.Equal("Observaciones nuevas", version.Observaciones);
    }

    [Fact]
    public void ModificarObservaciones_TextoVacio_AsignaNull()
    {
        var version = CrearVersion();

        version.ModificarObservaciones("   ");

        Assert.Null(version.Observaciones);
    }

    [Fact]
    public void ModificarObservaciones_TextoDemasiadoLargo_LanzaExcepcion()
    {
        var version = CrearVersion();

        Assert.Throws<ArgumentException>(
            () => version.ModificarObservaciones(
                new string('A', 1001)));
    }

    [Fact]
    public void Publicar_SinCursos_LanzaExcepcion()
    {
        var plan = CrearPlan();
        plan.CrearVersion(1, new DateOnly(2026, 3, 1));

        Assert.Throws<InvalidOperationException>(
            () => plan.PublicarVersion(1));
    }

    [Fact]
    public void Publicar_SinFechaInicio_LanzaExcepcion()
    {
        var plan = CrearPlan();
        var version = plan.CrearVersion(1);

        version.AgregarCurso(
            CrearCurso(),
            1,
            PeriodoRecomendado.Anual,
            120);

        Assert.Throws<InvalidOperationException>(
            () => plan.PublicarVersion(1));
    }

    [Fact]
    public void Publicar_ConCursosYFecha_CambiaEstado()
    {
        var plan = CrearPlan();
        var version = plan.CrearVersion(
            1,
            new DateOnly(2026, 3, 1));

        version.AgregarCurso(
            CrearCurso(),
            1,
            PeriodoRecomendado.Anual,
            120);

        plan.PublicarVersion(1);

        Assert.Equal(
            EstadoPlanEstudiosVersion.Vigente,
            version.Estado);
    }

    [Fact]
    public void FinalizarVigencia_FechaAnteriorAlInicio_LanzaExcepcion()
    {
        var plan = CrearPlan();
        var version = plan.CrearVersion(
            1,
            new DateOnly(2026, 3, 1));

        version.AgregarCurso(
            CrearCurso(),
            1,
            PeriodoRecomendado.Anual,
            120);

        plan.PublicarVersion(1);

        Assert.Throws<ArgumentException>(
            () => plan.FinalizarVersionVigente(
                new DateOnly(2026, 2, 28)));
    }

    [Fact]
    public void VersionPublicada_NoPermiteAgregarCursos()
    {
        var plan = CrearPlan();
        var version = plan.CrearVersion(
            1,
            new DateOnly(2026, 3, 1));

        version.AgregarCurso(
            CrearCurso(),
            1,
            PeriodoRecomendado.Anual,
            120);

        plan.PublicarVersion(1);

        Assert.Throws<InvalidOperationException>(
            () => version.AgregarCurso(
                CrearCurso("CUR-000002", "Inglés II"),
                2,
                PeriodoRecomendado.Anual,
                120));
    }

    [Fact]
    public void VersionPublicada_NoPermiteModificarCursos()
    {
        var plan = CrearPlan();
        var version = plan.CrearVersion(
            1,
            new DateOnly(2026, 3, 1));

        var curso = CrearCurso();

        version.AgregarCurso(
            curso,
            1,
            PeriodoRecomendado.Anual,
            120);

        plan.PublicarVersion(1);

        Assert.Throws<InvalidOperationException>(
            () => version.ModificarCurso(
                curso.CursoId,
                2,
                PeriodoRecomendado.Anual,
                120));
    }

    [Fact]
    public void VersionPublicada_NoPermiteQuitarCursos()
    {
        var plan = CrearPlan();
        var version = plan.CrearVersion(
            1,
            new DateOnly(2026, 3, 1));

        var curso = CrearCurso();

        version.AgregarCurso(
            curso,
            1,
            PeriodoRecomendado.Anual,
            120);

        plan.PublicarVersion(1);

        Assert.Throws<InvalidOperationException>(
            () => version.QuitarCurso(curso.CursoId));
    }

    [Fact]
    public void VersionPublicada_NoPermiteModificarObservaciones()
    {
        var plan = CrearPlan();
        var version = plan.CrearVersion(
            1,
            new DateOnly(2026, 3, 1));

        version.AgregarCurso(
            CrearCurso(),
            1,
            PeriodoRecomendado.Anual,
            120);

        plan.PublicarVersion(1);

        Assert.Throws<InvalidOperationException>(
            () => version.ModificarObservaciones("Cambio"));
    }
}
