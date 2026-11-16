
using LISTO.Application.DTOs;
using LISTO.Domain.Enums;

namespace LISTO.Application.Interfaces;

public interface IInscripcionService
{
    Task<InscripcionDto> InscribirAsync(
        long estudianteId,
        long planCursoId,
        CancellationToken cancellationToken = default);

    Task<InscripcionDto?> ObtenerPorIdAsync(
        long inscripcionId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<InscripcionDto>> ListarPorEstudianteAsync(
        long estudianteId,
        EstadoInscripcion? estado = null,
        CancellationToken cancellationToken = default);

    Task<bool> AprobarAsync(
        long inscripcionId,
        CancellationToken cancellationToken = default);

    Task<bool> DesaprobarAsync(
        long inscripcionId,
        CancellationToken cancellationToken = default);

    Task<bool> AbandonarAsync(
        long inscripcionId,
        CancellationToken cancellationToken = default);

    Task<bool> AnularAsync(
        long inscripcionId,
        CancellationToken cancellationToken = default);
}
