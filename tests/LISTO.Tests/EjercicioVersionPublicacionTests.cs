
using LISTO.Domain.Entities;
using LISTO.Domain.Enums;
using Xunit;

namespace LISTO.Tests;

public class EjercicioVersionPublicacionTests
{
    private static (Ejercicio Ejercicio, EjercicioVersion Version)
    CrearVersion()
    {
        var curso = new Curso(
            "Curso de prueba",
            "Descripción de prueba");

        var ejercicio = new Ejercicio(curso);
        var version = ejercicio.CrearVersion();

        return (ejercicio, version);
    }

    [Fact]
    public void PublicarVersion_SinPregunta_LanzaExcepcion()
    {
        var (ejercicio, version) = CrearVersion();
        version.EnviarARevision();
        Assert.Throws<InvalidOperationException>(() =>
            ejercicio.PublicarVersion(version.NumeroVersion));

        Assert.Equal(
            EstadoEjercicioVersion.EnRevision,
            version.Estado);
        Assert.Null(version.FechaPublicacion);
    }

    [Fact]
    public void PublicarVersion_OpcionMultipleSinOpciones_LanzaExcepcion()
    {
        var (ejercicio, version) = CrearVersion();

        version.CrearPregunta(
            "¿Cuál es la respuesta correcta?",
            TipoPregunta.OpcionMultiple);
        version.EnviarARevision();
        Assert.Throws<InvalidOperationException>(() =>
            ejercicio.PublicarVersion(version.NumeroVersion));

        Assert.Equal(EstadoEjercicioVersion.EnRevision, version.Estado);
    }

    [Fact]
    public void PublicarVersion_OpcionMultipleSinCorrecta_LanzaExcepcion()
    {
        var (ejercicio, version) = CrearVersion();

        var pregunta = version.CrearPregunta(
            "¿Cuál es la respuesta correcta?",
            TipoPregunta.OpcionMultiple);

        pregunta.CrearOpcion("Opción A", 1, false);
        pregunta.CrearOpcion("Opción B", 2, false);
        version.EnviarARevision();
        Assert.Throws<InvalidOperationException>(() =>
            ejercicio.PublicarVersion(version.NumeroVersion));
    }

    [Fact]
    public void PublicarVersion_VerdaderoFalsoSinConfiguracion_LanzaExcepcion()
    {
        var (ejercicio, version) = CrearVersion();

        version.CrearPregunta(
            "¿La afirmación es verdadera?",
            TipoPregunta.VerdaderoFalso);
        version.EnviarARevision();
        Assert.Throws<InvalidOperationException>(() =>
            ejercicio.PublicarVersion(version.NumeroVersion));
    }

    [Fact]
    public void PublicarVersion_RespuestaLibreSinConfiguracion_LanzaExcepcion()
    {
        var (ejercicio, version) = CrearVersion();

        version.CrearPregunta(
            "Explique el concepto.",
            TipoPregunta.RespuestaLibre);
        version.EnviarARevision();
        Assert.Throws<InvalidOperationException>(() =>
            ejercicio.PublicarVersion(version.NumeroVersion));
    }

    [Fact]
    public void PublicarVersion_RespuestaLibreConPonderacionInvalida_LanzaExcepcion()
    {
        var (ejercicio, version) = CrearVersion();

        var pregunta = version.CrearPregunta(
            "Explique el concepto.",
            TipoPregunta.RespuestaLibre);

        var configuracion = pregunta.ConfigurarRespuestaLibre(
            "Respuesta de referencia.");

        configuracion.AgregarCriterio(
            "Precisión conceptual",
            60m,
            1);

        configuracion.AgregarCriterio(
            "Claridad",
            30m,
            2);
        version.EnviarARevision();
        Assert.Throws<InvalidOperationException>(() =>
            ejercicio.PublicarVersion(version.NumeroVersion));

        Assert.Equal(EstadoEjercicioVersion.EnRevision, version.Estado);
        Assert.Null(version.FechaPublicacion);
    }

    [Fact]
    public void PublicarVersion_RespuestaLibreValida_PublicaCorrectamente()
    {
        var (ejercicio, version) = CrearVersion();

        var pregunta = version.CrearPregunta(
            "Explique el concepto.",
            TipoPregunta.RespuestaLibre);

        var configuracion = pregunta.ConfigurarRespuestaLibre(
            "Respuesta de referencia.");

        configuracion.AgregarCriterio(
            "Precisión conceptual",
            60m,
            1);

        configuracion.AgregarCriterio(
            "Claridad",
            40m,
            2);
        version.EnviarARevision();
        ejercicio.PublicarVersion(version.NumeroVersion);

        Assert.Equal(EstadoEjercicioVersion.Publicado, version.Estado);
        Assert.NotNull(version.FechaPublicacion);
    }

    [Fact]
    public void PublicarVersion_VerdaderoFalsoValida_PublicaCorrectamente()
    {
        var (ejercicio, version) = CrearVersion();

        var pregunta = version.CrearPregunta(
            "¿La afirmación es verdadera?",
            TipoPregunta.VerdaderoFalso);

        pregunta.ConfigurarVerdaderoFalso(true);
        version.EnviarARevision();
        ejercicio.PublicarVersion(version.NumeroVersion);

        Assert.Equal(EstadoEjercicioVersion.Publicado, version.Estado);
        Assert.NotNull(version.FechaPublicacion);
    }

    [Fact]
    public void PublicarVersion_OpcionMultipleValida_PublicaCorrectamente()
    {
        var (ejercicio, version) = CrearVersion();

        var pregunta = version.CrearPregunta(
            "¿Cuál es la respuesta correcta?",
            TipoPregunta.OpcionMultiple);

        pregunta.CrearOpcion("Opción A", 1, true);
        pregunta.CrearOpcion("Opción B", 2, false);
        version.EnviarARevision();
        ejercicio.PublicarVersion(version.NumeroVersion);

        Assert.Equal(EstadoEjercicioVersion.Publicado, version.Estado);
        Assert.NotNull(version.FechaPublicacion);
    }
}
