
using System.Net;
using System.Net.Http.Json;
using LISTO.Api.Contracts.Estudiantes;
using LISTO.Api.Contracts.Usuarios;
using LISTO.Application.DTOs;
using LISTO.Tests.Infrastructure;
using Npgsql;
using Xunit;

namespace LISTO.Tests;

public class EstudiantesControllerIntegracionTests
    : IClassFixture<ListoApiFactory>
{
    private readonly ListoApiFactory _factory;

    public EstudiantesControllerIntegracionTests(ListoApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Registrar_EstudianteValido_DevuelveCreatedYPersiste()
    {
        using var client = _factory.CreateClient();

        long? usuarioId = null;

        try
        {
            // 1. Registrar el usuario asociado.
            var usuarioRequest = new RegistrarUsuarioRequest
            {
                Email = $"estudiante.{Guid.NewGuid():N}@listo.test",
                Nombre = "Estudiante",
                Apellido = "Prueba",
                Contrasena = "ClavePrueba123"
            };

            var usuarioResponse = await client.PostAsJsonAsync(
                "/api/usuarios",
                usuarioRequest);

            Assert.Equal(
                HttpStatusCode.Created,
                usuarioResponse.StatusCode);

            var usuario =
                await usuarioResponse.Content.ReadFromJsonAsync<UsuarioDto>();

            Assert.NotNull(usuario);
            Assert.True(usuario.UsuarioId > 0);

            usuarioId = usuario.UsuarioId;

            // 2. Registrar el estudiante.
            var estudianteRequest = new CrearEstudianteRequest
            {
                UsuarioId = usuario.UsuarioId,
                DNI = Random.Shared.Next(10000000, 99999999).ToString()
            };

            var estudianteResponse = await client.PostAsJsonAsync(
                "/api/estudiantes",
                estudianteRequest);

            Assert.Equal(
                HttpStatusCode.Created,
                estudianteResponse.StatusCode);

            var estudiante =
                await estudianteResponse.Content
                    .ReadFromJsonAsync<EstudianteDto>();

            Assert.NotNull(estudiante);
            Assert.True(estudiante.EstudianteId > 0);
            Assert.Equal(usuario.UsuarioId, estudiante.UsuarioId);
            Assert.Equal(estudianteRequest.DNI, estudiante.DNI);
            Assert.True(estudiante.EstaActivo);

            // 3. Consultar el estudiante mediante otra solicitud HTTP.
            var consulta = await client.GetAsync(
                $"/api/estudiantes/{estudiante.EstudianteId}");

            Assert.Equal(HttpStatusCode.OK, consulta.StatusCode);

            var estudianteConsultado =
                await consulta.Content.ReadFromJsonAsync<EstudianteDto>();

            Assert.NotNull(estudianteConsultado);
            Assert.Equal(
                estudiante.EstudianteId,
                estudianteConsultado.EstudianteId);
            Assert.Equal(
                estudiante.UsuarioId,
                estudianteConsultado.UsuarioId);
            Assert.Equal(
                estudiante.DNI,
                estudianteConsultado.DNI);
        }
        finally
        {
            if (usuarioId is not null)
                await EliminarUsuarioYEstudianteAsync(usuarioId.Value);
        }
    }

    private static async Task EliminarUsuarioYEstudianteAsync(
        long usuarioId)
    {
        var connectionString = Environment.GetEnvironmentVariable(
            "LISTO_TEST_DB_CONNECTION_STRING");

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException(
                "No está configurada la conexión de pruebas.");

        var builder = new NpgsqlConnectionStringBuilder(connectionString);

        if (!string.Equals(
            builder.Database,
            "listo_test_db",
            StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "La limpieza de pruebas solo puede ejecutarse contra listo_test_db.");
        }

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            await using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText =
                    "DELETE FROM estudiante WHERE usuario_id = @usuarioId";

                command.Parameters.AddWithValue("usuarioId", usuarioId);

                await command.ExecuteNonQueryAsync();
            }

            await using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText =
                    "DELETE FROM usuario WHERE usuario_id = @usuarioId";

                command.Parameters.AddWithValue("usuarioId", usuarioId);

                await command.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
