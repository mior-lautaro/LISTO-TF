using System;
using System.Collections.Generic;
using System.Text;

using LISTO.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace LISTO.Tests.Integration;

public class PostgresConnectionTests
{
    [Fact]
    public async Task Conexion_DebeConectarseALaBaseDePruebas()
    {
        // Obtener la cadena de conexión exclusiva de pruebas.
        var connectionString = Environment.GetEnvironmentVariable(
            "LISTO_TEST_DB_CONNECTION_STRING");

        Assert.False(
            string.IsNullOrWhiteSpace(connectionString),
            "No está configurada la variable LISTO_TEST_DB_CONNECTION_STRING.");

        // Validar el destino antes de intentar conectarse.
        var connection = new NpgsqlConnectionStringBuilder(
            connectionString);

        Assert.Equal("127.0.0.1", connection.Host);
        Assert.Equal(5432, connection.Port);
        Assert.Equal("listo_test_db", connection.Database);
        Assert.Equal("listo_app", connection.Username);

        // Crear el contexto utilizando exclusivamente esa conexión.
        var options = new DbContextOptionsBuilder<ListoDbContext>()
            .UseNpgsql(connection.ConnectionString)
            .Options;

        await using var context = new ListoDbContext(options);

        // Comprobar la conexión sin crear ni modificar objetos.
        try
        {
            await context.Database.OpenConnectionAsync();

            Assert.True(
                context.Database.GetDbConnection().State
                    == System.Data.ConnectionState.Open);
        }
        catch (Exception ex)
        {
            var detalle = ex.ToString();

            throw new Xunit.Sdk.XunitException(
                $"Error al conectarse a listo_test_db:{Environment.NewLine}{detalle}");
        }
        finally
        {
            await context.Database.CloseConnectionAsync();
        }


    }
    [Fact]
    public async Task Consulta_DebeIdentificarLaBaseDePruebas()
    {
        var connectionString = Environment.GetEnvironmentVariable(
            "LISTO_TEST_DB_CONNECTION_STRING");

        Assert.False(
            string.IsNullOrWhiteSpace(connectionString),
            "No está configurada la variable LISTO_TEST_DB_CONNECTION_STRING.");

        var options = new DbContextOptionsBuilder<ListoDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        await using var context = new ListoDbContext(options);

        var nombreBase = await context.Database
            .SqlQueryRaw<string>(
                "SELECT current_database() AS \"Value\"")
            .SingleAsync();

        Assert.Equal("listo_test_db", nombreBase);
    }
}
