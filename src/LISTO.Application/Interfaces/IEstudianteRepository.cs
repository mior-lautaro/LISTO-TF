
using LISTO.Domain.Entities;

namespace LISTO.Application.Interfaces;

public interface IEstudianteRepository
{
    Task<Estudiante?> ObtenerPorIdAsync(
        long estudianteId,
        CancellationToken cancellationToken = default);

    Task<Estudiante?> ObtenerPorUsuarioIdAsync(
        long usuarioId,
        CancellationToken cancellationToken = default);

    Task<Estudiante?> ObtenerPorDNIAsync(
        string dni,
        CancellationToken cancellationToken = default);

    Task<bool> ExisteDNIAsync(
        string dni,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Estudiante>> ListarAsync(
        bool? estaActivo = null,
        CancellationToken cancellationToken = default);

    Task AgregarAsync(
        Estudiante estudiante,
        CancellationToken cancellationToken = default);

    Task<int> GuardarCambiosAsync(
        CancellationToken cancellationToken = default);
}
