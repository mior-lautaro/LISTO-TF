using LISTO.Application.DTOs;

namespace LISTO.Application.Interfaces;

public interface IPlanEstudiosService
{
    Task<PlanEstudiosDto> CrearAsync(
        string nombre,
        string? descripcion = null,
        CancellationToken cancellationToken = default);

    Task<PlanEstudiosDto?> ObtenerPorIdAsync(
        long planEstudiosId,
        CancellationToken cancellationToken = default);

    Task<PlanEstudiosDto?> ObtenerPorCodigoAsync(
        string codigo,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PlanEstudiosDto>> ListarAsync(
        bool? estaActivo = null,
        CancellationToken cancellationToken = default);

    Task<bool> ModificarDatosAsync(
        long planEstudiosId,
        string nombre,
        string? descripcion,
        CancellationToken cancellationToken = default);

    Task<bool> DesactivarAsync(
        long planEstudiosId,
        CancellationToken cancellationToken = default);

    Task<bool> ActivarAsync(
        long planEstudiosId,
        CancellationToken cancellationToken = default);
}