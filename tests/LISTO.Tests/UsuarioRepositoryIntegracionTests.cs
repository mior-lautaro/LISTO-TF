using System;
using System.Collections.Generic;
using System.Text;
using LISTO.Application.Exceptions;
using LISTO.Domain.Entities;
using LISTO.Infrastructure.Data;
using LISTO.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LISTO.Tests;

public class UsuarioRepositoryIntegracionTests
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
    public async Task GuardarCambiosAsync_EmailDuplicado_LanzaEmailDuplicadoException()
    {
        var opciones = CrearOpciones();
        var identificador = Guid.NewGuid().ToString("N");
        var email = $"integracion.{identificador}@listo.local";

        try
        {
            // Primer contexto: registra el usuario inicial.
            await using (var context = new ListoDbContext(opciones))
            {
                var repository = new UsuarioRepository(context);

                var usuario = new Usuario(
                    email,
                    "Usuario",
                    "Integracion",
                    "hash-de-prueba");

                await repository.AgregarAsync(usuario);
                await repository.GuardarCambiosAsync();
            }

            // Segundo contexto: intenta registrar el mismo email.
            await using (var context = new ListoDbContext(opciones))
            {
                var repository = new UsuarioRepository(context);

                var usuarioDuplicado = new Usuario(
                    email,
                    "Otro",
                    "Usuario",
                    "otro-hash-de-prueba");

                await repository.AgregarAsync(usuarioDuplicado);

                await Assert.ThrowsAsync<EmailDuplicadoException>(
                    () => repository.GuardarCambiosAsync());
            }
        }
        finally
        {
            // Elimina el usuario de prueba.
            await using var context = new ListoDbContext(opciones);

            var usuarioCreado = await context.Usuarios
                .SingleOrDefaultAsync(u => u.Email == email);

            if (usuarioCreado is not null)
            {
                context.Usuarios.Remove(usuarioCreado);
                await context.SaveChangesAsync();
            }
        }
    }

    [Fact]
    public async Task GuardarCambiosAsync_OtraViolacionUnica_NoLaConvierteEnEmailDuplicadoException()
    {
        var opciones = CrearOpciones();
        var nombreRol = $"Rol Integracion {Guid.NewGuid():N}";

        try
        {
            // Primer contexto: registra el rol inicial.
            await using (var context = new ListoDbContext(opciones))
            {
                var rol = new Rol(nombreRol);

                context.Roles.Add(rol);
                await context.SaveChangesAsync();
            }

            // Segundo contexto: intenta registrar otro rol
            // cuyo nombre normalizado ya existe.
            await using (var context = new ListoDbContext(opciones))
            {
                var repository = new UsuarioRepository(context);

                var rolDuplicado = new Rol(
                    nombreRol.ToUpperInvariant());

                context.Roles.Add(rolDuplicado);

                await Assert.ThrowsAsync<DbUpdateException>(
                    () => repository.GuardarCambiosAsync());
            }
        }
        finally
        {
            // Elimina el rol de prueba.
            await using var context = new ListoDbContext(opciones);

            var rolCreado = await context.Roles
                .SingleOrDefaultAsync(r => r.Nombre == nombreRol);

            if (rolCreado is not null)
            {
                context.Roles.Remove(rolCreado);
                await context.SaveChangesAsync();
            }
        }
    }
}