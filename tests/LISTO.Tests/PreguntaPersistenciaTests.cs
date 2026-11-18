using LISTO.Infrastructure.Data;
using LISTO.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Xunit;
using Microsoft.EntityFrameworkCore.Infrastructure;
namespace LISTO.Tests;

public class PreguntaPersistenciaTests
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
    public void Modelo_ContieneLaEntidadPregunta()
    {
        using var contexto = CrearContexto();

        var entidad = contexto.Model.FindEntityType(
            typeof(Pregunta));

        Assert.NotNull(entidad);
    }

    [Fact]
    public void Pregunta_UtilizaLaTablaEsperada()
    {
        using var contexto = CrearContexto();

        var entidad = contexto.Model.FindEntityType(
            typeof(Pregunta));

        Assert.NotNull(entidad);
        Assert.Equal("pregunta", entidad.GetTableName());
    }

    [Fact]
    public void Pregunta_TieneClavePrimariaConfigurada()
    {
        using var contexto = CrearContexto();

        var entidad = contexto.Model.FindEntityType(
            typeof(Pregunta));

        Assert.NotNull(entidad);

        var clave = entidad.FindPrimaryKey();

        Assert.NotNull(clave);
        Assert.Equal(
            "PreguntaId",
            clave.Properties.Single().Name);
    }

    [Fact]
    public void Pregunta_TieneRelacionUnoAUnoConEjercicioVersion()
    {
        using var contexto = CrearContexto();

        var entidad = contexto.Model.FindEntityType(
            typeof(Pregunta));

        Assert.NotNull(entidad);

        var relacion = entidad.GetForeignKeys()
            .Single(fk =>
                fk.PrincipalEntityType.ClrType ==
                typeof(EjercicioVersion));

        Assert.True(relacion.IsUnique);
        Assert.Equal(
            DeleteBehavior.Restrict,
            relacion.DeleteBehavior);
        Assert.Equal(
            "EjercicioVersionId",
            relacion.Properties.Single().Name);
    }

    [Fact]
    public void Pregunta_TieneIndiceUnicoPorEjercicioVersion()
    {
        using var contexto = CrearContexto();

        var entidad = contexto.Model.FindEntityType(
            typeof(Pregunta));

        Assert.NotNull(entidad);

        var indice = entidad.GetIndexes()
            .Single(i =>
                i.GetDatabaseName() ==
                "ux_pregunta_ejercicio_version");

        Assert.True(indice.IsUnique);
        Assert.Equal(
            "EjercicioVersionId",
            indice.Properties.Single().Name);
    }

    [Fact]
    public void Pregunta_TieneRestriccionCheckParaTipo()
    {
        using var contexto = CrearContexto();

        var modelo = contexto
            .GetService<IDesignTimeModel>()
            .Model;

        var entidad = modelo.FindEntityType(typeof(Pregunta));

        Assert.NotNull(entidad);

        var restricciones = entidad.GetCheckConstraints();

        Assert.Contains(
            restricciones,
            c => c.Name == "ck_pregunta_tipo_valido" &&
                 c.Sql == "tipo IN (1, 2, 3, 4)");
    }
}