using System;
using System.Collections.Generic;
using System.Text;
using LISTO.Application.Enums;
using LISTO.Infrastructure.Generators;

namespace LISTO.Tests.Infrastructure;

public class CodigoInstitucionalDefinitionTests
{
    [Theory]
    [InlineData(
        TipoCodigoInstitucional.Curso,
        "CUR",
        "curso_codigo_seq")]
    [InlineData(
        TipoCodigoInstitucional.PlanEstudios,
        "PLA",
        "plan_estudios_codigo_seq")]
    public void ObtenerConfiguracion_DebeDevolverConfiguracionCorrecta(
        TipoCodigoInstitucional tipo,
        string prefijoEsperado,
        string secuenciaEsperada)
    {
        var resultado =
            CodigoInstitucionalDefinition.ObtenerConfiguracion(tipo);

        Assert.Equal(prefijoEsperado, resultado.Prefijo);
        Assert.Equal(secuenciaEsperada, resultado.Secuencia);
    }

    [Fact]
    public void ObtenerConfiguracion_TipoInvalido_DebeLanzarExcepcion()
    {
        var tipoInvalido = (TipoCodigoInstitucional)999;

        Assert.Throws<ArgumentOutOfRangeException>(
            () => CodigoInstitucionalDefinition.ObtenerConfiguracion(
                tipoInvalido));
    }

    [Theory]
    [InlineData("CUR", 1, "CUR-000001")]
    [InlineData("CUR", 25, "CUR-000025")]
    [InlineData("PLA", 999999, "PLA-999999")]
    [InlineData("PLA", 1000000, "PLA-1000000")]
    public void Formatear_DebeGenerarCodigoCorrecto(
        string prefijo,
        long numero,
        string codigoEsperado)
    {
        var resultado =
            CodigoInstitucionalDefinition.Formatear(prefijo, numero);

        Assert.Equal(codigoEsperado, resultado);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Formatear_PrefijoVacio_DebeLanzarExcepcion(
        string prefijo)
    {
        Assert.Throws<ArgumentException>(
            () => CodigoInstitucionalDefinition.Formatear(prefijo, 1));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Formatear_NumeroNoPositivo_DebeLanzarExcepcion(
        long numero)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => CodigoInstitucionalDefinition.Formatear("CUR", numero));
    }
}
