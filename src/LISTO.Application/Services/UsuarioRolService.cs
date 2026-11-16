using LISTO.Application.DTOs;
using LISTO.Application.Interfaces;
using LISTO.Domain.Entities;

namespace LISTO.Application.Services;

public class UsuarioRolService : IUsuarioRolService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IRolRepository _rolRepository;
    private readonly IUsuarioRolRepository _usuarioRolRepository;

    public UsuarioRolService(
        IUsuarioRepository usuarioRepository,
        IRolRepository rolRepository,
        IUsuarioRolRepository usuarioRolRepository)
    {
        ArgumentNullException.ThrowIfNull(usuarioRepository);
        ArgumentNullException.ThrowIfNull(rolRepository);
        ArgumentNullException.ThrowIfNull(usuarioRolRepository);

        _usuarioRepository = usuarioRepository;
        _rolRepository = rolRepository;
        _usuarioRolRepository = usuarioRolRepository;
    }

    public async Task<UsuarioRolDto?> AsignarAsync(
        long usuarioId,
        long rolId,
        CancellationToken cancellationToken = default)
    {
        if (usuarioId <= 0)
            throw new ArgumentOutOfRangeException(nameof(usuarioId));

        if (rolId <= 0)
            throw new ArgumentOutOfRangeException(nameof(rolId));

        var usuario = await _usuarioRepository.ObtenerPorIdAsync(
            usuarioId, cancellationToken);

        if (usuario is null)
            return null;

        var rol = await _rolRepository.ObtenerPorIdAsync(
            rolId, cancellationToken);

        if (rol is null)
            return null;

        var usuarioRol = new UsuarioRol(usuario, rol);

        await _usuarioRolRepository.AgregarAsync(
            usuarioRol, cancellationToken);

        await _usuarioRolRepository.GuardarCambiosAsync(
            cancellationToken);

        return MapearADto(usuarioRol);
    }

    public async Task<bool> RevocarAsync(
        long usuarioId,
        long rolId,
        CancellationToken cancellationToken = default)
    {
        if (usuarioId <= 0)
            throw new ArgumentOutOfRangeException(nameof(usuarioId));

        if (rolId <= 0)
            throw new ArgumentOutOfRangeException(nameof(rolId));

        var asignacion =
            await _usuarioRolRepository.ObtenerAsignacionVigenteAsync(
                usuarioId,
                rolId,
                cancellationToken);

        if (asignacion is null)
            return false;

        asignacion.Revocar();

        await _usuarioRolRepository.GuardarCambiosAsync(
            cancellationToken);

        return true;
    }

    public async Task<IReadOnlyList<UsuarioRolDto>?> ListarPorUsuarioAsync(
        long usuarioId,
        bool? soloVigentes = null,
        CancellationToken cancellationToken = default)
    {
        if (usuarioId <= 0)
            throw new ArgumentOutOfRangeException(nameof(usuarioId));

        var usuario = await _usuarioRepository.ObtenerPorIdAsync(
            usuarioId, cancellationToken);

        if (usuario is null)
            return null;

        var asignaciones =
            await _usuarioRolRepository.ListarPorUsuarioAsync(
                usuarioId,
                soloVigentes,
                cancellationToken);

        return asignaciones
            .Select(MapearADto)
            .ToList();
    }

    private static UsuarioRolDto MapearADto(UsuarioRol usuarioRol)
    {
        return new UsuarioRolDto
        {
            UsuarioRolId = usuarioRol.UsuarioRolId,
            UsuarioId = usuarioRol.UsuarioId,
            RolId = usuarioRol.RolId,
            NombreRol = usuarioRol.Rol.Nombre,
            FechaAsignacion = usuarioRol.FechaAsignacion,
            FechaRevocacion = usuarioRol.FechaRevocacion
        };
    }
}