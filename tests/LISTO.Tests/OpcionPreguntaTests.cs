using LISTO.Domain.Entities;
using LISTO.Domain.Enums;
using Xunit;

namespace LISTO.Tests;

public class OpcionPreguntaTests
{
    private static Pregunta CrearPreguntaMultiple()
    {
        var curso = new Curso(
            "Curso de prueba",
            "Descripción de prueba");

        var ejercicio = new Ejercicio(curso);
        var version = ejercicio.CrearVersion();

        return version.CrearPregunta(
            "¿Cuál es la respuesta correcta?",
            TipoPregunta.OpcionMultiple);
    }

    [Fact]
    public void CrearOpcion_DatosValidos_CreaOpcionCorrectamente()
    {
        var pregunta = CrearPreguntaMultiple();

        var opcion = pregunta.CrearOpcion(
            "Respuesta A",
            1,
            true);

        Assert.NotNull(opcion);
        Assert.Equal("Respuesta A", opcion.Texto);
        Assert.Equal(1, opcion.Orden);
        Assert.True(opcion.EsCorrecta);
        Assert.Same(pregunta, opcion.Pregunta);
        Assert.Contains(opcion, pregunta.Opciones);
    }

    [Fact]
    public void CrearOpcion_TextoVacio_LanzaExcepcion()
    {
        var pregunta = CrearPreguntaMultiple();

        Assert.Throws<ArgumentException>(() =>
            pregunta.CrearOpcion(" ", 1, false));
    }

    [Fact]
    public void CrearOpcion_OrdenCeroOLimiteInferior_LanzaExcepcion()
    {
        var pregunta = CrearPreguntaMultiple();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            pregunta.CrearOpcion("Respuesta A", 0, false));
    }

    [Fact]
    public void CrearOpcion_OrdenNegativo_LanzaExcepcion()
    {
        var pregunta = CrearPreguntaMultiple();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            pregunta.CrearOpcion("Respuesta A", -1, false));
    }

    [Fact]
    public void CrearOpcion_OrdenDuplicado_LanzaExcepcion()
    {
        var pregunta = CrearPreguntaMultiple();

        pregunta.CrearOpcion("Respuesta A", 1, false);

        Assert.Throws<InvalidOperationException>(() =>
            pregunta.CrearOpcion("Respuesta B", 1, true));
    }

    [Fact]
    public void CrearOpcion_SegundaOpcionCorrecta_LanzaExcepcion()
    {
        var pregunta = CrearPreguntaMultiple();

        pregunta.CrearOpcion("Respuesta A", 1, true);

        Assert.Throws<InvalidOperationException>(() =>
            pregunta.CrearOpcion("Respuesta B", 2, true));
    }

    [Fact]
    public void CrearOpcion_PreguntaDeOtroTipo_LanzaExcepcion()
    {
        var curso = new Curso(
            "Curso de prueba",
            "Descripción de prueba");

        var ejercicio = new Ejercicio(curso);
        var version = ejercicio.CrearVersion();

        var pregunta = version.CrearPregunta(
            "¿Es verdadero o falso?",
            TipoPregunta.VerdaderoFalso);

        Assert.Throws<InvalidOperationException>(() =>
            pregunta.CrearOpcion("Verdadero", 1, true));
    }

    [Fact]
    public void CrearOpcion_VersionEnRevision_LanzaExcepcion()
    {
        var pregunta = CrearPreguntaMultiple();

        pregunta.EjercicioVersion.EnviarARevision();

        Assert.Throws<InvalidOperationException>(() =>
            pregunta.CrearOpcion("Respuesta A", 1, true));
    }
}