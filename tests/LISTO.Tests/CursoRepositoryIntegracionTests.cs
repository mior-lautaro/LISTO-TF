using System;
using System.Linq;
using System.Threading.Tasks;
using LISTO.Domain.Entities;
using LISTO.Infrastructure.Data;
using LISTO.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LISTO.Tests;

public class CursoRepositoryIntegracionTests
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

    // Genera un código de exactamente 20 caracteres como máximo.
    private static string GenerarCodigo()
        => $"CT{Guid.NewGuid():N}"[..20];

    [Fact]
    public async Task AgregarAsync_CursoValido_PersisteCorrectamente()
    {
        var opciones = CrearOpciones();
        var codigo = GenerarCodigo();
        var nombre = $"Curso Integracion {Guid.NewGuid():N}";
        long cursoId = 0;

        try
        {
            await using (var context = new ListoDbContext(opciones))
            {
                var repository = new CursoRepository(context);
                var curso = new Curso(codigo, nombre, "Descripción de prueba");

                await repository.AgregarAsync(curso);
                await repository.GuardarCambiosAsync();

                cursoId = curso.CursoId;
            }

            await using (var context = new ListoDbContext(opciones))
            {
                var repository = new CursoRepository(context);

                var cursoObtenido =
                    await repository.ObtenerPorIdAsync(cursoId);

                Assert.NotNull(cursoObtenido);
                Assert.Equal(codigo, cursoObtenido.Codigo);
                Assert.Equal(nombre, cursoObtenido.Nombre);
                Assert.Equal("Descripción de prueba", cursoObtenido.Descripcion);
                Assert.True(cursoObtenido.EstaActivo);
            }
        }
        finally
        {
            await EliminarCursoAsync(opciones, codigo);
        }
    }

    [Fact]
    public async Task ObtenerPorCodigoAsync_NormalizaCodigo_EncuentraCurso()
    {
        var opciones = CrearOpciones();
        var codigo = GenerarCodigo();
        var nombre = $"Curso Integracion {Guid.NewGuid():N}";

        try
        {
            await using (var context = new ListoDbContext(opciones))
            {
                var repository = new CursoRepository(context);

                await repository.AgregarAsync(
                    new Curso(codigo, nombre));

                await repository.GuardarCambiosAsync();
            }

            await using (var context = new ListoDbContext(opciones))
            {
                var repository = new CursoRepository(context);

                var cursoObtenido =
                    await repository.ObtenerPorCodigoAsync(
                        $"  {codigo}  ");

                Assert.NotNull(cursoObtenido);
                Assert.Equal(codigo, cursoObtenido.Codigo);
                Assert.Equal(nombre, cursoObtenido.Nombre);
            }
        }
        finally
        {
            await EliminarCursoAsync(opciones, codigo);
        }
    }

    [Fact]
    public async Task ObtenerPorIdAsync_IdInexistente_DevuelveNull()
    {
        var opciones = CrearOpciones();

        await using var context = new ListoDbContext(opciones);
        var repository = new CursoRepository(context);

        var curso = await repository.ObtenerPorIdAsync(
            long.MaxValue);

        Assert.Null(curso);
    }

    [Fact]
    public async Task ListarAsync_FiltraPorEstadoActivo()
    {
        var opciones = CrearOpciones();

        var codigoActivo = GenerarCodigo();
        var codigoInactivo = GenerarCodigo();

        try
        {
            await using (var context = new ListoDbContext(opciones))
            {
                var repository = new CursoRepository(context);

                var cursoActivo = new Curso(
                    codigoActivo,
                    $"Curso Activo {Guid.NewGuid():N}");

                var cursoInactivo = new Curso(
                    codigoInactivo,
                    $"Curso Inactivo {Guid.NewGuid():N}");

                cursoInactivo.Desactivar();

                await repository.AgregarAsync(cursoActivo);
                await repository.AgregarAsync(cursoInactivo);
                await repository.GuardarCambiosAsync();
            }

            await using (var context = new ListoDbContext(opciones))
            {
                var repository = new CursoRepository(context);

                var activos = await repository.ListarAsync(true);
                var inactivos = await repository.ListarAsync(false);

                Assert.Contains(
                    activos,
                    c => c.Codigo == codigoActivo);

                Assert.DoesNotContain(
                    activos,
                    c => c.Codigo == codigoInactivo);

                Assert.Contains(
                    inactivos,
                    c => c.Codigo == codigoInactivo);

                Assert.DoesNotContain(
                    inactivos,
                    c => c.Codigo == codigoActivo);
            }
        }
        finally
        {
            await EliminarCursoAsync(opciones, codigoActivo);
            await EliminarCursoAsync(opciones, codigoInactivo);
        }
    }

    [Fact]
    public async Task GuardarCambiosAsync_ModificacionDeCurso_PersisteCambios()
    {
        var opciones = CrearOpciones();
        var codigo = GenerarCodigo();
        var nombreOriginal = $"Curso Original {Guid.NewGuid():N}";
        var nombreModificado = $"Curso Modificado {Guid.NewGuid():N}";

        try
        {
            await using (var context = new ListoDbContext(opciones))
            {
                var repository = new CursoRepository(context);

                await repository.AgregarAsync(
                    new Curso(codigo, nombreOriginal));

                await repository.GuardarCambiosAsync();
            }

            await using (var context = new ListoDbContext(opciones))
            {
                var repository = new CursoRepository(context);

                var curso = await repository.ObtenerPorCodigoAsync(codigo);

                Assert.NotNull(curso);

                curso.ModificarDatos(
                    nombreModificado,
                    "Descripción modificada");

                await repository.GuardarCambiosAsync();
            }

            await using (var context = new ListoDbContext(opciones))
            {
                var repository = new CursoRepository(context);

                var cursoObtenido =
                    await repository.ObtenerPorCodigoAsync(codigo);

                Assert.NotNull(cursoObtenido);
                Assert.Equal(nombreModificado, cursoObtenido.Nombre);
                Assert.Equal(
                    "Descripción modificada",
                    cursoObtenido.Descripcion);

                // El código institucional debe permanecer inalterado.
                Assert.Equal(codigo, cursoObtenido.Codigo);
            }
        }
        finally
        {
            await EliminarCursoAsync(opciones, codigo);
        }
    }

    [Fact]
    public async Task GuardarCambiosAsync_CodigoDuplicado_LanzaDbUpdateException()
    {
        var opciones = CrearOpciones();
        var codigo = GenerarCodigo();

        try
        {
            // Primer contexto: registra el curso inicial.
            await using (var context = new ListoDbContext(opciones))
            {
                var repository = new CursoRepository(context);

                await repository.AgregarAsync(
                    new Curso(
                        codigo,
                        $"Curso Original {Guid.NewGuid():N}"));

                await repository.GuardarCambiosAsync();
            }

            // Segundo contexto: intenta registrar otro curso
            // con el mismo código institucional.
            await using (var context = new ListoDbContext(opciones))
            {
                var repository = new CursoRepository(context);

                await repository.AgregarAsync(
                    new Curso(
                        codigo,
                        $"Curso Duplicado {Guid.NewGuid():N}"));

                await Assert.ThrowsAsync<DbUpdateException>(
                    () => repository.GuardarCambiosAsync());
            }
        }
        finally
        {
            await EliminarCursoAsync(opciones, codigo);
        }
    }

    private static async Task EliminarCursoAsync(
        DbContextOptions<ListoDbContext> opciones,
        string codigo)
    {
        await using var context = new ListoDbContext(opciones);

        var curso = await context.Cursos
            .SingleOrDefaultAsync(c => c.Codigo == codigo);

        if (curso is not null)
        {
            context.Cursos.Remove(curso);
            await context.SaveChangesAsync();
        }
    }
}
