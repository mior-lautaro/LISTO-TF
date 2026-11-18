using LISTO.Domain.Entities;
using LISTO.Domain.Enums;
using Xunit;

namespace LISTO.Tests;

public class ConfiguracionVerdaderoFalsoTests
{
    private static Pregunta CrearPreguntaVerdaderoFalso()
    {
        var curso = new Curso(
            "Curso de prueba",
            "Descripción de prueba");

        var ejercicio = new Ejercicio(curso);
        var version = ejercicio.CrearVersion();

        return version.CrearPregunta(
            "¿La afirmación es verdadera?",
            TipoPregunta.VerdaderoFalso);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ConfigurarVerdaderoFalso_DatosValidos_CreaConfiguracion(
        bool respuestaCorrecta)
    {
        var pregunta = CrearPreguntaVerdaderoFalso();

        var configuracion = pregunta.ConfigurarVerdaderoFalso(
            respuestaCorrecta);

        Assert.NotNull(configuracion);
        Assert.Equal(
            respuestaCorrecta,
            configuracion.RespuestaCorrecta);
        Assert.Equal(
            pregunta.PreguntaId,
            configuracion.PreguntaId);
        Assert.Same(pregunta, configuracion.Pregunta);
        Assert.Same(
            configuracion,
            pregunta.ConfiguracionVerdaderoFalso);
    }

    [Fact]
    public void ConfigurarVerdaderoFalso_DosVeces_LanzaExcepcion()
    {
        var pregunta = CrearPreguntaVerdaderoFalso();

        pregunta.ConfigurarVerdaderoFalso(true);

        Assert.Throws<InvalidOperationException>(() =>
            pregunta.ConfigurarVerdaderoFalso(false));
    }

    [Fact]
    public void ConfigurarVerdaderoFalso_PreguntaDeOtroTipo_LanzaExcepcion()
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
            pregunta.ConfigurarVerdaderoFalso(true));
    }

    [Fact]
    public void ConfigurarVerdaderoFalso_VersionEnRevision_LanzaExcepcion()
    {
        var pregunta = CrearPreguntaVerdaderoFalso();

        pregunta.EjercicioVersion.EnviarARevision();

        Assert.Throws<InvalidOperationException>(() =>
            pregunta.ConfigurarVerdaderoFalso(true));
    }

    [Fact]
    public void ConfigurarVerdaderoFalso_VersionPublicada_LanzaExcepcion()
    {
        var curso = new Curso(
            "Curso de prueba",
            "Descripción de prueba");

        var ejercicio = new Ejercicio(curso);
        var version = ejercicio.CrearVersion();

        var pregunta = version.CrearPregunta(
            "¿La afirmación es verdadera?",
            TipoPregunta.VerdaderoFalso);

        // La configuración debe existir antes de publicar.
        pregunta.ConfigurarVerdaderoFalso(true);

        version.EnviarARevision();
        ejercicio.PublicarVersion(version.NumeroVersion);

        Assert.Throws<InvalidOperationException>(() =>
            pregunta.ConfigurarVerdaderoFalso(true));
    }
}