using System;
using System.Collections.Generic;
using System.Text;

using System;
using System.Collections.Generic;
using System.Linq;
using LISTO.Application.DTOs;
using LISTO.Application.Enums;
using LISTO.Application.Interfaces;
using LISTO.Domain.Entities;

namespace LISTO.Application.Services;

public class CursoService : ICursoService
{
    private readonly ICursoRepository _cursoRepository;
    private readonly ICodigoInstitucionalGenerator _codigoGenerator;

    public CursoService(
        ICursoRepository cursoRepository,
        ICodigoInstitucionalGenerator codigoGenerator)
    {
        ArgumentNullException.ThrowIfNull(cursoRepository);
        ArgumentNullException.ThrowIfNull(codigoGenerator);

        _cursoRepository = cursoRepository;
        _codigoGenerator = codigoGenerator;
    }

    public async Task<CursoDto> CrearAsync(
        string nombre,
        string? descripcion = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nombre);

        var nombreNormalizado = nombre.Trim();
        var descripcionNormalizada = NormalizarDescripcion(descripcion);

        ValidarDatos(nombreNormalizado, descripcionNormalizada);

        var codigo = await _codigoGenerator.GenerarAsync(
            TipoCodigoInstitucional.Curso,
            cancellationToken);

        var curso = new Curso(
            codigo,
            nombreNormalizado,
            descripcionNormalizada);

        await _cursoRepository.AgregarAsync(
            curso,
            cancellationToken);

        await _cursoRepository.GuardarCambiosAsync(
            cancellationToken);

        return MapearADto(curso);
    }

    public async Task<CursoDto?> ObtenerPorIdAsync(
        long cursoId,
        CancellationToken cancellationToken = default)
    {
        var curso = await _cursoRepository.ObtenerPorIdAsync(
            cursoId,
            cancellationToken);

        return curso is null ? null : MapearADto(curso);
    }

    public async Task<CursoDto?> ObtenerPorCodigoAsync(
        string codigo,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(codigo);

        var curso = await _cursoRepository.ObtenerPorCodigoAsync(
            codigo.Trim(),
            cancellationToken);

        return curso is null ? null : MapearADto(curso);
    }

    public async Task<IReadOnlyList<CursoDto>> ListarAsync(
        bool? estaActivo = null,
        CancellationToken cancellationToken = default)
    {
        var cursos = await _cursoRepository.ListarAsync(
            estaActivo,
            cancellationToken);

        return cursos
            .Select(MapearADto)
            .ToList();
    }

    public async Task<bool> ModificarDatosAsync(
        long cursoId,
        string nombre,
        string? descripcion,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nombre);

        var nombreNormalizado = nombre.Trim();
        var descripcionNormalizada = NormalizarDescripcion(descripcion);

        ValidarDatos(nombreNormalizado, descripcionNormalizada);

        var curso = await _cursoRepository.ObtenerPorIdAsync(
            cursoId,
            cancellationToken);

        if (curso is null)
            return false;

        curso.ModificarDatos(
            nombreNormalizado,
            descripcionNormalizada);

        await _cursoRepository.GuardarCambiosAsync(
            cancellationToken);

        return true;
    }

    public async Task<bool> DesactivarAsync(
        long cursoId,
        CancellationToken cancellationToken = default)
    {
        var curso = await _cursoRepository.ObtenerPorIdAsync(
            cursoId,
            cancellationToken);

        if (curso is null)
            return false;

        if (!curso.EstaActivo)
            return true;

        curso.Desactivar();

        await _cursoRepository.GuardarCambiosAsync(
            cancellationToken);

        return true;
    }

    public async Task<bool> ActivarAsync(
        long cursoId,
        CancellationToken cancellationToken = default)
    {
        var curso = await _cursoRepository.ObtenerPorIdAsync(
            cursoId,
            cancellationToken);

        if (curso is null)
            return false;

        if (curso.EstaActivo)
            return true;

        curso.Activar();

        await _cursoRepository.GuardarCambiosAsync(
            cancellationToken);

        return true;
    }

    private static void ValidarDatos(
        string nombre,
        string? descripcion)
    {
        if (nombre.Length > 150)
        {
            throw new ArgumentException(
                "El nombre del curso no puede superar los 150 caracteres.",
                nameof(nombre));
        }

        if (descripcion?.Length > 500)
        {
            throw new ArgumentException(
                "La descripción del curso no puede superar los 500 caracteres.",
                nameof(descripcion));
        }
    }

    private static string? NormalizarDescripcion(string? descripcion)
    {
        return string.IsNullOrWhiteSpace(descripcion)
            ? null
            : descripcion.Trim();
    }

    private static CursoDto MapearADto(Curso curso)
    {
        return new CursoDto
        {
            CursoId = curso.CursoId,
            Codigo = curso.Codigo,
            Nombre = curso.Nombre,
            Descripcion = curso.Descripcion,
            EstaActivo = curso.EstaActivo
        };
    }
}