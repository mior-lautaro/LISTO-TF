
using LISTO.Application.Interfaces;
using LISTO.Application.Services;
using LISTO.Domain.Entities;
using LISTO.Domain.Enums;

namespace LISTO.Tests;

public class EjercicioServiceTests
{
    private readonly RepositorioEjercicioFalso _ejercicioRepository = new();
    private readonly RepositorioCursoFalso _cursoRepository = new();
    private readonly EjercicioService _servicio;

    public EjercicioServiceTests()
    {
        _servicio = new EjercicioService(
            _ejercicioRepository,
            _cursoRepository);
    }

    [Fact]
    public async Task CrearAsync_DebeCrearEjercicioYDevolverDto()
    {
        var curso = CrearCurso();
        _cursoRepository.CursoExistente = curso;

        var resultado = await _servicio.CrearAsync(1);

        Assert.NotNull(_ejercicioRepository.EjercicioGuardado);
        Assert.Equal(1, _ejercicioRepository.CantidadGuardados);

        Assert.Equal(1, resultado.EjercicioId);
        Assert.Equal(curso.CursoId, resultado.CursoId);
        Assert.Equal(EstadoEjercicio.Activo, resultado.Estado);
        Assert.NotEqual(default, resultado.FechaCreacion);
    }

    [Fact]
    public async Task CrearAsync_DebeRechazarCursoInexistente()
    {
        await Assert.ThrowsAsync<ArgumentException>(
            () => _servicio.CrearAsync(999));

        Assert.Null(_ejercicioRepository.EjercicioGuardado);
        Assert.Equal(0, _ejercicioRepository.CantidadGuardados);
    }

    [Fact]
    public async Task CrearAsync_DebeRechazarCursoInactivo()
    {
        var curso = CrearCurso();
        curso.Desactivar();
        _cursoRepository.CursoExistente = curso;

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _servicio.CrearAsync(1));

