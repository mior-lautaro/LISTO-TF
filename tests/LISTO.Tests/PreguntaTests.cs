using LISTO.Domain.Entities;
using LISTO.Domain.Enums;
using Xunit;

namespace LISTO.Tests;

public class PreguntaTests
{
    private static EjercicioVersion CrearVersionBorrador()
    {
        var curso = new Curso(
            "Curso de prueba",
            "Descripción de prueba");

        var ejercicio = new Ejercicio(curso);

        return ejercicio.CrearVersion();
    }

    [Fact]
    public void CrearPregunta_ConDatosValidos_CreaPreguntaCorrectamente()
    {
        var version = CrearVersionBorrador();

        var pregunta = version.CrearPregunta(
            "¿Cuál es la respuesta correcta?",
            TipoPregunta.OpcionMultiple);

        Assert.NotNull(pregunta);
        Assert.Equal(
            "¿Cuál es la respuesta correcta?",
            pregunta.Enunciado);
        Assert.Equal(
            TipoPregunta.OpcionMultiple,
            pregunta.Tipo);
        Assert.Same(version, pregunta.EjercicioVersion);
        Assert.Same(pregunta, version.Pregunta);
    }

    [Fact]
    public void CrearPregunta_EnunciadoVacio_LanzaExcepcion()
    {
        var version = CrearVersionBorrador();

        Assert.Throws<ArgumentException>(() =>
            version.CrearPregunta(
                " ",
                TipoPregunta.OpcionMultiple));
    }

