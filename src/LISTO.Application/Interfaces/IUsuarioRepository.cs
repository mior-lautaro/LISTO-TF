using LISTO.Domain.Entities;

namespace LISTO.Application.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> ObtenerPorIdAsync(
        long usuarioId,
        CancellationToken cancellationToken = default);

    Task<Usuario?> ObtenerPorEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task<bool> ExisteEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Usuario>> ListarAsync(
    bool? estaActivo = null,
    CancellationToken cancellationToken = default);

    Task AgregarAsync(
        Usuario usuario,
        CancellationToken cancellationToken = default);

    Task<int> GuardarCambiosAsync(
        CancellationToken cancellationToken = default);
}