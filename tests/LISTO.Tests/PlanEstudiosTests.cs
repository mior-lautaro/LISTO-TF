
using System;
using LISTO.Domain.Entities;
using LISTO.Domain.Enums;
using Xunit;

namespace LISTO.Tests.Domain;

public class PlanEstudiosTests
{
    private static PlanEstudios CrearPlan()
    {
        return new PlanEstudios(
            "PLA-000001",
            "Plan de Inglés",
            "Plan de estudios de inglés.");
    }

    private static Curso CrearCurso(
        string codigo = "CUR-000001",
        string nombre = "Inglés I")
    {
        return new Curso(codigo, nombre);
    }

    private static PlanEstudiosVersion CrearVersionPublicable(
        PlanEstudios plan,
        int numeroVersion = 1,
        DateOnly? fecha = null)
    {
        var version = plan.CrearVersion(
            numeroVersion,
            fecha ?? new DateOnly(2026, 3, 1));

        version.AgregarCurso(
            CrearCurso(),
            1,
            PeriodoRecomendado.Anual,
            120);

        return version;
    }

    [Fact]
    public void Constructor_DatosValidos_CreaPlanActivo()
    {
        var plan = CrearPlan();

        Assert.Equal("PLA-000001", plan.Codigo);
        Assert.Equal("Plan de Inglés", plan.Nombre);
        Assert.Equal("Plan de estudios de inglés.", plan.Descripcion);
        Assert.True(plan.EstaActivo);
        Assert.Empty(plan.Versiones);
        Assert.Equal(0, plan.PlanEstudiosId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_CodigoVacio_LanzaExcepcion(string codigo)
    {
        Assert.Throws<ArgumentException>(
            () => new PlanEstudios(codigo, "Plan de Inglés"));
    }

    [Fact]
    public void Constructor_CodigoDemasiadoLargo_LanzaExcepcion()
    {
        Assert.Throws<ArgumentException>(
            () => new PlanEstudios(
                new string('A', 21),
                "Plan de Inglés"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_NombreVacio_LanzaExcepcion(string nombre)
    {
        Assert.Throws<ArgumentException>(
            () => new PlanEstudios("PLA-000001", nombre));
    }

    [Fact]
    public void Constructor_NombreDemasiadoLargo_LanzaExcepcion()
    {
        Assert.Throws<ArgumentException>(
            () => new PlanEstudios(
                "PLA-000001",
                new string('A', 151)));
    }

    [Fact]
    public void Constructor_DescripcionDemasiadoLarga_LanzaExcepcion()
    {
        Assert.Throws<ArgumentException>(
            () => new PlanEstudios(
                "PLA-000001",
                "Plan de Inglés",
                new string('A', 501)));
    }

    [Fact]
    public void Constructor_NormalizaEspacios()
    {
        var plan = new PlanEstudios(
            " PLA-000001 ",
            " Plan de Inglés ",
            " Descripción ");

        Assert.Equal("PLA-000001", plan.Codigo);
        Assert.Equal("Plan de Inglés", plan.Nombre);
        Assert.Equal("Descripción", plan.Descripcion);
    }

    [Fact]
    public void CrearVersion_NumeroValido_CreaVersionBorrador()
    {
        var plan = CrearPlan();

        var version = plan.CrearVersion(
            1,
            new DateOnly(2026, 3, 1));

        Assert.Equal(1, version.NumeroVersion);
        Assert.Equal(
            EstadoPlanEstudiosVersion.Borrador,
            version.Estado);
        Assert.Single(plan.Versiones);
    }

    [Fact]
    public void CrearVersion_NumeroNoPositivo_LanzaExcepcion()
    {
        var plan = CrearPlan();

        Assert.Throws<ArgumentException>(
            () => plan.CrearVersion(0));
    }

    [Fact]
    public void CrearVersion_NumeroRepetido_LanzaExcepcion()
    {
        var plan = CrearPlan();

        plan.CrearVersion(1);

        Assert.Throws<InvalidOperationException>(
            () => plan.CrearVersion(1));
    }

    [Fact]
    public void CrearVersion_NumeroMenorAlAnterior_LanzaExcepcion()
    {
        var plan = CrearPlan();

        plan.CrearVersion(2);

        Assert.Throws<InvalidOperationException>(
            () => plan.CrearVersion(1));
    }

    [Fact]
    public void CrearVersion_NumeroMayorAlAnterior_CreaVersion()
    {
        var plan = CrearPlan();

        plan.CrearVersion(1);
        var version = plan.CrearVersion(2);

        Assert.Equal(2, version.NumeroVersion);
        Assert.Equal(2, plan.Versiones.Count);
    }

    [Fact]
    public void CrearVersion_PlanInactivo_LanzaExcepcion()
    {
        var plan = CrearPlan();
        plan.Desactivar();

        Assert.Throws<InvalidOperationException>(
            () => plan.CrearVersion(1));
    }

    [Fact]
    public void ObtenerVersion_VersionExistente_RetornaVersion()
    {
        var plan = CrearPlan();
        var creada = plan.CrearVersion(1);

        var obtenida = plan.ObtenerVersion(1);

        Assert.Same(creada, obtenida);
    }

    [Fact]
    public void ObtenerVersion_VersionInexistente_RetornaNull()
    {
        var plan = CrearPlan();

        Assert.Null(plan.ObtenerVersion(1));
    }

    [Fact]
    public void ModificarDatos_DatosValidos_ActualizaDatos()
    {
        var plan = CrearPlan();

        plan.ModificarDatos(
            "Plan de Inglés Avanzado",
            "Nueva descripción");

        Assert.Equal("PLA-000001", plan.Codigo);
        Assert.Equal("Plan de Inglés Avanzado", plan.Nombre);
        Assert.Equal("Nueva descripción", plan.Descripcion);
    }

    [Fact]
    public void ModificarDatos_NombreVacio_LanzaExcepcion()
    {
        var plan = CrearPlan();

        Assert.Throws<ArgumentException>(
            () => plan.ModificarDatos("", null));
    }

    [Fact]
    public void ModificarDatos_DescripcionVacia_AsignaNull()
    {
        var plan = CrearPlan();

        plan.ModificarDatos("Plan de Inglés", "   ");

        Assert.Null(plan.Descripcion);
    }

    [Fact]
    public void PublicarVersion_VersionValida_CambiaEstadoAVigente()
    {
        var plan = CrearPlan();
        var version = CrearVersionPublicable(plan);

        plan.PublicarVersion(1);

        Assert.Equal(
            EstadoPlanEstudiosVersion.Vigente,
            version.Estado);
    }

    [Fact]
    public void PublicarVersion_VersionInexistente_LanzaExcepcion()
    {
        var plan = CrearPlan();

        Assert.Throws<InvalidOperationException>(
            () => plan.PublicarVersion(1));
    }

    [Fact]
    public void PublicarVersion_PlanInactivo_LanzaExcepcion()
    {
        var plan = CrearPlan();
        CrearVersionPublicable(plan);
        plan.Desactivar();

        Assert.Throws<InvalidOperationException>(
            () => plan.PublicarVersion(1));
    }

    [Fact]
    public void PublicarVersion_OtraVersionVigente_LanzaExcepcion()
    {
        var plan = CrearPlan();

        CrearVersionPublicable(plan, 1);
        CrearVersionPublicable(plan, 2);

        plan.PublicarVersion(1);

        Assert.Throws<InvalidOperationException>(
            () => plan.PublicarVersion(2));
    }

    [Fact]
    public void FinalizarVersionVigente_VersionVigente_CambiaEstado()
    {
        var plan = CrearPlan();
        var version = CrearVersionPublicable(plan);

        plan.PublicarVersion(1);

        plan.FinalizarVersionVigente(
            new DateOnly(2026, 12, 1));

        Assert.Equal(
            EstadoPlanEstudiosVersion.NoVigente,
            version.Estado);

        Assert.Equal(
            new DateOnly(2026, 12, 1),
            version.FechaVigenciaHasta);
    }

    [Fact]
    public void FinalizarVersionVigente_SinVersionVigente_LanzaExcepcion()
    {
        var plan = CrearPlan();

        Assert.Throws<InvalidOperationException>(
            () => plan.FinalizarVersionVigente(
                new DateOnly(2026, 12, 1)));
    }

    [Fact]
    public void Desactivar_PlanSinVersionVigente_DesactivaPlan()
    {
        var plan = CrearPlan();

        plan.Desactivar();

        Assert.False(plan.EstaActivo);
    }

    [Fact]
    public void Desactivar_PlanConVersionVigente_LanzaExcepcion()
    {
        var plan = CrearPlan();
        CrearVersionPublicable(plan);

        plan.PublicarVersion(1);

        Assert.Throws<InvalidOperationException>(
            () => plan.Desactivar());
    }

    [Fact]
    public void Activar_PlanInactivo_ActivaPlan()
    {
        var plan = CrearPlan();

        plan.Desactivar();
        plan.Activar();

        Assert.True(plan.EstaActivo);
    }
}
