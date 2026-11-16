
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using LISTO.Api.Contracts.Cursos;
using LISTO.Api.Contracts.Estudiantes;
using LISTO.Api.Contracts.Inscripciones;
using LISTO.Api.Contracts.PlanesEstudios;
using LISTO.Api.Contracts.Usuarios;
using LISTO.Application.DTOs;
using LISTO.Domain.Enums;
using LISTO.Tests.Infrastructure;
using Npgsql;
using Xunit;

namespace LISTO.Tests;

public class InscripcionesControllerIntegracionTests
    : IClassFixture<ListoApiFactory>
{
    private readonly ListoApiFactory _factory;

    public InscripcionesControllerIntegracionTests(
        ListoApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Inscribir_EnPlanVigente_CreaInscripcionYPersiste()
    {
        using var client = _factory.CreateClient();

        long? usuarioId = null;
        long? cursoId = null;
        long? planId = null;
        long? versionId = null;
        long? estudianteId = null;
        long? inscripcionId = null;

        try
        {
            // 1. Crear curso.
            var cursoResponse = await client.PostAsJsonAsync(
                "/api/cursos",
                new CrearCursoRequest
                {
                    Nombre = $"Curso de prueba {Guid.NewGuid():N}",
                    Descripcion = "Curso para pruebas de integración."
                });

            Assert.Equal(HttpStatusCode.Created, cursoResponse.StatusCode);

            cursoId = await LeerIdAsync(cursoResponse, "CursoId");

            // 2. Crear plan de estudios.
            var planResponse = await client.PostAsJsonAsync(
                "/api/planes-estudios",
                new CrearPlanEstudiosRequest
                {
                    Nombre = $"Plan de prueba {Guid.NewGuid():N}",
                    Descripcion = "Plan para pruebas de integración."
                });

            Assert.Equal(HttpStatusCode.Created, planResponse.StatusCode);

            planId = await LeerIdAsync(planResponse, "PlanEstudiosId");

            // 3. Crear versión del plan.
            var versionResponse = await client.PostAsJsonAsync(
                $"/api/planes-estudios/{planId}/versiones",
                new CrearPlanEstudiosVersionRequest
                {
                    FechaVigenciaDesde =
                        DateOnly.FromDateTime(DateTime.UtcNow),
                    Observaciones = "Versión de prueba."
                });

            Assert.Equal(HttpStatusCode.Created, versionResponse.StatusCode);

            versionId = await LeerIdAsync(
                versionResponse,
                "VersionId",
                "PlanEstudiosVersionId");

            // 4. Incorporar el curso a la versión.
            var planCursoResponse = await client.PostAsJsonAsync(
                $"/api/planes-estudios/{planId}/versiones/{versionId}/cursos",
                new AgregarCursoAPlanEstudiosVersionRequest
                {
                    CursoId = cursoId.Value,
                    AnioNivel = 1,
                    PeriodoRecomendado = PeriodoRecomendado.Anual,
                    CargaHoraria = 96
                });

            Assert.Equal(
                HttpStatusCode.Created,
                planCursoResponse.StatusCode);

            var planCursoId = await LeerIdAsync(
                planCursoResponse,
                "PlanCursoId");

            // 5. Publicar la versión.
            var publicarResponse = await client.PatchAsync(
                $"/api/planes-estudios/{planId}/versiones/{versionId}/publicar",
                null);

            Assert.Equal(
                HttpStatusCode.NoContent,
                publicarResponse.StatusCode);

            // 6. Crear usuario.
            var usuarioResponse = await client.PostAsJsonAsync(
                "/api/usuarios",
                new RegistrarUsuarioRequest
                {
                    Email = $"inscripcion.{Guid.NewGuid():N}@listo.test",
                    Nombre = "Estudiante",
                    Apellido = "Prueba",
                    Contrasena = "ClavePrueba123"
                });

            Assert.Equal(HttpStatusCode.Created, usuarioResponse.StatusCode);

            var usuario =
                await usuarioResponse.Content.ReadFromJsonAsync<UsuarioDto>();

            Assert.NotNull(usuario);
            usuarioId = usuario.UsuarioId;

            // 7. Registrar estudiante.
            var estudianteResponse = await client.PostAsJsonAsync(
                "/api/estudiantes",
                new CrearEstudianteRequest
                {
                    UsuarioId = usuario.UsuarioId,
                    DNI = Random.Shared.Next(10000000, 99999999).ToString()
                });

            Assert.Equal(
                HttpStatusCode.Created,
                estudianteResponse.StatusCode);

            var estudiante =
                await estudianteResponse.Content
                    .ReadFromJsonAsync<EstudianteDto>();

            Assert.NotNull(estudiante);
            estudianteId = estudiante.EstudianteId;

            // 8. Inscribir al estudiante.
            var inscripcionRequest = new CrearInscripcionRequest
            {
                EstudianteId = estudianteId.Value,
                PlanCursoId = planCursoId
            };

            var inscripcionResponse = await client.PostAsJsonAsync(
                "/api/inscripciones",
                inscripcionRequest);

            Assert.Equal(
                HttpStatusCode.Created,
                inscripcionResponse.StatusCode);

            var inscripcion =
                await inscripcionResponse.Content
                    .ReadFromJsonAsync<InscripcionDto>();

            Assert.NotNull(inscripcion);
            inscripcionId = inscripcion.InscripcionId;

            Assert.True(inscripcion.InscripcionId > 0);
            Assert.Equal(estudianteId, inscripcion.EstudianteId);
            Assert.Equal(planCursoId, inscripcion.PlanCursoId);
            Assert.Equal(EstadoInscripcion.Activa, inscripcion.Estado);
            Assert.Null(inscripcion.FechaResolucion);

            // 9. Verificar persistencia mediante GET.
            var consultaResponse = await client.GetAsync(
                $"/api/inscripciones/{inscripcionId}");

            Assert.Equal(HttpStatusCode.OK, consultaResponse.StatusCode);

            var inscripcionConsultada =
                await consultaResponse.Content
                    .ReadFromJsonAsync<InscripcionDto>();

            Assert.NotNull(inscripcionConsultada);
            Assert.Equal(
                inscripcion.InscripcionId,
                inscripcionConsultada.InscripcionId);

            // 10. Intentar una inscripción activa duplicada.
            var duplicadaResponse = await client.PostAsJsonAsync(
                "/api/inscripciones",
                inscripcionRequest);

            Assert.Equal(
                HttpStatusCode.Conflict,
                duplicadaResponse.StatusCode);

            // 11. Listar inscripciones del estudiante.
            var listadoResponse = await client.GetAsync(
                $"/api/inscripciones/estudiante/{estudianteId}");

            Assert.Equal(HttpStatusCode.OK, listadoResponse.StatusCode);

            var listado =
                await listadoResponse.Content
                    .ReadFromJsonAsync<List<InscripcionDto>>();

            Assert.NotNull(listado);
            Assert.Contains(
                listado,
                i => i.InscripcionId == inscripcionId);

            // 12. Aprobar inscripción.
            var aprobarResponse = await client.PatchAsync(
                $"/api/inscripciones/{inscripcionId}/aprobar",
                null);

            Assert.Equal(
                HttpStatusCode.NoContent,
                aprobarResponse.StatusCode);

            // 13. Verificar estado final.
            var consultaFinalResponse = await client.GetAsync(
                $"/api/inscripciones/{inscripcionId}");

            Assert.Equal(
                HttpStatusCode.OK,
                consultaFinalResponse.StatusCode);

            var inscripcionFinal =
                await consultaFinalResponse.Content
                    .ReadFromJsonAsync<InscripcionDto>();

            Assert.NotNull(inscripcionFinal);
            Assert.Equal(
                EstadoInscripcion.Aprobada,
                inscripcionFinal.Estado);

            Assert.NotNull(inscripcionFinal.FechaResolucion);

            // 14. Intentar modificar una inscripción finalizada.
            var reaprobacionResponse = await client.PatchAsync(
                $"/api/inscripciones/{inscripcionId}/aprobar",
                null);

            Assert.Equal(
                HttpStatusCode.Conflict,
                reaprobacionResponse.StatusCode);
        }
        finally
        {
            await LimpiarDatosAsync(
                inscripcionId,
                estudianteId,
                usuarioId,
                versionId,
                planId,
                cursoId);
        }
    }

    [Fact]
    public async Task Inscribir_EnVersionBorrador_DevuelveConflict()
    {
        using var client = _factory.CreateClient();

        long? usuarioId = null;
        long? cursoId = null;
        long? planId = null;
        long? versionId = null;
        long? estudianteId = null;

        try
        {
            // 1. Crear curso.
            var cursoResponse = await client.PostAsJsonAsync(
                "/api/cursos",
                new CrearCursoRequest
                {
                    Nombre = $"Curso borrador {Guid.NewGuid():N}",
                    Descripcion = "Curso para prueba de versión borrador."
                });

            Assert.Equal(HttpStatusCode.Created, cursoResponse.StatusCode);

            cursoId = await LeerIdAsync(cursoResponse, "CursoId");

            // 2. Crear plan.
            var planResponse = await client.PostAsJsonAsync(
                "/api/planes-estudios",
                new CrearPlanEstudiosRequest
                {
                    Nombre = $"Plan borrador {Guid.NewGuid():N}"
                });

            Assert.Equal(HttpStatusCode.Created, planResponse.StatusCode);

            planId = await LeerIdAsync(planResponse, "PlanEstudiosId");

            // 3. Crear versión sin publicarla.
            var versionResponse = await client.PostAsJsonAsync(
                $"/api/planes-estudios/{planId}/versiones",
                new CrearPlanEstudiosVersionRequest
                {
                    FechaVigenciaDesde =
                        DateOnly.FromDateTime(DateTime.UtcNow),
                    Observaciones = "Versión deliberadamente en borrador."
                });

            Assert.Equal(
                HttpStatusCode.Created,
                versionResponse.StatusCode);

            versionId = await LeerIdAsync(
                versionResponse,
                "VersionId",
                "PlanEstudiosVersionId");

            // 4. Incorporar curso.
            var planCursoResponse = await client.PostAsJsonAsync(
                $"/api/planes-estudios/{planId}/versiones/{versionId}/cursos",
                new AgregarCursoAPlanEstudiosVersionRequest
                {
                    CursoId = cursoId.Value,
                    AnioNivel = 1,
                    PeriodoRecomendado = PeriodoRecomendado.Anual,
                    CargaHoraria = 96
                });

            Assert.Equal(
                HttpStatusCode.Created,
                planCursoResponse.StatusCode);

            var planCursoId = await LeerIdAsync(
                planCursoResponse,
                "PlanCursoId");

            // 5. Crear usuario.
            var usuarioResponse = await client.PostAsJsonAsync(
                "/api/usuarios",
                new RegistrarUsuarioRequest
                {
                    Email = $"borrador.{Guid.NewGuid():N}@listo.test",
                    Nombre = "Estudiante",
                    Apellido = "Borrador",
                    Contrasena = "ClavePrueba123"
                });

            Assert.Equal(HttpStatusCode.Created, usuarioResponse.StatusCode);

            var usuario =
                await usuarioResponse.Content.ReadFromJsonAsync<UsuarioDto>();

            Assert.NotNull(usuario);
            usuarioId = usuario.UsuarioId;

            // 6. Crear estudiante.
            var estudianteResponse = await client.PostAsJsonAsync(
                "/api/estudiantes",
                new CrearEstudianteRequest
                {
                    UsuarioId = usuario.UsuarioId,
                    DNI = Random.Shared.Next(10000000, 99999999).ToString()
                });

            Assert.Equal(
                HttpStatusCode.Created,
                estudianteResponse.StatusCode);

            var estudiante =
                await estudianteResponse.Content
                    .ReadFromJsonAsync<EstudianteDto>();

            Assert.NotNull(estudiante);
            estudianteId = estudiante.EstudianteId;

            // 7. Intentar inscribir en una versión no vigente.
            var inscripcionResponse = await client.PostAsJsonAsync(
                "/api/inscripciones",
                new CrearInscripcionRequest
                {
                    EstudianteId = estudianteId.Value,
                    PlanCursoId = planCursoId
                });

            Assert.Equal(
                HttpStatusCode.Conflict,
                inscripcionResponse.StatusCode);
        }
        finally
        {
            await LimpiarDatosAsync(
                null,
                estudianteId,
                usuarioId,
                versionId,
                planId,
                cursoId);
        }
    }

    private static async Task<long> LeerIdAsync(
        HttpResponseMessage response,
        params string[] nombres)
    {
        using var document = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync());

        foreach (var property in document.RootElement.EnumerateObject())
        {
            if (nombres.Any(nombre =>
                string.Equals(
                    nombre,
                    property.Name,
                    StringComparison.OrdinalIgnoreCase)))
            {
                return property.Value.GetInt64();
            }
        }

        throw new InvalidOperationException(
            $"No se encontró ninguna propiedad de identificador: " +
            $"{string.Join(", ", nombres)}.");
    }

    private static async Task LimpiarDatosAsync(
        long? inscripcionId,
        long? estudianteId,
        long? usuarioId,
        long? versionId,
        long? planId,
        long? cursoId)
    {
        var connectionString = Environment.GetEnvironmentVariable(
            "LISTO_TEST_DB_CONNECTION_STRING");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "No está configurada la conexión de pruebas.");
        }

        var builder = new NpgsqlConnectionStringBuilder(connectionString);

        if (!string.Equals(
            builder.Database,
            "listo_test_db",
            StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "La limpieza de pruebas solo puede ejecutarse contra listo_test_db.");
        }

        await using var connection =
            new NpgsqlConnection(connectionString);

        await connection.OpenAsync();

        await using var transaction =
            await connection.BeginTransactionAsync();

        try
        {
            if (inscripcionId.HasValue)
            {
                await EjecutarDeleteAsync(
                    connection,
                    transaction,
                    "DELETE FROM inscripcion WHERE inscripcion_id = @id",
                    inscripcionId.Value);
            }

            if (estudianteId.HasValue)
            {
                await EjecutarDeleteAsync(
                    connection,
                    transaction,
                    "DELETE FROM inscripcion WHERE estudiante_id = @id",
                    estudianteId.Value);

                await EjecutarDeleteAsync(
                    connection,
                    transaction,
                    "DELETE FROM estudiante WHERE estudiante_id = @id",
                    estudianteId.Value);
            }

            if (usuarioId.HasValue)
            {
                await EjecutarDeleteAsync(
                    connection,
                    transaction,
                    "DELETE FROM usuario_rol WHERE usuario_id = @id",
                    usuarioId.Value);

                await EjecutarDeleteAsync(
                    connection,
                    transaction,
                    "DELETE FROM usuario WHERE usuario_id = @id",
                    usuarioId.Value);
            }

            if (versionId.HasValue)
            {
                await EjecutarDeleteAsync(
                    connection,
                    transaction,
                    "DELETE FROM plan_curso WHERE plan_estudios_version_id = @id",
                    versionId.Value);

                await EjecutarDeleteAsync(
                    connection,
                    transaction,
                    "DELETE FROM plan_estudios_version WHERE plan_estudios_version_id = @id",
                    versionId.Value);
            }

            if (planId.HasValue)
            {
                await EjecutarDeleteAsync(
                    connection,
                    transaction,
                    "DELETE FROM plan_estudios WHERE plan_estudios_id = @id",
                    planId.Value);
            }

            if (cursoId.HasValue)
            {
                await EjecutarDeleteAsync(
                    connection,
                    transaction,
                    "DELETE FROM curso WHERE curso_id = @id",
                    cursoId.Value);
            }

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private static async Task EjecutarDeleteAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction transaction,
        string sql,
        long id)
    {
        await using var command = connection.CreateCommand();

        command.Transaction = transaction;
        command.CommandText = sql;
        command.Parameters.AddWithValue("id", id);

        await command.ExecuteNonQueryAsync();
    }
}
