using LISTO.Domain.Entities;
using LISTO.Domain.Enums;
using Xunit;

namespace LISTO.Tests;

public class IntentoTests
{
    private static (Estudiante estudiante, Ejercicio ejercicio,
        EjercicioVersion version) CrearContexto()
    {
        var usuario = new Usuario(
            "Juan",
            "Pérez",
            "juan.perez@test.com",
            "hash-de-prueba");

        var estudiante = new Estudiante(usuario, "12345678");

        var curso = new Curso("Curso de prueba", "Descripción");
        var ejercicio = new Ejercicio(curso);
        var version = ejercicio.CrearVersion();

        var pregunta = version.CrearPregunta(
            "¿La afirmación es verdadera?",
            TipoPregunta.VerdaderoFalso);

        pregunta.ConfigurarVerdaderoFalso(true);

        version.EnviarARevision();
        ejercicio.PublicarVersion(version.NumeroVersion);

        return (estudiante, ejercicio, version);
    }

    [Fact]
    public void CrearIntento_ConDatosValidos_CreaIntentoEnCurso()
    {
        var (estudiante, ejercicio, version) = CrearContexto();

        var intento = new Intento(estudiante, ejercicio, version);

        Assert.Equal(EstadoIntento.EnCurso, intento.Estado);
        Assert.Same(estudiante, intento.Estudiante);
        Assert.Same(ejercicio, intento.Ejercicio);
        Assert.Same(version, intento.EjercicioVersion);
        Assert.True(intento.FechaInicio <= DateTime.UtcNow);
        Assert.Null(intento.FechaEnvio);
        Assert.Null(intento.FechaEvaluacion);
    }

    [Fact]
    public void CrearIntento_EstudianteInactivo_LanzaExcepcion()
    {
        var (estudiante, ejercicio, version) = CrearContexto();
        estudiante.Desactivar();

        Assert.Throws<InvalidOperationException>(() =>
            new Intento(estudiante, ejercicio, version));
    }

    [Fact]
    public void CrearIntento_EjercicioRetirado_LanzaExcepcion()
    {
        var (estudiante, ejercicio, version) = CrearContexto();
        ejercicio.Retirar();

        Assert.Throws<InvalidOperationException>(() =>
            new Intento(estudiante, ejercicio, version));
    }

    [Fact]
    public void CrearIntento_VersionNoPublicada_LanzaExcepcion()
    {
        var (estudiante, ejercicio, _) = CrearContexto();

        var otraVersion = ejercicio.CrearVersion();

        Assert.Throws<InvalidOperationException>(() =>
            new Intento(estudiante, ejercicio, otraVersion));
    }

    [Fact]
    public void Enviar_IntentoEnCurso_CambiaEstadoYFecha()
    {
        var (estudiante, ejercicio, version) = CrearContexto();
        var intento = new Intento(estudiante, ejercicio, version);

        intento.Enviar();

        Assert.Equal(EstadoIntento.Enviado, intento.Estado);
        Assert.NotNull(intento.FechaEnvio);
        Assert.Null(intento.FechaEvaluacion);
    }

    [Fact]
    public void Enviar_IntentoYaEnviado_LanzaExcepcion()
    {
        var (estudiante, ejercicio, version) = CrearContexto();
        var intento = new Intento(estudiante, ejercicio, version);

        intento.Enviar();

        Assert.Throws<InvalidOperationException>(() =>
            intento.Enviar());
    }

    [Fact]
    public void RegistrarEvaluacion_IntentoEnviado_CambiaEstadoYFecha()
    {
        var (estudiante, ejercicio, version) = CrearContexto();
        var intento = new Intento(estudiante, ejercicio, version);

        intento.Enviar();
        intento.RegistrarEvaluacion();

        Assert.Equal(EstadoIntento.Evaluado, intento.Estado);
        Assert.NotNull(intento.FechaEnvio);
        Assert.NotNull(intento.FechaEvaluacion);
    }

    [Fact]
    public void RegistrarEvaluacion_IntentoEnCurso_LanzaExcepcion()
    {
        var (estudiante, ejercicio, version) = CrearContexto();
        var intento = new Intento(estudiante, ejercicio, version);

        Assert.Throws<InvalidOperationException>(() =>
            intento.RegistrarEvaluacion());
    }

    [Fact]
    public void RegistrarEvaluacion_IntentoYaEvaluado_LanzaExcepcion()
    {
        var (estudiante, ejercicio, version) = CrearContexto();
        var intento = new Intento(estudiante, ejercicio, version);

        intento.Enviar();
        intento.RegistrarEvaluacion();

        Assert.Throws<InvalidOperationException>(() =>
            intento.RegistrarEvaluacion());
    }
}