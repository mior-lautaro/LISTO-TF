
using System.Net;
using System.Net.Http.Json;
using LISTO.Api.Contracts.Cursos;
using LISTO.Application.DTOs;
using LISTO.Tests.Infrastructure;
using Npgsql;
using Xunit;

namespace LISTO.Tests;

public class CursosControllerIntegracionTests : IClassFixture<ListoApiFactory>
{
    private readonly ListoApiFactory _factory;

    public CursosControllerIntegracionTests(ListoApiFactory factory)
    {
        _factory = factory;
    }

    // ---------------------------------------------------------
    // POST: Crear curso
    // ---------------------------------------------------------

    [Fact]
    public async Task Crear_CursoValido_DevuelveCreatedYPersiste()
    {
        using var client = _factory.CreateClient();
        string? codigo = null;

        try
        {
            var request = new CrearCursoRequest
            {
                Nombre = "Curso HTTP de integración",
                Descripcion = "Prueba de creación mediante API"
            };

            var response = await client.PostAsJsonAsync(
                "/api/cursos", request);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var curso = await response.Content.ReadFromJsonAsync<CursoDto>();

            Assert.NotNull(curso);
            Assert.True(curso.CursoId > 0);
            Assert.StartsWith("CUR-", curso.Codigo);
            Assert.Equal(request.Nombre, curso.Nombre);
            Assert.Equal(request.Descripcion, curso.Descripcion);
            Assert.True(curso.EstaActivo);

            codigo = curso.Codigo;

            // Verificar persistencia mediante una segunda solicitud HTTP.
            var consulta = await client.GetAsync(
                $"/api/cursos/{curso.CursoId}");

            Assert.Equal(HttpStatusCode.OK, consulta.StatusCode);

            var cursoConsultado =
                await consulta.Content.ReadFromJsonAsync<CursoDto>();

            Assert.NotNull(cursoConsultado);
            Assert.Equal(curso.Codigo, cursoConsultado.Codigo);
            Assert.Equal(curso.Nombre, cursoConsultado.Nombre);
        }
        finally
        {
            if (codigo is not null)
                await EliminarCursoAsync(codigo);
        }
    }

