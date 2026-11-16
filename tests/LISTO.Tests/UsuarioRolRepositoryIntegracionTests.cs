using System;
using System.Collections.Generic;
using System.Text;
using LISTO.Application.Exceptions;
using LISTO.Domain.Entities;
using LISTO.Infrastructure.Data;
using LISTO.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LISTO.Tests;

public class UsuarioRolRepositoryIntegracionTests
{
    private static DbContextOptions<ListoDbContext> CrearOpciones()
    {
        var connectionString = Environment.GetEnvironmentVariable(
            "LISTO_TEST_DB_CONNECTION_STRING");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "La variable de entorno LISTO_TEST_DB_CONNECTION_STRING no está configurada.");
        }

        var connectionBuilder =
            new Npgsql.NpgsqlConnectionStringBuilder(connectionString);

        if (!string.Equals(
            connectionBuilder.Database,
            "listo_test_db",
            StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Las pruebas de integración solo pueden ejecutarse contra listo_test_db.");
        }

        return new DbContextOptionsBuilder<ListoDbContext>()
            .UseNpgsql(connectionBuilder.ConnectionString)
            .Options;
    }

    private static async Task<(
    long UsuarioId,
    long RolId,
    string Email,
    string NombreRol)> CrearUsuarioYRolAsync(
        DbContextOptions<ListoDbContext> opciones)
    {
        var email = $"integracion_{Guid.NewGuid():N}@test.com";
    var nombreRol = $"Rol Integracion {Guid.NewGuid():N}";

    await using var context = new ListoDbContext(opciones);

var usuario = new Usuario(
    email,
    "Usuario",
    "Integracion",
    "hash-prueba");

var rol = new Rol(nombreRol);

context.Usuarios.Add(usuario);
        context.Roles.Add(rol);

        await context.SaveChangesAsync();

        return (
            usuario.UsuarioId,
            rol.RolId,
            email,
            nombreRol);
    }

    private static async Task EliminarDatosAsync(
        DbContextOptions<ListoDbContext> opciones,
        long usuarioId,
        long rolId)
{
    await using var context = new ListoDbContext(opciones);

    var asignaciones = await context.UsuariosRoles
        .Where(ur =>
            ur.UsuarioId == usuarioId &&
            ur.RolId == rolId)
        .ToListAsync();

    context.UsuariosRoles.RemoveRange(asignaciones);

    var usuario = await context.Usuarios
        .SingleOrDefaultAsync(u => u.UsuarioId == usuarioId);

    var rol = await context.Roles
        .SingleOrDefaultAsync(r => r.RolId == rolId);

    if (usuario is not null)
        context.Usuarios.Remove(usuario);

    if (rol is not null)
        context.Roles.Remove(rol);

    await context.SaveChangesAsync();
}

[Fact]
public async Task AgregarAsync_AsignacionValida_PersisteCorrectamente()
{
    var opciones = CrearOpciones();
    var datos = await CrearUsuarioYRolAsync(opciones);
    long usuarioRolId = 0;

    try
    {
        await using (var context = new ListoDbContext(opciones))
        {
            var usuario = await context.Usuarios
                .SingleAsync(u => u.UsuarioId == datos.UsuarioId);

            var rol = await context.Roles
                .SingleAsync(r => r.RolId == datos.RolId);

            var repository = new UsuarioRolRepository(context);
            var asignacion = new UsuarioRol(usuario, rol);

            await repository.AgregarAsync(asignacion);
            await repository.GuardarCambiosAsync();

            usuarioRolId = asignacion.UsuarioRolId;
        }

        await using (var context = new ListoDbContext(opciones))
        {
            var repository = new UsuarioRolRepository(context);

            var obtenida = await repository.ObtenerPorIdAsync(
                usuarioRolId);

            Assert.NotNull(obtenida);
            Assert.Equal(datos.UsuarioId, obtenida.UsuarioId);
            Assert.Equal(datos.RolId, obtenida.RolId);
            Assert.Null(obtenida.FechaRevocacion);
        }
    }
    finally
    {
        await EliminarDatosAsync(
            opciones,
            datos.UsuarioId,
            datos.RolId);
    }
}

[Fact]
public async Task ObtenerAsignacionVigenteAsync_AsignacionExistente_DevuelveAsignacion()
{
    var opciones = CrearOpciones();
    var datos = await CrearUsuarioYRolAsync(opciones);

    try
    {
        await using (var context = new ListoDbContext(opciones))
        {
            var usuario = await context.Usuarios
                .SingleAsync(u => u.UsuarioId == datos.UsuarioId);

            var rol = await context.Roles
                .SingleAsync(r => r.RolId == datos.RolId);

            var repository = new UsuarioRolRepository(context);

            await repository.AgregarAsync(
                new UsuarioRol(usuario, rol));

            await repository.GuardarCambiosAsync();
        }

        await using (var context = new ListoDbContext(opciones))
        {
            var repository = new UsuarioRolRepository(context);

            var asignacion =
                await repository.ObtenerAsignacionVigenteAsync(
                    datos.UsuarioId,
                    datos.RolId);

            Assert.NotNull(asignacion);
            Assert.Equal(datos.UsuarioId, asignacion.UsuarioId);
            Assert.Equal(datos.RolId, asignacion.RolId);
            Assert.Null(asignacion.FechaRevocacion);
        }
    }
    finally
    {
        await EliminarDatosAsync(
            opciones,
            datos.UsuarioId,
            datos.RolId);
    }
}