        Assert.Null(_ejercicioRepository.EjercicioGuardado);
        Assert.Equal(0, _ejercicioRepository.CantidadGuardados);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_DebeDevolverDtoCuandoExiste()
    {
        var ejercicio = CrearEjercicio();
        _ejercicioRepository.EjercicioExistente = ejercicio;

        var resultado = await _servicio.ObtenerPorIdAsync(1);

        Assert.NotNull(resultado);
        Assert.Equal(ejercicio.EjercicioId, resultado.EjercicioId);
        Assert.Equal(ejercicio.CursoId, resultado.CursoId);
        Assert.Equal(ejercicio.Estado, resultado.Estado);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_DebeDevolverNullCuandoNoExiste()
    {
        var resultado = await _servicio.ObtenerPorIdAsync(999);

        Assert.Null(resultado);
    }

    [Fact]
    public async Task ListarAsync_DebeDevolverEjerciciosComoDtos()
    {
        _ejercicioRepository.EjerciciosExistentes.Add(CrearEjercicio());
        _ejercicioRepository.EjerciciosExistentes.Add(CrearEjercicio());

        var resultado = await _servicio.ListarAsync();

        Assert.Equal(2, resultado.Count);
        Assert.All(resultado, ejercicio =>
            Assert.Equal(EstadoEjercicio.Activo, ejercicio.Estado));
    }

    [Fact]
    public async Task ListarAsync_DebeDelegarFiltroPorCurso()
    {
        var curso = CrearCurso();
        _cursoRepository.CursoExistente = curso;

        await _servicio.ListarAsync(curso.CursoId);

        Assert.Equal(curso.CursoId, _ejercicioRepository.UltimoCursoId);
    }

    [Fact]
    public async Task ListarAsync_DebeRechazarCursoInexistente()
    {
        await Assert.ThrowsAsync<ArgumentException>(
            () => _servicio.ListarAsync(999));

        Assert.Equal(0, _ejercicioRepository.CantidadConsultas);
    }

    [Fact]
    public async Task RetirarAsync_DebeRetirarEjercicio()
    {
        var ejercicio = CrearEjercicio();
        _ejercicioRepository.EjercicioExistente = ejercicio;

        var resultado = await _servicio.RetirarAsync(1);

        Assert.True(resultado);
        Assert.Equal(EstadoEjercicio.Retirado, ejercicio.Estado);
        Assert.Equal(1, _ejercicioRepository.CantidadGuardados);
    }

    [Fact]
    public async Task RetirarAsync_DebeSerIdempotente()
    {
        var ejercicio = CrearEjercicio();
        ejercicio.Retirar();
        _ejercicioRepository.EjercicioExistente = ejercicio;

        var resultado = await _servicio.RetirarAsync(1);

        Assert.True(resultado);
        Assert.Equal(EstadoEjercicio.Retirado, ejercicio.Estado);
        Assert.Equal(0, _ejercicioRepository.CantidadGuardados);
    }

    [Fact]
    public async Task RetirarAsync_DebeDevolverFalseCuandoNoExiste()
    {
        var resultado = await _servicio.RetirarAsync(999);

        Assert.False(resultado);
        Assert.Equal(0, _ejercicioRepository.CantidadGuardados);
    }

    private static Curso CrearCurso()
    {
        var curso = new Curso(
            "CUR-000001",
            "Matemática");

        typeof(Curso)
            .GetProperty(nameof(Curso.CursoId))!
            .SetValue(curso, 1L);

        return curso;
    }

    private static Ejercicio CrearEjercicio()
    {
        var ejercicio = new Ejercicio(CrearCurso());

        typeof(Ejercicio)
            .GetProperty(nameof(Ejercicio.EjercicioId))!
            .SetValue(ejercicio, 1L);

        return ejercicio;
    }

    private sealed class RepositorioCursoFalso : ICursoRepository
    {
        public Curso? CursoExistente { get; set; }

        public Task<Curso?> ObtenerPorIdAsync(
            long cursoId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(CursoExistente);
        }

        public Task<Curso?> ObtenerPorCodigoAsync(
            string codigo,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<Curso?>(null);
        }

        public Task<IReadOnlyList<Curso>> ListarAsync(
            bool? estaActivo = null,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<Curso>>(
                CursoExistente is null
                    ? new List<Curso>()
                    : new List<Curso> { CursoExistente });
        }

        public Task AgregarAsync(
            Curso curso,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task<int> GuardarCambiosAsync(
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(0);
        }
    }

    private sealed class RepositorioEjercicioFalso : IEjercicioRepository
    {
        public Ejercicio? EjercicioGuardado { get; private set; }
        public Ejercicio? EjercicioExistente { get; set; }

        public List<Ejercicio> EjerciciosExistentes { get; } = new();

        public int CantidadGuardados { get; private set; }
        public int CantidadConsultas { get; private set; }
        public long? UltimoCursoId { get; private set; }

        public Task<Ejercicio?> ObtenerPorIdAsync(
            long ejercicioId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(EjercicioExistente);
        }

        public Task<Ejercicio?> ObtenerConVersionesPorIdAsync(
    long ejercicioId,
    CancellationToken cancellationToken = default)
        {
            return Task.FromResult(EjercicioExistente);
        }

        public Task<IReadOnlyList<Ejercicio>> ListarAsync(
            long? cursoId = null,
            CancellationToken cancellationToken = default)
        {
            CantidadConsultas++;
            UltimoCursoId = cursoId;

            IEnumerable<Ejercicio> consulta = EjerciciosExistentes;

            if (cursoId.HasValue)
            {
                consulta = consulta.Where(
                    e => e.CursoId == cursoId.Value);
            }

            return Task.FromResult<IReadOnlyList<Ejercicio>>(
                consulta.ToList());
        }

        public Task AgregarAsync(
            Ejercicio ejercicio,
            CancellationToken cancellationToken = default)
        {
            EjercicioGuardado = ejercicio;
            return Task.CompletedTask;
        }

        public Task<int> GuardarCambiosAsync(
            CancellationToken cancellationToken = default)
        {
            CantidadGuardados++;

            if (EjercicioGuardado is not null)
            {
                typeof(Ejercicio)
                    .GetProperty(nameof(Ejercicio.EjercicioId))!
                    .SetValue(EjercicioGuardado, 1L);
            }

            return Task.FromResult(1);
        }
    }
}