    [Fact]
    public void CrearPregunta_TipoInvalido_LanzaExcepcion()
    {
        var version = CrearVersionBorrador();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            version.CrearPregunta(
                "Pregunta de prueba",
                (TipoPregunta)99));
    }

    [Fact]
    public void CrearPregunta_DosVecesEnLaMismaVersion_LanzaExcepcion()
    {
        var version = CrearVersionBorrador();

        version.CrearPregunta(
            "Primera pregunta",
            TipoPregunta.OpcionMultiple);

        Assert.Throws<InvalidOperationException>(() =>
            version.CrearPregunta(
                "Segunda pregunta",
                TipoPregunta.VerdaderoFalso));
    }

    [Fact]
    public void CrearPregunta_EnVersionEnRevision_LanzaExcepcion()
    {
        var version = CrearVersionBorrador();

        version.EnviarARevision();

        Assert.Throws<InvalidOperationException>(() =>
            version.CrearPregunta(
                "Pregunta de prueba",
                TipoPregunta.OpcionMultiple));
    }

    [Fact]
    public void CrearPregunta_EnVersionPublicada_LanzaExcepcion()
    {
        var ejercicioPublicable = new Ejercicio(
            new Curso("Curso publicable", "Descripción"));

        var versionAValidar = ejercicioPublicable.CrearVersion();

        var pregunta = versionAValidar.CrearPregunta(
            "¿La afirmación es verdadera?",
            TipoPregunta.VerdaderoFalso);

        pregunta.ConfigurarVerdaderoFalso(true);

        versionAValidar.EnviarARevision();

        ejercicioPublicable.PublicarVersion(
            versionAValidar.NumeroVersion);

        Assert.Throws<InvalidOperationException>(() =>
            versionAValidar.CrearPregunta(
                "Pregunta posterior a publicación",
                TipoPregunta.OpcionMultiple));
    }

    [Fact]
    public void PublicarPreguntaCompletar_SinModoCorreccion_LanzaExcepcion()
    {
        var curso = new Curso("Curso de prueba", "Descripción");
        var ejercicio = new Ejercicio(curso);
        var version = ejercicio.CrearVersion();

        var pregunta = version.CrearPregunta(
            "Complete la oración",
            TipoPregunta.Completar);

        pregunta.AgregarEspacioCompletar("respuesta", 1);

        version.EnviarARevision();

        Assert.Throws<InvalidOperationException>(() =>
            ejercicio.PublicarVersion(version.NumeroVersion));
    }

    [Fact]
    public void PublicarPreguntaCompletar_SinEspacios_LanzaExcepcion()
    {
        var curso = new Curso("Curso de prueba", "Descripción");
        var ejercicio = new Ejercicio(curso);
        var version = ejercicio.CrearVersion();

        var pregunta = version.CrearPregunta(
            "Complete la oración",
            TipoPregunta.Completar);

        pregunta.ConfigurarModoCorreccionCompletar(
            ModoCorreccionCompletar.CoincidenciaNormalizada);

        version.EnviarARevision();

        Assert.Throws<InvalidOperationException>(() =>
            ejercicio.PublicarVersion(version.NumeroVersion));
    }

    [Fact]
    public void PublicarPreguntaCompletar_SinPonderaciones_PublicaCorrectamente()
    {
        var curso = new Curso("Curso de prueba", "Descripción");
        var ejercicio = new Ejercicio(curso);
        var version = ejercicio.CrearVersion();

        var pregunta = version.CrearPregunta(
            "Complete la oración",
            TipoPregunta.Completar);

        pregunta.ConfigurarModoCorreccionCompletar(
            ModoCorreccionCompletar.CoincidenciaNormalizada);

        pregunta.AgregarEspacioCompletar("primera", 1);
        pregunta.AgregarEspacioCompletar("segunda", 2);

        version.EnviarARevision();

        ejercicio.PublicarVersion(version.NumeroVersion);

        Assert.Equal(
            EstadoEjercicioVersion.Publicado,
            version.Estado);
    }

    [Fact]
    public void PublicarPreguntaCompletar_PonderacionesIncompletas_LanzaExcepcion()
    {
        var curso = new Curso("Curso de prueba", "Descripción");
        var ejercicio = new Ejercicio(curso);
        var version = ejercicio.CrearVersion();

        var pregunta = version.CrearPregunta(
            "Complete la oración",
            TipoPregunta.Completar);

        pregunta.ConfigurarModoCorreccionCompletar(
            ModoCorreccionCompletar.CoincidenciaNormalizada);

        pregunta.AgregarEspacioCompletar("primera", 1, 60m);
        pregunta.AgregarEspacioCompletar("segunda", 2);

        version.EnviarARevision();

        Assert.Throws<InvalidOperationException>(() =>
            ejercicio.PublicarVersion(version.NumeroVersion));
    }

    [Fact]
    public void PublicarPreguntaCompletar_PonderacionesNoSuman100_LanzaExcepcion()
    {
        var curso = new Curso("Curso de prueba", "Descripción");
        var ejercicio = new Ejercicio(curso);
        var version = ejercicio.CrearVersion();

        var pregunta = version.CrearPregunta(
            "Complete la oración",
            TipoPregunta.Completar);

        pregunta.ConfigurarModoCorreccionCompletar(
            ModoCorreccionCompletar.CoincidenciaNormalizada);

        pregunta.AgregarEspacioCompletar("primera", 1, 40m);
        pregunta.AgregarEspacioCompletar("segunda", 2, 40m);

        version.EnviarARevision();

        Assert.Throws<InvalidOperationException>(() =>
            ejercicio.PublicarVersion(version.NumeroVersion));
    }

    [Fact]
    public void AgregarEspacioCompletar_EnVersionEnRevision_LanzaExcepcion()
    {
        var version = CrearVersionBorrador();

        var pregunta = version.CrearPregunta(
            "Complete la oración",
            TipoPregunta.Completar);

        pregunta.ConfigurarModoCorreccionCompletar(
            ModoCorreccionCompletar.CoincidenciaNormalizada);

        version.EnviarARevision();

        Assert.Throws<InvalidOperationException>(() =>
            pregunta.AgregarEspacioCompletar("respuesta", 1));
    }

    [Fact]
    public void AgregarVariante_EnVersionEnRevision_LanzaExcepcion()
    {
        var version = CrearVersionBorrador();

        var pregunta = version.CrearPregunta(
            "Complete la oración",
            TipoPregunta.Completar);

        pregunta.ConfigurarModoCorreccionCompletar(
            ModoCorreccionCompletar.CoincidenciaNormalizada);

        var espacio = pregunta.AgregarEspacioCompletar(
            "respuesta esperada",
            1);

        version.EnviarARevision();

        Assert.Throws<InvalidOperationException>(() =>
            espacio.AgregarVariante("otra respuesta"));
    }
}