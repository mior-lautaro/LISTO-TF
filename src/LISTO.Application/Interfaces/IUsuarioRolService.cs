using LISTO.Application.DTOs;

namespace LISTO.Application.Interfaces;

public interface IUsuarioRolService
{
    Task<UsuarioRolDto?> AsignarAsync(
        long usuarioId,
        long rolId,
        CancellationToken cancellationToken = default);

    Task<bool> RevocarAsync(
        long usuarioId,
        long rolId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<UsuarioRolDto>?> ListarPorUsuarioAsync(
        long usuarioId,
        bool? soloVigentes = null,
        CancellationToken cancellationToken = default);
}