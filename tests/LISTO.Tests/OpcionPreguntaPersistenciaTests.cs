using LISTO.Domain.Entities;
using LISTO.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Xunit;

namespace LISTO.Tests;

public class OpcionPreguntaPersistenciaTests
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
    public void Modelo_ContieneLaEntidadOpcionPregunta()
    {
        using var contexto = CrearContexto();

        var entidad = contexto.Model.FindEntityType(
            typeof(OpcionPregunta));

        Assert.NotNull(entidad);
    }

    [Fact]
    public void OpcionPregunta_UtilizaLaTablaEsperada()
    {
        using var contexto = CrearContexto();

        var entidad = contexto.Model.FindEntityType(
            typeof(OpcionPregunta));

        Assert.NotNull(entidad);
        Assert.Equal("opcion_pregunta", entidad.GetTableName());
    }

    [Fact]
    public void OpcionPregunta_TieneRelacionConPregunta()
    {
        using var contexto = CrearContexto();

        var entidad = contexto.Model.FindEntityType(
            typeof(OpcionPregunta));

        Assert.NotNull(entidad);

        var relacion = entidad.GetForeignKeys()
            .Single(fk =>
                fk.PrincipalEntityType.ClrType == typeof(Pregunta));

        Assert.False(relacion.IsUnique);
        Assert.Equal(
            DeleteBehavior.Restrict,
            relacion.DeleteBehavior);
        Assert.Equal(
            "PreguntaId",
            relacion.Properties.Single().Name);
    }

    [Fact]
    public void OpcionPregunta_TieneIndiceUnicoPorPreguntaYOrden()
    {
        using var contexto = CrearContexto();

        var entidad = contexto.Model.FindEntityType(
            typeof(OpcionPregunta));

        Assert.NotNull(entidad);

        var indice = entidad.GetIndexes()
            .Single(i =>
                i.GetDatabaseName() ==
                "ux_opcion_pregunta_pregunta_orden");

        Assert.True(indice.IsUnique);
        Assert.Equal(
            new[] { "PreguntaId", "Orden" },
            indice.Properties.Select(p => p.Name));
    }

    [Fact]
    public void OpcionPregunta_TieneIndiceUnicoFiltradoParaCorrecta()
    {
        using var contexto = CrearContexto();

        var entidad = contexto.Model.FindEntityType(
            typeof(OpcionPregunta));

        Assert.NotNull(entidad);

        var indice = entidad.GetIndexes()
            .Single(i =>
                i.GetDatabaseName() ==
                "ux_opcion_pregunta_correcta");

        Assert.True(indice.IsUnique);
        Assert.Equal(
            "es_correcta = TRUE",
            indice.GetFilter());
        Assert.Equal(
            "PreguntaId",
            indice.Properties.Single().Name);
    }

    [Fact]
    public void OpcionPregunta_TieneRestriccionCheckParaOrden()
    {
        using var contexto = CrearContexto();

        var modelo = contexto
            .GetService<IDesignTimeModel>()
            .Model;

        var entidad = modelo.FindEntityType(
            typeof(OpcionPregunta));

        Assert.NotNull(entidad);

        var restricciones = entidad.GetCheckConstraints();

        Assert.Contains(
            restricciones,
            c => c.Name == "ck_opcion_pregunta_orden_positivo" &&
                 c.Sql == "orden > 0");
    }
}