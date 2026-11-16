using System;
using System.Collections.Generic;
using System.Text;
using LISTO.Application.DTOs;
using LISTO.Application.Interfaces;
using LISTO.Domain.Entities;
using LISTO.Domain.Enums;

namespace LISTO.Application.Services;

public class PlanEstudiosVersionService : IPlanEstudiosVersionService
{
    private readonly IPlanEstudiosRepository _planEstudiosRepository;
    private readonly ICursoRepository _cursoRepository;

    public PlanEstudiosVersionService(
    IPlanEstudiosRepository planEstudiosRepository,
    ICursoRepository cursoRepository)
    {
        ArgumentNullException.ThrowIfNull(planEstudiosRepository);
        ArgumentNullException.ThrowIfNull(cursoRepository);

        _planEstudiosRepository = planEstudiosRepository;
        _cursoRepository = cursoRepository;
    }
    public async Task<PlanEstudiosVersionDto?> CrearAsync(
        long planEstudiosId,
        DateOnly? fechaVigenciaDesde = null,
        string? observaciones = null,
        CancellationToken cancellationToken = default)
    {
        if (planEstudiosId <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(planEstudiosId),
                "El identificador del plan de estudios debe ser positivo.");
        }

        var observacionesNormalizadas =
            NormalizarObservaciones(observaciones);

        ValidarDatos(observacionesNormalizadas);

        var version = await _planEstudiosRepository.CrearVersionAsync(
            planEstudiosId,
            fechaVigenciaDesde,
            observacionesNormalizadas,
            cancellationToken);

        return version is null
            ? null
            : MapearADto(version);
    }

    private static void ValidarDatos(string? observaciones)
    {
        if (observaciones?.Length > 1000)
        {
            throw new ArgumentException(
                "Las observaciones no pueden superar los 1000 caracteres.",
                nameof(observaciones));
        }
    }

    private static string? NormalizarObservaciones(
        string? observaciones)
    {
        return string.IsNullOrWhiteSpace(observaciones)
            ? null
            : observaciones.Trim();
    }

    private static PlanEstudiosVersionDto MapearADto(
    PlanEstudiosVersion version)
    {
        return new PlanEstudiosVersionDto
        {
            PlanEstudiosVersionId = version.PlanEstudiosVersionId,
            PlanEstudiosId = version.PlanEstudiosId,
            NumeroVersion = version.NumeroVersion,
            FechaVigenciaDesde = version.FechaVigenciaDesde,
            FechaVigenciaHasta = version.FechaVigenciaHasta,
            Estado = version.Estado,
            Observaciones = version.Observaciones,

            Cursos = version.Cursos
                .Select(pc => new PlanCursoDto
                {
                    PlanCursoId = pc.PlanCursoId,
                    CursoId = pc.CursoId,
                    CodigoCurso = pc.Curso.Codigo,
                    NombreCurso = pc.Curso.Nombre,
                    AnioNivel = pc.AnioNivel,
                    PeriodoRecomendado = pc.PeriodoRecomendado,
                    CargaHoraria = pc.CargaHoraria
                })
                .ToList()
        };
    }

    public async Task<IReadOnlyList<PlanEstudiosVersionDto>> ListarAsync(
    long planEstudiosId,
    CancellationToken cancellationToken = default)
    {
        if (planEstudiosId <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(planEstudiosId),
                "El identificador del plan de estudios debe ser positivo.");
        }

        var versiones = await _planEstudiosRepository.ListarVersionesAsync(
            planEstudiosId,
            cancellationToken);

        return versiones
            .Select(MapearADto)
            .ToList();
    }

    public async Task<PlanEstudiosVersionDto?> ObtenerPorIdAsync(
    long versionId,
    CancellationToken cancellationToken = default)
    {
        if (versionId <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(versionId),
                "El identificador de la versión debe ser positivo.");
        }

        var version = await _planEstudiosRepository.ObtenerVersionPorIdAsync(
            versionId,
            cancellationToken);

        return version is null
            ? null
            : MapearADto(version);
    }

    public async Task<PlanCursoDto?> AgregarCursoAsync(
    long planEstudiosId,
    long versionId,
    long cursoId,
    int anioNivel,
    PeriodoRecomendado periodoRecomendado,
    decimal cargaHoraria,
    CancellationToken cancellationToken = default)
    {
        if (planEstudiosId <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(planEstudiosId),
                "El identificador del plan debe ser positivo.");

        if (versionId <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(versionId),
                "El identificador de la versión debe ser positivo.");

        if (cursoId <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(cursoId),
                "El identificador del curso debe ser positivo.");

        var plan = await _planEstudiosRepository.ObtenerPorIdAsync(
            planEstudiosId,
            cancellationToken);

        if (plan is null)
            return null;

        var version = plan.Versiones.FirstOrDefault(
            v => v.PlanEstudiosVersionId == versionId);

        if (version is null)
            return null;

        var curso = await _cursoRepository.ObtenerPorIdAsync(
            cursoId,
            cancellationToken);

        if (curso is null)
            throw new KeyNotFoundException(
                "El curso indicado no existe.");

        var planCurso = version.AgregarCurso(
            curso,
            anioNivel,
            periodoRecomendado,
            cargaHoraria);

        await _planEstudiosRepository.GuardarCambiosAsync(
            cancellationToken);

        return new PlanCursoDto
        {
            PlanCursoId = planCurso.PlanCursoId,
            CursoId = planCurso.CursoId,
            CodigoCurso = curso.Codigo,
            NombreCurso = curso.Nombre,
            AnioNivel = planCurso.AnioNivel,
            PeriodoRecomendado = planCurso.PeriodoRecomendado,
            CargaHoraria = planCurso.CargaHoraria
        };
    }

    public async Task<bool> PublicarAsync(
    long planEstudiosId,
    long versionId,
    CancellationToken cancellationToken = default)
    {
        if (planEstudiosId <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(planEstudiosId),
                "El identificador del plan debe ser positivo.");

        if (versionId <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(versionId),
                "El identificador de la versión debe ser positivo.");

        var plan = await _planEstudiosRepository.ObtenerPorIdAsync(
            planEstudiosId,
            cancellationToken);

        if (plan is null)
            return false;

        var version = plan.Versiones.FirstOrDefault(
            v => v.PlanEstudiosVersionId == versionId);

        if (version is null)
            return false;

        plan.PublicarVersion(version.NumeroVersion);

        await _planEstudiosRepository.GuardarCambiosAsync(
            cancellationToken);

        return true;
    }

    public async Task<bool> FinalizarVigenciaAsync(
    long planEstudiosId,
    long versionId,
    DateOnly fechaVigenciaHasta,
    CancellationToken cancellationToken = default)
    {
        if (planEstudiosId <= 0)
            throw new ArgumentOutOfRangeException(nameof(planEstudiosId));

        if (versionId <= 0)
            throw new ArgumentOutOfRangeException(nameof(versionId));

        var plan = await _planEstudiosRepository.ObtenerPorIdAsync(
            planEstudiosId,
            cancellationToken);

        if (plan is null)
            return false;

        var version = plan.Versiones.FirstOrDefault(
            v => v.PlanEstudiosVersionId == versionId);

        if (version is null)
            return false;

        if (version.Estado != EstadoPlanEstudiosVersion.Vigente)
            throw new InvalidOperationException(
                "Solo se puede finalizar la vigencia de una versión vigente.");

        plan.FinalizarVersionVigente(fechaVigenciaHasta);

        await _planEstudiosRepository.GuardarCambiosAsync(
            cancellationToken);

        return true;
    }
}