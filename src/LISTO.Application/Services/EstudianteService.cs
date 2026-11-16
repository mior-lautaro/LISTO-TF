
using LISTO.Application.DTOs;
using LISTO.Application.Interfaces;
using LISTO.Domain.Entities;

namespace LISTO.Application.Services;

public class EstudianteService : IEstudianteService
{
    private readonly IEstudianteRepository _estudianteRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public EstudianteService(
        IEstudianteRepository estudianteRepository,
        IUsuarioRepository usuarioRepository)
    {
        ArgumentNullException.ThrowIfNull(estudianteRepository);
        ArgumentNullException.ThrowIfNull(usuarioRepository);

        _estudianteRepository = estudianteRepository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<EstudianteDto> RegistrarAsync(
        long usuarioId,
        string dni,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dni);

        var dniNormalizado = dni.Trim();

        if (dniNormalizado.Length < 7 ||
            dniNormalizado.Length > 8 ||
            !dniNormalizado.All(char.IsDigit))
        {
            throw new ArgumentException(
                "El DNI debe contener entre 7 y 8 dígitos.",
                nameof(dni));
        }

        var usuario = await _usuarioRepository.ObtenerPorIdAsync(
            usuarioId,
            cancellationToken);

        if (usuario is null)
            throw new ArgumentException(
                "El usuario indicado no existe.",
                nameof(usuarioId));

        if (!usuario.EstaActivo)
            throw new InvalidOperationException(
                "No se puede registrar como estudiante a un usuario inactivo.");

        var estudianteExistente =
            await _estudianteRepository.ObtenerPorUsuarioIdAsync(
                usuarioId,
                cancellationToken);

        if (estudianteExistente is not null)
            throw new InvalidOperationException(
                "El usuario ya está registrado como estudiante.");

        if (await _estudianteRepository.ExisteDNIAsync(
            dniNormalizado,
            cancellationToken))
        {
            throw new InvalidOperationException(
                "Ya existe un estudiante registrado con ese DNI.");
        }

        var estudiante = new Estudiante(usuario, dniNormalizado);

        await _estudianteRepository.AgregarAsync(
            estudiante,
            cancellationToken);

        await _estudianteRepository.GuardarCambiosAsync(
            cancellationToken);

        return MapearADto(estudiante);
    }

    public async Task<EstudianteDto?> ObtenerPorIdAsync(
        long estudianteId,
        CancellationToken cancellationToken = default)
    {
        var estudiante = await _estudianteRepository.ObtenerPorIdAsync(
            estudianteId,
            cancellationToken);

        return estudiante is null ? null : MapearADto(estudiante);
    }

    public async Task<EstudianteDto?> ObtenerPorUsuarioIdAsync(
        long usuarioId,
        CancellationToken cancellationToken = default)
    {
        var estudiante =
            await _estudianteRepository.ObtenerPorUsuarioIdAsync(
                usuarioId,
                cancellationToken);

        return estudiante is null ? null : MapearADto(estudiante);
    }

    public async Task<EstudianteDto?> ObtenerPorDNIAsync(
        string dni,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dni);

        var estudiante = await _estudianteRepository.ObtenerPorDNIAsync(
            dni.Trim(),
            cancellationToken);

        return estudiante is null ? null : MapearADto(estudiante);
    }

    public async Task<IReadOnlyList<EstudianteDto>> ListarAsync(
        bool? estaActivo = null,
        CancellationToken cancellationToken = default)
    {
        var estudiantes = await _estudianteRepository.ListarAsync(
            estaActivo,
            cancellationToken);

        return estudiantes
            .Select(MapearADto)
            .ToList();
    }

    public async Task<bool> DesactivarAsync(
        long estudianteId,
        CancellationToken cancellationToken = default)
    {
        var estudiante = await _estudianteRepository.ObtenerPorIdAsync(
            estudianteId,
            cancellationToken);

        if (estudiante is null)
            return false;

        if (!estudiante.EstaActivo)
            return true;

        estudiante.Desactivar();

        await _estudianteRepository.GuardarCambiosAsync(
            cancellationToken);

        return true;
    }

    public async Task<bool> ActivarAsync(
        long estudianteId,
        CancellationToken cancellationToken = default)
    {
        var estudiante = await _estudianteRepository.ObtenerPorIdAsync(
            estudianteId,
            cancellationToken);

        if (estudiante is null)
            return false;

        if (estudiante.EstaActivo)
            return true;

        estudiante.Activar();

        await _estudianteRepository.GuardarCambiosAsync(
            cancellationToken);

        return true;
    }

    private static EstudianteDto MapearADto(Estudiante estudiante)
    {
        return new EstudianteDto
        {
            EstudianteId = estudiante.EstudianteId,
            UsuarioId = estudiante.UsuarioId,
            DNI = estudiante.DNI,
            FechaAlta = estudiante.FechaAlta,
            EstaActivo = estudiante.EstaActivo
        };
    }
}
