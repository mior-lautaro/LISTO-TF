using System;
using System.Collections.Generic;
using System.Linq;
using LISTO.Application.DTOs;
using LISTO.Application.Enums;
using LISTO.Application.Interfaces;
using LISTO.Application.Services;
using LISTO.Domain.Entities;

namespace LISTO.Tests;

public class CursoServiceTests
{
    private readonly RepositorioFalso _repositorio = new();
    private readonly GeneradorCodigoFalso _generador = new();
    private readonly CursoService _servicio;

    public CursoServiceTests()
    {
        _servicio = new CursoService(_repositorio, _generador);
    }

    [Fact]
    public async Task CrearAsync_DebeCrearCursoYDevolverDto()
    {
        var resultado = await _servicio.CrearAsync(
            " Matemática ",
            " Curso de matemática básica ");

        Assert.Equal(1, resultado.CursoId);
        Assert.Equal("CUR-000001", resultado.Codigo);
        Assert.Equal("Matemática", resultado.Nombre);
        Assert.Equal("Curso de matemática básica", resultado.Descripcion);
        Assert.True(resultado.EstaActivo);

        Assert.NotNull(_repositorio.CursoGuardado);
        Assert.Equal("CUR-000001", _repositorio.CursoGuardado.Codigo);
        Assert.Equal(1, _repositorio.CantidadGuardados);

        Assert.Equal(TipoCodigoInstitucional.Curso, _generador.UltimoTipoSolicitado);
        Assert.Equal(1, _generador.CantidadInvocaciones);
    }

    [Fact]
    public async Task CrearAsync_DebeNormalizarDescripcionVaciaComoNull()
    {
        var resultado = await _servicio.CrearAsync(
            "Matemática",
            "   ");

        Assert.Null(resultado.Descripcion);
        Assert.Null(_repositorio.CursoGuardado!.Descripcion);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CrearAsync_DebeRechazarNombreVacio(string nombre)
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _servicio.CrearAsync(nombre));

