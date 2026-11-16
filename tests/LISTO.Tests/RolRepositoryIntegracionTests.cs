using System;
using System.Collections.Generic;
using System.Text;
using LISTO.Domain.Entities;
using LISTO.Infrastructure.Data;
using LISTO.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using LISTO.Application.Exceptions;

namespace LISTO.Tests;

public class RolRepositoryIntegracionTests
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

    [Fact]
    public async Task AgregarAsync_RolValido_PersisteCorrectamente()
    {
        var opciones = CrearOpciones();
        var nombre = $"Rol Integracion {Guid.NewGuid():N}";
        long rolId = 0;

        try
        {
            await using (var context = new ListoDbContext(opciones))
            {
                var repository = new RolRepository(context);
                var rol = new Rol(nombre);

                await repository.AgregarAsync(rol);
                await repository.GuardarCambiosAsync();

                rolId = rol.RolId;
            }

            await using (var context = new ListoDbContext(opciones))
            {
                var repository = new RolRepository(context);

                var rolObtenido =
                    await repository.ObtenerPorIdAsync(rolId);

                Assert.NotNull(rolObtenido);
                Assert.Equal(nombre, rolObtenido.Nombre);
                Assert.True(rolObtenido.EstaActivo);
            }
        }
        finally
        {
            await EliminarRolAsync(opciones, nombre);
        }
    }

    [Fact]
    public async Task ObtenerPorNombreAsync_NormalizaNombre_EncuentraRol()
    {
        var opciones = CrearOpciones();
        var nombre = $"Rol Integracion {Guid.NewGuid():N}";

        try
        {
            await using (var context = new ListoDbContext(opciones))
            {
                var repository = new RolRepository(context);

                await repository.AgregarAsync(new Rol(nombre));
                await repository.GuardarCambiosAsync();
            }

            await using (var context = new ListoDbContext(opciones))
            {
                var repository = new RolRepository(context);

                var rolObtenido = await repository.ObtenerPorNombreAsync(
                    $"  {nombre.ToUpperInvariant()}  ");

                Assert.NotNull(rolObtenido);
                Assert.Equal(nombre, rolObtenido.Nombre);
            }
        }
        finally
        {
            await EliminarRolAsync(opciones, nombre);
        }
    }

    [Fact]
    public async Task ExisteNombreAsync_NombreRegistrado_DevuelveTrue()
    {
        var opciones = CrearOpciones();
        var nombre = $"Rol Integracion {Guid.NewGuid():N}";

        try
        {
            await using (var context = new ListoDbContext(opciones))
            {
                var repository = new RolRepository(context);

                await repository.AgregarAsync(new Rol(nombre));
                await repository.GuardarCambiosAsync();
            }

            await using (var context = new ListoDbContext(opciones))
            {
                var repository = new RolRepository(context);

                var existe = await repository.ExisteNombreAsync(
                    nombre.ToUpperInvariant());

                Assert.True(existe);
            }
        }
        finally
        {
            await EliminarRolAsync(opciones, nombre);
        }
    }

    [Fact]
    public async Task ExisteNombreAsync_NombreInexistente_DevuelveFalse()
    {
        var opciones = CrearOpciones();
        var nombre = $"Rol Inexistente {Guid.NewGuid():N}";

        await using var context = new ListoDbContext(opciones);
        var repository = new RolRepository(context);

        var existe = await repository.ExisteNombreAsync(nombre);

        Assert.False(existe);
    }

    [Fact]
    public async Task ListarAsync_FiltraPorEstadoActivo()
    {
        var opciones = CrearOpciones();
        var nombreActivo = $"Rol Activo {Guid.NewGuid():N}";
        var nombreInactivo = $"Rol Inactivo {Guid.NewGuid():N}";

        try
        {
            await using (var context = new ListoDbContext(opciones))
            {
                var repository = new RolRepository(context);

                var rolActivo = new Rol(nombreActivo);
                var rolInactivo = new Rol(nombreInactivo);

                rolInactivo.Desactivar();

                await repository.AgregarAsync(rolActivo);
                await repository.AgregarAsync(rolInactivo);
                await repository.GuardarCambiosAsync();
            }

            await using (var context = new ListoDbContext(opciones))
            {
                var repository = new RolRepository(context);

                var activos = await repository.ListarAsync(true);
                var inactivos = await repository.ListarAsync(false);

                Assert.Contains(
                    activos,
                    r => r.Nombre == nombreActivo);

                Assert.DoesNotContain(
                    activos,
                    r => r.Nombre == nombreInactivo);

                Assert.Contains(
                    inactivos,
                    r => r.Nombre == nombreInactivo);

                Assert.DoesNotContain(
                    inactivos,
                    r => r.Nombre == nombreActivo);
            }
        }
        finally
        {
            await EliminarRolAsync(opciones, nombreActivo);
            await EliminarRolAsync(opciones, nombreInactivo);
        }
    }

    [Fact]
    public async Task GuardarCambiosAsync_NombreDuplicado_LanzaNombreRolDuplicadoException()
    {
        var opciones = CrearOpciones();
        var nombre = $"Rol Duplicado {Guid.NewGuid():N}";

        try
        {
            // Primer contexto: registra el rol inicial.
            await using (var context = new ListoDbContext(opciones))
            {
                var repository = new RolRepository(context);

                await repository.AgregarAsync(new Rol(nombre));
                await repository.GuardarCambiosAsync();
            }

            // Segundo contexto: intenta registrar otro rol
            // con el mismo nombre normalizado.
            await using (var context = new ListoDbContext(opciones))
            {
                var repository = new RolRepository(context);

                var rolDuplicado = new Rol(
                    $"  {nombre.ToUpperInvariant()}  ");

                await repository.AgregarAsync(rolDuplicado);

                var excepcion = await Assert.ThrowsAsync<NombreRolDuplicadoException>(
    () => repository.GuardarCambiosAsync());

                Assert.NotNull(excepcion);
            }
        }
        finally
        {
            await EliminarRolAsync(opciones, nombre);
        }
    }

    private static async Task EliminarRolAsync(
        DbContextOptions<ListoDbContext> opciones,
        string nombre)
    {
        await using var context = new ListoDbContext(opciones);

        var rol = await context.Roles
            .SingleOrDefaultAsync(r => r.Nombre == nombre);

        if (rol is not null)
        {
            context.Roles.Remove(rol);
            await context.SaveChangesAsync();
        }
    }
}