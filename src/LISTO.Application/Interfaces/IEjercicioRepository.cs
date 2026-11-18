using LISTO.Domain.Entities;

namespace LISTO.Application.Interfaces;

public interface IEjercicioRepository
{
    Task<Ejercicio?> ObtenerPorIdAsync(
        long ejercicioId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Ejercicio>> ListarAsync(
        long? cursoId = null,
        CancellationToken cancellationToken = default);

    Task AgregarAsync(
        Ejercicio ejercicio,
        CancellationToken cancellationToken = default);

    Task<int> GuardarCambiosAsync(
        CancellationToken cancellationToken = default);

    Task<Ejercicio?> ObtenerConVersionesPorIdAsync(
    long ejercicioId,
    CancellationToken cancellationToken = default);
}