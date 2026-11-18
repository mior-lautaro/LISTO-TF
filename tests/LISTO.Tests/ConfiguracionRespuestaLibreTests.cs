using LISTO.Domain.Entities;
using LISTO.Domain.Enums;
using Xunit;

namespace LISTO.Tests;

public class ConfiguracionRespuestaLibreTests
{
    private static Pregunta CrearPreguntaRespuestaLibre()
    {
        var curso = new Curso(
            "Curso de prueba",
            "Descripción de prueba");

        var ejercicio = new Ejercicio(curso);
        var version = ejercicio.CrearVersion();

        return version.CrearPregunta(
            "Explique el concepto solicitado.",
            TipoPregunta.RespuestaLibre);
    }

    [Fact]
    public void ConfigurarRespuestaLibre_DatosValidos_CreaConfiguracion()
    {
        var pregunta = CrearPreguntaRespuestaLibre();
        const string respuestaReferencia = "Respuesta modelo de prueba.";

        var configuracion = pregunta.ConfigurarRespuestaLibre(
            respuestaReferencia);

        Assert.NotNull(configuracion);
        Assert.Equal(
            respuestaReferencia,
            configuracion.RespuestaReferencia);
        Assert.Equal(
            pregunta.PreguntaId,
            configuracion.PreguntaId);
        Assert.Same(pregunta, configuracion.Pregunta);
        Assert.Same(
            configuracion,
            pregunta.ConfiguracionRespuestaLibre);
    }

    [Fact]
    public void ConfigurarRespuestaLibre_RespuestaVacia_LanzaExcepcion()
    {
        var pregunta = CrearPreguntaRespuestaLibre();

        Assert.Throws<ArgumentException>(() =>
            pregunta.ConfigurarRespuestaLibre(" "));
    }

    [Fact]
    public void ConfigurarRespuestaLibre_DosVeces_LanzaExcepcion()
    {
        var pregunta = CrearPreguntaRespuestaLibre();

        pregunta.ConfigurarRespuestaLibre("Respuesta modelo.");

        Assert.Throws<InvalidOperationException>(() =>
            pregunta.ConfigurarRespuestaLibre("Otra respuesta."));
    }

    [Fact]
    public void ConfigurarRespuestaLibre_PreguntaDeOtroTipo_LanzaExcepcion()
    {
        var curso = new Curso(
            "Curso de prueba",
            "Descripción de prueba");

        var ejercicio = new Ejercicio(curso);
        var version = ejercicio.CrearVersion();

        var pregunta = version.CrearPregunta(
            "¿Cuál es la respuesta correcta?",
            TipoPregunta.OpcionMultiple);

        Assert.Throws<InvalidOperationException>(() =>
            pregunta.ConfigurarRespuestaLibre("Respuesta modelo."));
    }

    [Fact]
    public void ConfigurarRespuestaLibre_VersionEnRevision_LanzaExcepcion()
    {
        var pregunta = CrearPreguntaRespuestaLibre();

        pregunta.EjercicioVersion.EnviarARevision();

        Assert.Throws<InvalidOperationException>(() =>
            pregunta.ConfigurarRespuestaLibre("Respuesta modelo."));
    }

    [Fact]
    public void ConfigurarRespuestaLibre_VersionPublicada_LanzaExcepcion()
    {
        var curso = new Curso(
            "Curso de prueba",
            "Descripción de prueba");

        var ejercicio = new Ejercicio(curso);
        var version = ejercicio.CrearVersion();

        var pregunta = version.CrearPregunta(
            "Explique el concepto solicitado.",
            TipoPregunta.RespuestaLibre);

        var configuracion = pregunta.ConfigurarRespuestaLibre(
            "Respuesta modelo.");

        configuracion.AgregarCriterio(
            "Comprensión del concepto",
            60m,
            1);

        configuracion.AgregarCriterio(
            "Claridad de la explicación",
            40m,
            2);

        version.EnviarARevision();
        ejercicio.PublicarVersion(version.NumeroVersion);

        Assert.Throws<InvalidOperationException>(() =>
            pregunta.ConfigurarRespuestaLibre("Otra respuesta modelo."));
    }
}
