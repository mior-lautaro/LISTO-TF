using LISTO.Application.DTOs;

namespace LISTO.Application.Interfaces;

public interface IEjercicioService
{
    Task<EjercicioDto> CrearAsync(
        long cursoId,
        CancellationToken cancellationToken = default);

    Task<EjercicioDto?> ObtenerPorIdAsync(
        long ejercicioId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<EjercicioDto>> ListarAsync(
        long? cursoId = null,
        CancellationToken cancellationToken = default);

    Task<bool> RetirarAsync(
        long ejercicioId,
        CancellationToken cancellationToken = default);
}