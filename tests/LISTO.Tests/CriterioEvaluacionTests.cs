
using LISTO.Domain.Entities;
using LISTO.Domain.Enums;
using Xunit;

namespace LISTO.Tests;

public class CriterioEvaluacionTests
{
    private static ConfiguracionRespuestaLibre CrearConfiguracion()
    {
        var curso = new Curso(
            "Curso de prueba",
            "Descripción de prueba");

        var ejercicio = new Ejercicio(curso);
        var version = ejercicio.CrearVersion();

        var pregunta = version.CrearPregunta(
            "Explique el concepto solicitado.",
            TipoPregunta.RespuestaLibre);

        return pregunta.ConfigurarRespuestaLibre(
            "Respuesta modelo.");
    }

    [Fact]
    public void AgregarCriterio_DatosValidos_CreaCriterio()
    {
        var configuracion = CrearConfiguracion();

        var criterio = configuracion.AgregarCriterio(
            "Precisión conceptual",
            60m,
            1);

        Assert.NotNull(criterio);
        Assert.Equal("Precisión conceptual", criterio.Descripcion);
        Assert.Equal(60m, criterio.Ponderacion);
        Assert.Equal(1, criterio.Orden);
        Assert.Equal(configuracion.PreguntaId, criterio.PreguntaId);
        Assert.Same(configuracion, criterio.ConfiguracionRespuestaLibre);
        Assert.Contains(criterio, configuracion.Criterios);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void AgregarCriterio_DescripcionVacia_LanzaExcepcion(
        string descripcion)
    {
        var configuracion = CrearConfiguracion();

        Assert.Throws<ArgumentException>(() =>
            configuracion.AgregarCriterio(descripcion, 50m, 1));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(100.1)]
    public void AgregarCriterio_PonderacionInvalida_LanzaExcepcion(
        decimal ponderacion)
    {
        var configuracion = CrearConfiguracion();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            configuracion.AgregarCriterio(
                "Precisión conceptual",
                ponderacion,
                1));
    }

    [Fact]
    public void AgregarCriterio_OrdenInvalido_LanzaExcepcion()
    {
        var configuracion = CrearConfiguracion();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            configuracion.AgregarCriterio(
                "Precisión conceptual",
                50m,
                0));
    }

    [Fact]
    public void AgregarCriterio_OrdenDuplicado_LanzaExcepcion()
    {
        var configuracion = CrearConfiguracion();

        configuracion.AgregarCriterio("Criterio 1", 50m, 1);

        Assert.Throws<InvalidOperationException>(() =>
            configuracion.AgregarCriterio("Criterio 2", 50m, 1));
    }

    [Fact]
    public void ModificarPonderacion_DatosValidos_ModificaPonderacion()
    {
        var configuracion = CrearConfiguracion();
        var criterio = configuracion.AgregarCriterio(
            "Precisión conceptual",
            50m,
            1);

        criterio.ModificarPonderacion(70m);

        Assert.Equal(70m, criterio.Ponderacion);
    }

    [Fact]
    public void ModificarDescripcion_DatosValidos_ModificaDescripcion()
    {
        var configuracion = CrearConfiguracion();
        var criterio = configuracion.AgregarCriterio(
            "Precisión conceptual",
            50m,
            1);

        criterio.ModificarDescripcion("Comprensión conceptual");

        Assert.Equal("Comprensión conceptual", criterio.Descripcion);
    }

    [Fact]
    public void ModificarOrden_OrdenDuplicado_LanzaExcepcion()
    {
        var configuracion = CrearConfiguracion();

        var criterio1 = configuracion.AgregarCriterio("Criterio 1", 50m, 1);
        var criterio2 = configuracion.AgregarCriterio("Criterio 2", 50m, 2);

        Assert.Throws<InvalidOperationException>(() =>
            criterio2.ModificarOrden(criterio1.Orden));
    }


}
