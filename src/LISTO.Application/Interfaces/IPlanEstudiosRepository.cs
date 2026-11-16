using System;
using System.Collections.Generic;
using System.Text;
using LISTO.Domain.Entities;

namespace LISTO.Application.Interfaces;

public interface IPlanEstudiosRepository
{
    Task<PlanEstudios?> ObtenerPorIdAsync(
        long planEstudiosId,
        CancellationToken cancellationToken = default);

    Task<PlanEstudios?> ObtenerPorCodigoAsync(
        string codigo,
        CancellationToken cancellationToken = default);

    Task<PlanEstudiosVersion?> CrearVersionAsync(
    long planEstudiosId,
    DateOnly? fechaVigenciaDesde,
    string? observaciones,
    CancellationToken cancellationToken = default);


    Task<IReadOnlyList<PlanEstudios>> ListarAsync(
        bool? estaActivo = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PlanEstudiosVersion>> ListarVersionesAsync(
    long planEstudiosId,
    CancellationToken cancellationToken = default);

    Task<PlanEstudiosVersion?> ObtenerVersionPorIdAsync(
    long versionId,
    CancellationToken cancellationToken = default);

    Task<PlanEstudiosVersion?> ObtenerVersionPorPlanCursoIdAsync(
        long planCursoId,
        CancellationToken cancellationToken = default);

    Task AgregarAsync(
        PlanEstudios planEstudios,
        CancellationToken cancellationToken = default);

    Task<int> GuardarCambiosAsync(
        CancellationToken cancellationToken = default);


}