using LISTO.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace LISTO.Api.Data;

public class ListoDbContextFactory
    : IDesignTimeDbContextFactory<ListoDbContext>
{
    public ListoDbContext CreateDbContext(string[] args)
    {
        var environment = Environment.GetEnvironmentVariable(
            "ASPNETCORE_ENVIRONMENT") ?? "Development";

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile(
                $"appsettings.{environment}.json",
                optional: true)
            .Build();

        var connectionString = configuration
            .GetConnectionString("ListoDb");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "No se encontró la cadena de conexión ListoDb.");
        }

        var password = Environment.GetEnvironmentVariable(
            "LISTO_DB_PASSWORD");

        // Para generar migraciones no hace falta conectarse
        // a PostgreSQL. La contraseña real será necesaria
        // al ejecutar operaciones que accedan a la base.
        connectionString = connectionString.Replace(
            "${LISTO_DB_PASSWORD}",
            password ?? "__design_time__");

        var optionsBuilder =
            new DbContextOptionsBuilder<ListoDbContext>();

        optionsBuilder.UseNpgsql(connectionString);

        return new ListoDbContext(optionsBuilder.Options);
    }
}