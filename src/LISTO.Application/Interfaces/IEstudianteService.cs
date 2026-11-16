
using LISTO.Application.DTOs;

namespace LISTO.Application.Interfaces;

public interface IEstudianteService
{
    Task<EstudianteDto> RegistrarAsync(
        long usuarioId,
        string dni,
        CancellationToken cancellationToken = default);

    Task<EstudianteDto?> ObtenerPorIdAsync(
        long estudianteId,
        CancellationToken cancellationToken = default);

    Task<EstudianteDto?> ObtenerPorUsuarioIdAsync(
        long usuarioId,
        CancellationToken cancellationToken = default);

    Task<EstudianteDto?> ObtenerPorDNIAsync(
        string dni,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<EstudianteDto>> ListarAsync(
        bool? estaActivo = null,
        CancellationToken cancellationToken = default);

    Task<bool> DesactivarAsync(
        long estudianteId,
        CancellationToken cancellationToken = default);

    Task<bool> ActivarAsync(
        long estudianteId,
        CancellationToken cancellationToken = default);
}
