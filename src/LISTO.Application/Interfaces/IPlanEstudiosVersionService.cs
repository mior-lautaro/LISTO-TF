using System;
using System.Collections.Generic;
using System.Text;
using LISTO.Application.DTOs;
using LISTO.Domain.Enums;

namespace LISTO.Application.Interfaces;

public interface IPlanEstudiosVersionService
{
    Task<PlanEstudiosVersionDto?> CrearAsync(
        long planEstudiosId,
        DateOnly? fechaVigenciaDesde = null,
        string? observaciones = null,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PlanEstudiosVersionDto>> ListarAsync(
    long planEstudiosId,
    CancellationToken cancellationToken = default);

    Task<PlanEstudiosVersionDto?> ObtenerPorIdAsync(
    long versionId,
    CancellationToken cancellationToken = default);

    Task<PlanCursoDto?> AgregarCursoAsync(
    long planEstudiosId,
    long versionId,
    long cursoId,
    int anioNivel,
    PeriodoRecomendado periodoRecomendado,
    decimal cargaHoraria,
    CancellationToken cancellationToken = default);

    Task<bool> PublicarAsync(
    long planEstudiosId,
    long versionId,
    CancellationToken cancellationToken = default);

    Task<bool> FinalizarVigenciaAsync(
    long planEstudiosId,
    long versionId,
    DateOnly fechaVigenciaHasta,
    CancellationToken cancellationToken = default);
}