[Fact]
public async Task ObtenerAsignacionVigenteAsync_SinAsignacion_DevuelveNull()
{
    var opciones = CrearOpciones();
    var datos = await CrearUsuarioYRolAsync(opciones);

    try
    {
        await using var context = new ListoDbContext(opciones);

        var repository = new UsuarioRolRepository(context);

        var asignacion =
            await repository.ObtenerAsignacionVigenteAsync(
                datos.UsuarioId,
                datos.RolId);

        Assert.Null(asignacion);
    }
    finally
    {
        await EliminarDatosAsync(
            opciones,
            datos.UsuarioId,
            datos.RolId);
    }
}

[Fact]
public async Task ListarPorUsuarioAsync_FiltraAsignacionesSegunVigencia()
{
    var opciones = CrearOpciones();
    var datos = await CrearUsuarioYRolAsync(opciones);

    try
    {
        await using (var context = new ListoDbContext(opciones))
        {
            var usuario = await context.Usuarios
                .SingleAsync(u => u.UsuarioId == datos.UsuarioId);

            var rol = await context.Roles
                .SingleAsync(r => r.RolId == datos.RolId);

            var repository = new UsuarioRolRepository(context);

            var primera = new UsuarioRol(usuario, rol);

            await repository.AgregarAsync(primera);
            await repository.GuardarCambiosAsync();

            primera.Revocar();
            await repository.GuardarCambiosAsync();

            var segunda = new UsuarioRol(usuario, rol);

            await repository.AgregarAsync(segunda);
            await repository.GuardarCambiosAsync();
        }

        await using (var context = new ListoDbContext(opciones))
        {
            var repository = new UsuarioRolRepository(context);

            var historial = await repository.ListarPorUsuarioAsync(
                datos.UsuarioId);

            var vigentes = await repository.ListarPorUsuarioAsync(
                datos.UsuarioId,
                true);

            var revocadas = await repository.ListarPorUsuarioAsync(
                datos.UsuarioId,
                false);

            Assert.Equal(2, historial.Count);
            Assert.Single(vigentes);
            Assert.Single(revocadas);

            Assert.Null(vigentes[0].FechaRevocacion);
            Assert.NotNull(revocadas[0].FechaRevocacion);

            Assert.NotNull(vigentes[0].Rol);
            Assert.Equal(datos.NombreRol, vigentes[0].Rol.Nombre);
        }
    }
    finally
    {
        await EliminarDatosAsync(
            opciones,
            datos.UsuarioId,
            datos.RolId);
    }
}

[Fact]
public async Task ObtenerPorIdAsync_AsignacionInexistente_DevuelveNull()
{
    var opciones = CrearOpciones();

    await using var context = new ListoDbContext(opciones);

    var repository = new UsuarioRolRepository(context);

    var asignacion = await repository.ObtenerPorIdAsync(
        long.MaxValue);

    Assert.Null(asignacion);
}

[Fact]
public async Task GuardarCambiosAsync_AsignacionVigenteDuplicada_LanzaExcepcion()
{
    var opciones = CrearOpciones();
    var datos = await CrearUsuarioYRolAsync(opciones);

    try
    {
        // Primer contexto: crea la asignación inicial.
        await using (var context = new ListoDbContext(opciones))
        {
            var usuario = await context.Usuarios
                .SingleAsync(u => u.UsuarioId == datos.UsuarioId);

            var rol = await context.Roles
                .SingleAsync(r => r.RolId == datos.RolId);

            var repository = new UsuarioRolRepository(context);

            await repository.AgregarAsync(
                new UsuarioRol(usuario, rol));

            await repository.GuardarCambiosAsync();
        }

        // Segundo contexto: intenta crear una asignación duplicada.
        await using (var context = new ListoDbContext(opciones))
        {
            var usuario = await context.Usuarios
                .SingleAsync(u => u.UsuarioId == datos.UsuarioId);

            var rol = await context.Roles
                .SingleAsync(r => r.RolId == datos.RolId);

            var repository = new UsuarioRolRepository(context);

            await repository.AgregarAsync(
                new UsuarioRol(usuario, rol));

            var excepcion =
                await Assert.ThrowsAsync<AsignacionRolDuplicadaException>(
                    () => repository.GuardarCambiosAsync());

            Assert.NotNull(excepcion);
        }
    }
    finally
    {
        await EliminarDatosAsync(
            opciones,
            datos.UsuarioId,
            datos.RolId);
    }
}
}