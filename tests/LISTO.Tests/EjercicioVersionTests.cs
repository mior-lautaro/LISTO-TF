
using LISTO.Domain.Entities;
using LISTO.Domain.Enums;

namespace LISTO.Tests;

public class EjercicioVersionTests
{
    private static EjercicioVersion CrearVersion()
    {
        var ejercicio = new Ejercicio(
            new Curso("CUR-000001", "Matemática"));

        return ejercicio.CrearVersion();
    }

    [Fact]
    public void CrearVersion_EstadoInicial_EsBorrador()
    {
        var version = CrearVersion();

        Assert.Equal(
            EstadoEjercicioVersion.Borrador,
            version.Estado);

        Assert.Equal(1, version.NumeroVersion);
        Assert.NotEqual(default, version.FechaCreacion);
        Assert.Null(version.FechaPublicacion);
    }

    [Fact]
    public void EnviarARevision_Borrador_CambiaEstado()
    {
        var version = CrearVersion();

        version.EnviarARevision();

        Assert.Equal(
            EstadoEjercicioVersion.EnRevision,
            version.Estado);
    }

    [Fact]
    public void EnviarARevision_VersionYaEnRevision_LanzaExcepcion()
    {
        var version = CrearVersion();
        version.EnviarARevision();

        Assert.Throws<InvalidOperationException>(
            () => version.EnviarARevision());
    }

    [Fact]
    public void DevolverABorrador_VersionEnRevision_CambiaEstado()
    {
        var version = CrearVersion();
        version.EnviarARevision();

        version.DevolverABorrador();

        Assert.Equal(
            EstadoEjercicioVersion.Borrador,
            version.Estado);
    }

    [Fact]
    public void DevolverABorrador_VersionBorrador_LanzaExcepcion()
    {
        var version = CrearVersion();

        Assert.Throws<InvalidOperationException>(
            () => version.DevolverABorrador());
    }

    [Fact]
    public void Publicar_VersionEnRevision_CambiaEstadoYFecha()
    {
        var version = CrearVersion();

        var pregunta = version.CrearPregunta(
            "¿La afirmación es verdadera?",
            TipoPregunta.VerdaderoFalso);

        pregunta.ConfigurarVerdaderoFalso(true);

        version.EnviarARevision();

        version.GetType()
            .GetMethod(
                "Publicar",
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic)!
            .Invoke(version, null);

        Assert.Equal(
            EstadoEjercicioVersion.Publicado,
            version.Estado);

        Assert.NotNull(version.FechaPublicacion);
    }

    [Fact]
    public void Publicar_VersionBorrador_NoPermitePublicacion()
    {
        var version = CrearVersion();

        var metodo = version.GetType().GetMethod(
            "Publicar",
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.NonPublic)!;

        var excepcion = Assert.Throws<
            System.Reflection.TargetInvocationException>(
            () => metodo.Invoke(version, null));

        Assert.IsType<InvalidOperationException>(
            excepcion.InnerException);
    }

    [Fact]
    public void Retirar_VersionPublicada_CambiaEstado()
    {
        var version = CrearVersion();

        var pregunta = version.CrearPregunta(
            "¿La afirmación es verdadera?",
            TipoPregunta.VerdaderoFalso);

        pregunta.ConfigurarVerdaderoFalso(true);

        version.EnviarARevision();

        version.GetType()
            .GetMethod(
                "Publicar",
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic)!
            .Invoke(version, null);

        version.GetType()
            .GetMethod(
                "Retirar",
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic)!
            .Invoke(version, null);

        Assert.Equal(
            EstadoEjercicioVersion.Retirado,
            version.Estado);
    }

    [Fact]
    public void Retirar_VersionBorrador_NoPermiteRetirada()
    {
        var version = CrearVersion();

        var metodo = version.GetType().GetMethod(
            "Retirar",
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.NonPublic)!;

        var excepcion = Assert.Throws<
            System.Reflection.TargetInvocationException>(
            () => metodo.Invoke(version, null));

        Assert.IsType<InvalidOperationException>(
            excepcion.InnerException);
    }
}
