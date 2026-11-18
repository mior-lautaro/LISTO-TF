using LISTO.Domain.Entities;

namespace LISTO.Application.Interfaces;

public interface IIntentoRepository
{
    Task<Intento?> ObtenerPorIdAsync(
        long intentoId,
        CancellationToken cancellationToken = default);

    Task<bool> ExisteEnCursoAsync(
        long estudianteId,
        long ejercicioId,
        CancellationToken cancellationToken = default);

    Task AgregarAsync(
        Intento intento,
        CancellationToken cancellationToken = default);

    Task<int> GuardarCambiosAsync(
        CancellationToken cancellationToken = default);

    Task<Intento?> ObtenerConRespuestaPorIdAsync(
    long intentoId,
    CancellationToken cancellationToken = default);
}