        Assert.Equal(0, _generador.CantidadInvocaciones);
        Assert.Equal(0, _repositorio.CantidadGuardados);
    }

    [Fact]
    public async Task CrearAsync_DebeRechazarNombreDemasiadoLargo()
    {
        var nombre = new string('A', 151);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _servicio.CrearAsync(nombre));

        Assert.Equal(0, _generador.CantidadInvocaciones);
        Assert.Equal(0, _repositorio.CantidadGuardados);
    }

    [Fact]
    public async Task CrearAsync_DebeRechazarDescripcionDemasiadoLarga()
    {
        var descripcion = new string('A', 501);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _servicio.CrearAsync("Matemática", descripcion));

        Assert.Equal(0, _generador.CantidadInvocaciones);
        Assert.Equal(0, _repositorio.CantidadGuardados);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_DebeDevolverDtoCuandoExiste()
    {
        var curso = CrearCurso();
        _repositorio.CursoExistente = curso;

        var resultado = await _servicio.ObtenerPorIdAsync(1);

        Assert.NotNull(resultado);
        Assert.Equal(curso.Codigo, resultado.Codigo);
        Assert.Equal(curso.Nombre, resultado.Nombre);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_DebeDevolverNullCuandoNoExiste()
    {
        var resultado = await _servicio.ObtenerPorIdAsync(999);

        Assert.Null(resultado);
    }

    [Fact]
    public async Task ObtenerPorCodigoAsync_DebeConsultarCodigoNormalizado()
    {
        var curso = CrearCurso();
        _repositorio.CursoExistente = curso;

        var resultado = await _servicio.ObtenerPorCodigoAsync(
            " CUR-000001 ");

        Assert.NotNull(resultado);
        Assert.Equal("CUR-000001", resultado.Codigo);
        Assert.Equal("CUR-000001", _repositorio.UltimoCodigoConsultado);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task ObtenerPorCodigoAsync_DebeRechazarCodigoVacio(
        string codigo)
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _servicio.ObtenerPorCodigoAsync(codigo));

        Assert.Equal(0, _repositorio.CantidadConsultasCodigo);
    }

    [Fact]
    public async Task ListarAsync_DebeDevolverCursosComoDtos()
    {
        _repositorio.CursosExistentes.Add(CrearCurso());
        _repositorio.CursosExistentes.Add(
            new Curso("CUR-000002", "Historia"));

        var resultado = await _servicio.ListarAsync();

        Assert.Equal(2, resultado.Count);
        Assert.All(resultado, curso => Assert.IsType<CursoDto>(curso));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ListarAsync_DebeDelegarFiltroDeEstado(bool estaActivo)
    {
        await _servicio.ListarAsync(estaActivo);

        Assert.Equal(estaActivo, _repositorio.UltimoFiltroEstado);
    }

    [Fact]
    public async Task ModificarDatosAsync_DebeModificarDatosSinCambiarCodigo()
    {
        var curso = CrearCurso();
        _repositorio.CursoExistente = curso;

        var resultado = await _servicio.ModificarDatosAsync(
            1,
            "Matemática avanzada",
            "Nueva descripción");

        Assert.True(resultado);
        Assert.Equal("CUR-000001", curso.Codigo);
        Assert.Equal("Matemática avanzada", curso.Nombre);
        Assert.Equal("Nueva descripción", curso.Descripcion);
        Assert.Equal(1, _repositorio.CantidadGuardados);
    }

    [Fact]
    public async Task ModificarDatosAsync_DebeDevolverFalseCuandoNoExiste()
    {
        var resultado = await _servicio.ModificarDatosAsync(
            999,
            "Matemática",
            null);

        Assert.False(resultado);
        Assert.Equal(0, _repositorio.CantidadGuardados);
    }

    [Fact]
    public async Task DesactivarAsync_DebeDesactivarCurso()
    {
        var curso = CrearCurso();
        _repositorio.CursoExistente = curso;

        var resultado = await _servicio.DesactivarAsync(1);

        Assert.True(resultado);
        Assert.False(curso.EstaActivo);
        Assert.Equal(1, _repositorio.CantidadGuardados);
    }

    [Fact]
    public async Task DesactivarAsync_DebeSerIdempotente()
    {
        var curso = CrearCurso();
        curso.Desactivar();
        _repositorio.CursoExistente = curso;

        var resultado = await _servicio.DesactivarAsync(1);

        Assert.True(resultado);
        Assert.Equal(0, _repositorio.CantidadGuardados);
    }

    [Fact]
    public async Task DesactivarAsync_DebeDevolverFalseCuandoNoExiste()
    {
        var resultado = await _servicio.DesactivarAsync(999);

        Assert.False(resultado);
        Assert.Equal(0, _repositorio.CantidadGuardados);
    }

    [Fact]
    public async Task ActivarAsync_DebeActivarCurso()
    {
        var curso = CrearCurso();
        curso.Desactivar();
        _repositorio.CursoExistente = curso;

        var resultado = await _servicio.ActivarAsync(1);

        Assert.True(resultado);
        Assert.True(curso.EstaActivo);
        Assert.Equal(1, _repositorio.CantidadGuardados);
    }

    [Fact]
    public async Task ActivarAsync_DebeSerIdempotente()
    {
        var curso = CrearCurso();
        _repositorio.CursoExistente = curso;

        var resultado = await _servicio.ActivarAsync(1);

        Assert.True(resultado);
        Assert.Equal(0, _repositorio.CantidadGuardados);
    }

    [Fact]
    public async Task ActivarAsync_DebeDevolverFalseCuandoNoExiste()
    {
        var resultado = await _servicio.ActivarAsync(999);

        Assert.False(resultado);
        Assert.Equal(0, _repositorio.CantidadGuardados);
    }

    private static Curso CrearCurso()
    {
        return new Curso(
            "CUR-000001",
            "Matemática",
            "Curso de matemática");
    }

    private sealed class GeneradorCodigoFalso : ICodigoInstitucionalGenerator
    {
        public TipoCodigoInstitucional? UltimoTipoSolicitado { get; private set; }
        public int CantidadInvocaciones { get; private set; }

        public Task<string> GenerarAsync(
            TipoCodigoInstitucional tipo,
            CancellationToken cancellationToken = default)
        {
            UltimoTipoSolicitado = tipo;
            CantidadInvocaciones++;

            return Task.FromResult("CUR-000001");
        }
    }

    private sealed class RepositorioFalso : ICursoRepository
    {
        public Curso? CursoGuardado { get; private set; }
        public Curso? CursoExistente { get; set; }

        public List<Curso> CursosExistentes { get; } = new();

        public int CantidadGuardados { get; private set; }
        public int CantidadConsultasCodigo { get; private set; }

        public string? UltimoCodigoConsultado { get; private set; }
        public bool? UltimoFiltroEstado { get; private set; }

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
            CantidadConsultasCodigo++;
            UltimoCodigoConsultado = codigo;

            return Task.FromResult(CursoExistente);
        }

        public Task<IReadOnlyList<Curso>> ListarAsync(
            bool? estaActivo = null,
            CancellationToken cancellationToken = default)
        {
            UltimoFiltroEstado = estaActivo;

            IEnumerable<Curso> consulta = CursosExistentes;

            if (estaActivo.HasValue)
            {
                consulta = consulta.Where(
                    c => c.EstaActivo == estaActivo.Value);
            }

            return Task.FromResult<IReadOnlyList<Curso>>(
                consulta.ToList());
        }

        public Task AgregarAsync(
            Curso curso,
            CancellationToken cancellationToken = default)
        {
            CursoGuardado = curso;
            return Task.CompletedTask;
        }

        public Task<int> GuardarCambiosAsync(
            CancellationToken cancellationToken = default)
        {
            CantidadGuardados++;

            if (CursoGuardado is not null)
            {
                typeof(Curso)
                    .GetProperty(nameof(Curso.CursoId))!
                    .SetValue(CursoGuardado, 1L);
            }

            return Task.FromResult(1);
        }
    }
}