    [Fact]
    public async Task Crear_NombreInvalido_DevuelveBadRequest()
    {
        using var client = _factory.CreateClient();

        var request = new CrearCursoRequest
        {
            Nombre = "",
            Descripcion = "Curso inválido"
        };

        var response = await client.PostAsJsonAsync(
            "/api/cursos", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // ---------------------------------------------------------
    // GET: Obtener por identificador
    // ---------------------------------------------------------

    [Fact]
    public async Task ObtenerPorId_CursoExistente_DevuelveCurso()
    {
        using var client = _factory.CreateClient();
        string? codigo = null;

        try
        {
            var cursoCreado = await CrearCursoAsync(client);
            codigo = cursoCreado.Codigo;

            var response = await client.GetAsync(
                $"/api/cursos/{cursoCreado.CursoId}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var curso = await response.Content.ReadFromJsonAsync<CursoDto>();

            Assert.NotNull(curso);
            Assert.Equal(cursoCreado.CursoId, curso.CursoId);
            Assert.Equal(cursoCreado.Codigo, curso.Codigo);
        }
        finally
        {
            if (codigo is not null)
                await EliminarCursoAsync(codigo);
        }
    }

    [Fact]
    public async Task ObtenerPorId_CursoInexistente_DevuelveNotFound()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync(
            "/api/cursos/9223372036854775807");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ---------------------------------------------------------
    // GET: Obtener por código institucional
    // ---------------------------------------------------------

    [Fact]
    public async Task ObtenerPorCodigo_CursoExistente_DevuelveCurso()
    {
        using var client = _factory.CreateClient();
        string? codigo = null;

        try
        {
            var cursoCreado = await CrearCursoAsync(client);
            codigo = cursoCreado.Codigo;

            var response = await client.GetAsync(
                $"/api/cursos/codigo/{codigo}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var curso = await response.Content.ReadFromJsonAsync<CursoDto>();

            Assert.NotNull(curso);
            Assert.Equal(codigo, curso.Codigo);
        }
        finally
        {
            if (codigo is not null)
                await EliminarCursoAsync(codigo);
        }
    }

    [Fact]
    public async Task ObtenerPorCodigo_CodigoInexistente_DevuelveNotFound()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync(
            "/api/cursos/codigo/CUR-999999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ---------------------------------------------------------
    // GET: Listar cursos
    // ---------------------------------------------------------

    [Fact]
    public async Task Listar_FiltrandoPorEstado_DevuelveCursosCorrespondientes()
    {
        using var client = _factory.CreateClient();

        string? codigoActivo = null;
        string? codigoInactivo = null;

        try
        {
            var cursoActivo = await CrearCursoAsync(client);
            codigoActivo = cursoActivo.Codigo;

            var cursoInactivo = await CrearCursoAsync(client);
            codigoInactivo = cursoInactivo.Codigo;

            var desactivar = await client.PatchAsync(
                $"/api/cursos/{cursoInactivo.CursoId}/desactivar",
                null);

            Assert.Equal(HttpStatusCode.NoContent, desactivar.StatusCode);

            var responseActivos = await client.GetAsync(
                "/api/cursos?estaActivo=true");

            var responseInactivos = await client.GetAsync(
                "/api/cursos?estaActivo=false");

            Assert.Equal(HttpStatusCode.OK, responseActivos.StatusCode);
            Assert.Equal(HttpStatusCode.OK, responseInactivos.StatusCode);

            var activos =
                await responseActivos.Content.ReadFromJsonAsync<List<CursoDto>>();

            var inactivos =
                await responseInactivos.Content.ReadFromJsonAsync<List<CursoDto>>();

            Assert.NotNull(activos);
            Assert.NotNull(inactivos);

            Assert.Contains(activos, c => c.Codigo == codigoActivo);
            Assert.DoesNotContain(activos, c => c.Codigo == codigoInactivo);

            Assert.Contains(inactivos, c => c.Codigo == codigoInactivo);
            Assert.DoesNotContain(inactivos, c => c.Codigo == codigoActivo);
        }
        finally
        {
            if (codigoActivo is not null)
                await EliminarCursoAsync(codigoActivo);

            if (codigoInactivo is not null)
                await EliminarCursoAsync(codigoInactivo);
        }
    }

    // ---------------------------------------------------------
    // PUT: Modificar datos
    // ---------------------------------------------------------

    [Fact]
    public async Task Modificar_CursoExistente_ActualizaDatos()
    {
        using var client = _factory.CreateClient();
        string? codigo = null;

        try
        {
            var curso = await CrearCursoAsync(client);
            codigo = curso.Codigo;

            var request = new ModificarCursoRequest
            {
                Nombre = "Curso HTTP modificado",
                Descripcion = "Descripción actualizada"
            };

            var response = await client.PutAsJsonAsync(
                $"/api/cursos/{curso.CursoId}", request);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            var consulta = await client.GetAsync(
                $"/api/cursos/{curso.CursoId}");

            var cursoActualizado =
                await consulta.Content.ReadFromJsonAsync<CursoDto>();

            Assert.NotNull(cursoActualizado);
            Assert.Equal(request.Nombre, cursoActualizado.Nombre);
            Assert.Equal(request.Descripcion, cursoActualizado.Descripcion);
            Assert.Equal(codigo, cursoActualizado.Codigo);
        }
        finally
        {
            if (codigo is not null)
                await EliminarCursoAsync(codigo);
        }
    }

    [Fact]
    public async Task Modificar_CursoInexistente_DevuelveNotFound()
    {
        using var client = _factory.CreateClient();

        var request = new ModificarCursoRequest
        {
            Nombre = "Curso inexistente",
            Descripcion = null
        };

        var response = await client.PutAsJsonAsync(
            "/api/cursos/9223372036854775807", request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Modificar_NombreInvalido_DevuelveBadRequest()
    {
        using var client = _factory.CreateClient();
        string? codigo = null;

        try
        {
            var curso = await CrearCursoAsync(client);
            codigo = curso.Codigo;

            var request = new ModificarCursoRequest
            {
                Nombre = "",
                Descripcion = "Descripción"
            };

            var response = await client.PutAsJsonAsync(
                $"/api/cursos/{curso.CursoId}", request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        finally
        {
            if (codigo is not null)
                await EliminarCursoAsync(codigo);
        }
    }

    // ---------------------------------------------------------
    // PATCH: Activar y desactivar
    // ---------------------------------------------------------

    [Fact]
    public async Task Desactivar_CursoActivo_DesactivaCorrectamente()
    {
        using var client = _factory.CreateClient();
        string? codigo = null;

        try
        {
            var curso = await CrearCursoAsync(client);
            codigo = curso.Codigo;

            var response = await client.PatchAsync(
                $"/api/cursos/{curso.CursoId}/desactivar", null);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            var consulta = await client.GetAsync(
                $"/api/cursos/{curso.CursoId}");

            var cursoActualizado =
                await consulta.Content.ReadFromJsonAsync<CursoDto>();

            Assert.NotNull(cursoActualizado);
            Assert.False(cursoActualizado.EstaActivo);
        }
        finally
        {
            if (codigo is not null)
                await EliminarCursoAsync(codigo);
        }
    }

    [Fact]
    public async Task Activar_CursoInactivo_ActivaCorrectamente()
    {
        using var client = _factory.CreateClient();
        string? codigo = null;

        try
        {
            var curso = await CrearCursoAsync(client);
            codigo = curso.Codigo;

            var desactivar = await client.PatchAsync(
                $"/api/cursos/{curso.CursoId}/desactivar", null);

            Assert.Equal(HttpStatusCode.NoContent, desactivar.StatusCode);

            var activar = await client.PatchAsync(
                $"/api/cursos/{curso.CursoId}/activar", null);

            Assert.Equal(HttpStatusCode.NoContent, activar.StatusCode);

            var consulta = await client.GetAsync(
                $"/api/cursos/{curso.CursoId}");

            var cursoActualizado =
                await consulta.Content.ReadFromJsonAsync<CursoDto>();

            Assert.NotNull(cursoActualizado);
            Assert.True(cursoActualizado.EstaActivo);
        }
        finally
        {
            if (codigo is not null)
                await EliminarCursoAsync(codigo);
        }
    }

    [Fact]
    public async Task Desactivar_CursoInexistente_DevuelveNotFound()
    {
        using var client = _factory.CreateClient();

        var response = await client.PatchAsync(
            "/api/cursos/9223372036854775807/desactivar", null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Activar_CursoInexistente_DevuelveNotFound()
    {
        using var client = _factory.CreateClient();

        var response = await client.PatchAsync(
            "/api/cursos/9223372036854775807/activar", null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ---------------------------------------------------------
    // Métodos auxiliares
    // ---------------------------------------------------------

    private static async Task<CursoDto> CrearCursoAsync(HttpClient client)
    {
        var request = new CrearCursoRequest
        {
            Nombre = "Curso de prueba HTTP",
            Descripcion = "Registro temporal para pruebas de integración"
        };

        var response = await client.PostAsJsonAsync(
            "/api/cursos", request);

        response.EnsureSuccessStatusCode();

        var curso = await response.Content.ReadFromJsonAsync<CursoDto>();

        return curso ?? throw new InvalidOperationException(
            "La API no devolvió los datos del curso creado.");
    }

    private static async Task EliminarCursoAsync(string codigo)
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

        await using var command = connection.CreateCommand();

        command.CommandText =
            "DELETE FROM curso WHERE codigo = @codigo";

        command.Parameters.AddWithValue("codigo", codigo);

        await command.ExecuteNonQueryAsync();
    }
}
