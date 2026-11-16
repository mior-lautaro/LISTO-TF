using System;
using System.Collections.Generic;
using LISTO.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;

namespace LISTO.Tests.Infrastructure;

public class ListoApiFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString;

    public ListoApiFactory()
    {
        var connectionString = Environment.GetEnvironmentVariable(
            "LISTO_TEST_DB_CONNECTION_STRING");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "La variable LISTO_TEST_DB_CONNECTION_STRING no está configurada.");
        }

        var connectionBuilder =
            new NpgsqlConnectionStringBuilder(connectionString);

        if (!string.Equals(
            connectionBuilder.Database,
            "listo_test_db",
            StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Las pruebas HTTP solo pueden ejecutarse contra listo_test_db.");
        }

        _connectionString = connectionBuilder.ConnectionString;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["ConnectionStrings:ListoDb"] = _connectionString
                });
        });

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions<ListoDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<ListoDbContext>>();
            services.RemoveAll<ListoDbContext>();

            services.AddDbContext<ListoDbContext>(options =>
                options.UseNpgsql(_connectionString));
        });

        builder.UseEnvironment("Development");
    }
}
