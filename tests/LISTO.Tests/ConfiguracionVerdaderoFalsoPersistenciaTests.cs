using LISTO.Domain.Entities;
using LISTO.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LISTO.Tests;

public class ConfiguracionVerdaderoFalsoPersistenciaTests
{
    private static ListoDbContext CrearContexto()
    {
        var options = new DbContextOptionsBuilder<ListoDbContext>()
            .UseNpgsql(
                "Host=localhost;Database=listo_model_test;Username=test;Password=test")
            .Options;

        return new ListoDbContext(options);
    }

    [Fact]
    public void Modelo_ContieneConfiguracionVerdaderoFalso()
    {
        using var contexto = CrearContexto();

        var entidad = contexto.Model.FindEntityType(
            typeof(ConfiguracionVerdaderoFalso));

        Assert.NotNull(entidad);
    }

    [Fact]
    public void Configuracion_UtilizaLaTablaEsperada()
    {
        using var contexto = CrearContexto();

        var entidad = contexto.Model.FindEntityType(
            typeof(ConfiguracionVerdaderoFalso));

        Assert.NotNull(entidad);
        Assert.Equal(
            "configuracion_verdadero_falso",
            entidad.GetTableName());
    }

    [Fact]
    public void Configuracion_PreguntaIdEsClavePrimaria()
    {
        using var contexto = CrearContexto();

        var entidad = contexto.Model.FindEntityType(
            typeof(ConfiguracionVerdaderoFalso));

        Assert.NotNull(entidad);

        var clave = entidad.FindPrimaryKey();

        Assert.NotNull(clave);
        Assert.Equal(
            "PreguntaId",
            clave.Properties.Single().Name);
    }

    [Fact]
    public void Configuracion_TieneRelacionUnoAUnoConPregunta()
    {
        using var contexto = CrearContexto();

        var entidad = contexto.Model.FindEntityType(
            typeof(ConfiguracionVerdaderoFalso));

        Assert.NotNull(entidad);

        var relacion = entidad.GetForeignKeys()
            .Single(fk =>
                fk.PrincipalEntityType.ClrType == typeof(Pregunta));

        Assert.True(relacion.IsUnique);
        Assert.Equal(
            DeleteBehavior.Restrict,
            relacion.DeleteBehavior);

        Assert.Equal(
            "PreguntaId",
            relacion.Properties.Single().Name);
    }

    [Fact]
    public void Configuracion_RespuestaCorrectaEsObligatoria()
    {
        using var contexto = CrearContexto();

        var entidad = contexto.Model.FindEntityType(
            typeof(ConfiguracionVerdaderoFalso));

        Assert.NotNull(entidad);

        var propiedad = entidad.FindProperty(
            nameof(ConfiguracionVerdaderoFalso.RespuestaCorrecta));

        Assert.NotNull(propiedad);
        Assert.False(propiedad.IsNullable);
    }
}