
using LISTO.Application.DTOs;
using LISTO.Application.Interfaces;
using LISTO.Domain.Entities;

namespace LISTO.Application.Services;

public class EjercicioService : IEjercicioService
{
    private readonly IEjercicioRepository _ejercicioRepository;
    private readonly ICursoRepository _cursoRepository;

    public EjercicioService(
        IEjercicioRepository ejercicioRepository,
        ICursoRepository cursoRepository)
    {
        ArgumentNullException.ThrowIfNull(ejercicioRepository);
        ArgumentNullException.ThrowIfNull(cursoRepository);

        _ejercicioRepository = ejercicioRepository;
        _cursoRepository = cursoRepository;
    }

    public async Task<EjercicioDto> CrearAsync(
        long cursoId,
        CancellationToken cancellationToken = default)
    {
        var curso = await _cursoRepository.ObtenerPorIdAsync(
            cursoId,
            cancellationToken);

        if (curso is null)
            throw new ArgumentException(
                "El curso indicado no existe.",
                nameof(cursoId));

        var ejercicio = new Ejercicio(curso);

        await _ejercicioRepository.AgregarAsync(
            ejercicio,
            cancellationToken);

        await _ejercicioRepository.GuardarCambiosAsync(
            cancellationToken);

        return MapearADto(ejercicio);
    }

    public async Task<EjercicioDto?> ObtenerPorIdAsync(
        long ejercicioId,
        CancellationToken cancellationToken = default)
    {
        var ejercicio = await _ejercicioRepository.ObtenerPorIdAsync(
            ejercicioId,
            cancellationToken);

        return ejercicio is null ? null : MapearADto(ejercicio);
    }

    public async Task<IReadOnlyList<EjercicioDto>> ListarAsync(
        long? cursoId = null,
        CancellationToken cancellationToken = default)
    {
        if (cursoId.HasValue)
        {
            var curso = await _cursoRepository.ObtenerPorIdAsync(
                cursoId.Value,
                cancellationToken);

            if (curso is null)
                throw new ArgumentException(
                    "El curso indicado no existe.",
                    nameof(cursoId));
        }

        var ejercicios = await _ejercicioRepository.ListarAsync(
            cursoId,
            cancellationToken);

        return ejercicios
            .Select(MapearADto)
            .ToList();
    }

    public async Task<bool> RetirarAsync(
        long ejercicioId,
        CancellationToken cancellationToken = default)
    {
        var ejercicio = await _ejercicioRepository.ObtenerPorIdAsync(
            ejercicioId,
            cancellationToken);

        if (ejercicio is null)
            return false;

        if (ejercicio.Estado == LISTO.Domain.Enums.EstadoEjercicio.Retirado)
            return true;

        ejercicio.Retirar();

        await _ejercicioRepository.GuardarCambiosAsync(
            cancellationToken);

        return true;
    }

    private static EjercicioDto MapearADto(Ejercicio ejercicio)
    {
        return new EjercicioDto
        {
            EjercicioId = ejercicio.EjercicioId,
            CursoId = ejercicio.CursoId,
            FechaCreacion = ejercicio.FechaCreacion,
            Estado = ejercicio.Estado
        };
    }
}
