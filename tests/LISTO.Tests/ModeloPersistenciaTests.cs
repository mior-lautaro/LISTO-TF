using LISTO.Domain.Entities;
using LISTO.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LISTO.Tests;

public class ModeloPersistenciaTests
{
    private static ListoDbContext CrearContexto()
    {
        var options = new DbContextOptionsBuilder<ListoDbContext>()
            .UseNpgsql(
                "Host=localhost;Database=listo_model_test;" +
                "Username=test;Password=test")
            .Options;

        return new ListoDbContext(options);
    }

    [Fact]
    public void Modelo_ContieneLasTresEntidadesEsperadas()
    {
        using var context = CrearContexto();

        var modelo = context.Model;

        Assert.NotNull(modelo.FindEntityType(typeof(Usuario)));
        Assert.NotNull(modelo.FindEntityType(typeof(Rol)));
        Assert.NotNull(modelo.FindEntityType(typeof(UsuarioRol)));
    }

    [Fact]
    public void Modelo_ContieneLasTablasEsperadas()
    {
        using var context = CrearContexto();

        Assert.Equal(
            "usuario",
            context.Model.FindEntityType(typeof(Usuario))!.GetTableName());

        Assert.Equal(
            "rol",
            context.Model.FindEntityType(typeof(Rol))!.GetTableName());

        Assert.Equal(
            "usuario_rol",
            context.Model.FindEntityType(typeof(UsuarioRol))!.GetTableName());
    }

    [Fact]
    public void Usuario_TieneClavePrimariaConfigurada()
    {
        using var context = CrearContexto();

        var entidad = context.Model.FindEntityType(typeof(Usuario))!;

        Assert.Equal(
            nameof(Usuario.UsuarioId),
            Assert.Single(entidad.FindPrimaryKey()!.Properties).Name);
    }

    [Fact]
    public void Usuario_TieneIndiceUnicoPorEmail()
    {
        using var context = CrearContexto();

        var entidad = context.Model.FindEntityType(typeof(Usuario))!;

        var indice = Assert.Single(
            entidad.GetIndexes(),
            i => i.Properties.Any(p => p.Name == nameof(Usuario.Email)));

        Assert.True(indice.IsUnique);
        Assert.Equal("ux_usuario_email", indice.GetDatabaseName());
    }

    [Fact]
    public void Rol_TieneColumnaCalculadaENombreNormalizado()
    {
        using var context = CrearContexto();

        var entidad = context.Model.FindEntityType(typeof(Rol))!;

        var propiedad = entidad.FindProperty("NombreNormalizado");

        Assert.NotNull(propiedad);
        Assert.Equal(
            "lower(btrim(nombre))",
            propiedad.GetComputedColumnSql());

        Assert.True(propiedad.GetIsStored());
    }

    [Fact]
    public void Rol_TieneIndiceUnicoPorNombreNormalizado()
    {
        using var context = CrearContexto();

        var entidad = context.Model.FindEntityType(typeof(Rol))!;

        var indice = Assert.Single(
            entidad.GetIndexes(),
            i => i.Properties.Any(
                p => p.Name == "NombreNormalizado"));

        Assert.True(indice.IsUnique);
        Assert.Equal(
            "ux_rol_nombre_normalizado",
            indice.GetDatabaseName());
    }

    [Fact]
    public void UsuarioRol_TieneDosClavesForaneas()
    {
        using var context = CrearContexto();

        var entidad = context.Model.FindEntityType(typeof(UsuarioRol))!;

        Assert.Equal(2, entidad.GetForeignKeys().Count());

        Assert.Contains(
            entidad.GetForeignKeys(),
            fk => fk.PrincipalEntityType.ClrType == typeof(Usuario));

        Assert.Contains(
            entidad.GetForeignKeys(),
            fk => fk.PrincipalEntityType.ClrType == typeof(Rol));
    }

    [Fact]
    public void UsuarioRol_TieneIndiceUnicoFiltradoParaAsignacionesVigentes()
    {
        using var context = CrearContexto();

        var entidad = context.Model.FindEntityType(typeof(UsuarioRol))!;

        var indice = Assert.Single(
            entidad.GetIndexes(),
            i => i.GetDatabaseName() == "ux_usuario_rol_vigente");

        Assert.True(indice.IsUnique);
        Assert.Equal(
            "fecha_revocacion IS NULL",
            indice.GetFilter());
    }

    [Fact]
    public void Modelo_TieneLasRestriccionesCheckEsperadas()
    {
        using var context = CrearContexto();

        var modelo = context
            .GetService<IDesignTimeModel>()
            .Model;

        var usuario = modelo.FindEntityType(typeof(Usuario))!;
        var rol = modelo.FindEntityType(typeof(Rol))!;
        var usuarioRol = modelo.FindEntityType(typeof(UsuarioRol))!;

        Assert.Contains(
            usuario.GetCheckConstraints(),
            c => c.Name == "ck_usuario_email_valido");

        Assert.Contains(
            usuario.GetCheckConstraints(),
            c => c.Name == "ck_usuario_nombre_no_vacio");

        Assert.Contains(
            usuario.GetCheckConstraints(),
            c => c.Name == "ck_usuario_apellido_no_vacio");

        Assert.Contains(
            rol.GetCheckConstraints(),
            c => c.Name == "ck_rol_nombre_no_vacio");

        Assert.Contains(
            usuarioRol.GetCheckConstraints(),
            c => c.Name == "ck_usuario_rol_fechas");
    }

    [Fact]
    public void UsuarioRol_LasRelacionesRestringenLaEliminacion()
    {
        using var context = CrearContexto();

        var entidad = context.Model.FindEntityType(typeof(UsuarioRol))!;

        Assert.All(
            entidad.GetForeignKeys(),
            fk => Assert.Equal(
                DeleteBehavior.Restrict,
                fk.DeleteBehavior));
    }

    [Fact]
    public void UsuarioRol_FechaAsignacionEsObligatoria()
    {
        using var context = CrearContexto();

        var entidad = context.Model.FindEntityType(typeof(UsuarioRol))!;

        var propiedad = entidad.FindProperty(
            nameof(UsuarioRol.FechaAsignacion))!;

        Assert.False(propiedad.IsNullable);
        Assert.Equal(
            "timestamp with time zone",
            propiedad.GetColumnType());
    }

    [Fact]
    public void UsuarioRol_FechaRevocacionEsOpcional()
    {
        using var context = CrearContexto();

        var entidad = context.Model.FindEntityType(typeof(UsuarioRol))!;

        var propiedad = entidad.FindProperty(
            nameof(UsuarioRol.FechaRevocacion))!;

        Assert.True(propiedad.IsNullable);
        Assert.Equal(
            "timestamp with time zone",
            propiedad.GetColumnType());
    }
}