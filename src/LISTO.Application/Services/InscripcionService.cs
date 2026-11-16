
using LISTO.Application.DTOs;
using LISTO.Application.Interfaces;
using LISTO.Domain.Entities;
using LISTO.Domain.Enums;
using LISTO.Application.Exceptions;

namespace LISTO.Application.Services;

public class InscripcionService : IInscripcionService
{
    private readonly IInscripcionRepository _inscripcionRepository;
    private readonly IEstudianteRepository _estudianteRepository;
    private readonly IPlanEstudiosRepository _planEstudiosRepository;

    public InscripcionService(
        IInscripcionRepository inscripcionRepository,
        IEstudianteRepository estudianteRepository,
        IPlanEstudiosRepository planEstudiosRepository)
    {
        ArgumentNullException.ThrowIfNull(inscripcionRepository);
        ArgumentNullException.ThrowIfNull(estudianteRepository);
        ArgumentNullException.ThrowIfNull(planEstudiosRepository);

        _inscripcionRepository = inscripcionRepository;
        _estudianteRepository = estudianteRepository;
        _planEstudiosRepository = planEstudiosRepository;
    }

    public async Task<InscripcionDto> InscribirAsync(
        long estudianteId,
        long planCursoId,
        CancellationToken cancellationToken = default)
    {
        var estudiante = await _estudianteRepository.ObtenerPorIdAsync(
            estudianteId,
            cancellationToken);

        if (estudiante is null)
            throw new ArgumentException(
                "El estudiante indicado no existe.",
                nameof(estudianteId));

        if (!estudiante.EstaActivo)
            throw new InvalidOperationException(
                "No se puede inscribir a un estudiante inactivo.");

        var version =
            await _planEstudiosRepository.ObtenerVersionPorPlanCursoIdAsync(
                planCursoId,
                cancellationToken);

        if (version is null)
            throw new ArgumentException(
                "El curso no pertenece a ninguna versión de un plan de estudios.",
                nameof(planCursoId));

        if (version.Estado != EstadoPlanEstudiosVersion.Vigente)
            throw new InvalidOperationException(
                "Solo se permiten inscripciones en versiones vigentes de planes de estudios.");

        var inscripcionExistente =
            await _inscripcionRepository.ObtenerActivaAsync(
                estudianteId,
                planCursoId,
                cancellationToken);

        if (inscripcionExistente is not null)
            throw new InscripcionActivaDuplicadaException();

        var planCurso = version.Cursos.First(
            pc => pc.PlanCursoId == planCursoId);

        var inscripcion = new Inscripcion(
            estudiante,
            planCurso);

        await _inscripcionRepository.AgregarAsync(
            inscripcion,
            cancellationToken);

        await _inscripcionRepository.GuardarCambiosAsync(
            cancellationToken);

        return MapearADto(inscripcion);
    }

    public async Task<InscripcionDto?> ObtenerPorIdAsync(
        long inscripcionId,
        CancellationToken cancellationToken = default)
    {
        var inscripcion = await _inscripcionRepository.ObtenerPorIdAsync(
            inscripcionId,
            cancellationToken);

        return inscripcion is null ? null : MapearADto(inscripcion);
    }

    public async Task<IReadOnlyList<InscripcionDto>> ListarPorEstudianteAsync(
        long estudianteId,
        EstadoInscripcion? estado = null,
        CancellationToken cancellationToken = default)
    {
        var estudiante = await _estudianteRepository.ObtenerPorIdAsync(
            estudianteId,
            cancellationToken);

        if (estudiante is null)
            throw new ArgumentException(
                "El estudiante indicado no existe.",
                nameof(estudianteId));

        var inscripciones =
            await _inscripcionRepository.ListarPorEstudianteAsync(
                estudianteId,
                estado,
                cancellationToken);

        return inscripciones
            .Select(MapearADto)
            .ToList();
    }

    public Task<bool> AprobarAsync(
        long inscripcionId,
        CancellationToken cancellationToken = default)
    {
        return FinalizarAsync(
            inscripcionId,
            EstadoInscripcion.Aprobada,
            cancellationToken);
    }

    public Task<bool> DesaprobarAsync(
        long inscripcionId,
        CancellationToken cancellationToken = default)
    {
        return FinalizarAsync(
            inscripcionId,
            EstadoInscripcion.Desaprobada,
            cancellationToken);
    }

    public Task<bool> AbandonarAsync(
        long inscripcionId,
        CancellationToken cancellationToken = default)
    {
        return FinalizarAsync(
            inscripcionId,
            EstadoInscripcion.Abandonada,
            cancellationToken);
    }

    public Task<bool> AnularAsync(
        long inscripcionId,
        CancellationToken cancellationToken = default)
    {
        return FinalizarAsync(
            inscripcionId,
            EstadoInscripcion.Anulada,
            cancellationToken);
    }

    private async Task<bool> FinalizarAsync(
        long inscripcionId,
        EstadoInscripcion estado,
        CancellationToken cancellationToken)
    {
        var inscripcion = await _inscripcionRepository.ObtenerPorIdAsync(
            inscripcionId,
            cancellationToken);

        if (inscripcion is null)
            return false;

        switch (estado)
        {
            case EstadoInscripcion.Aprobada:
                inscripcion.Aprobar();
                break;

            case EstadoInscripcion.Desaprobada:
                inscripcion.Desaprobar();
                break;

            case EstadoInscripcion.Abandonada:
                inscripcion.Abandonar();
                break;

            case EstadoInscripcion.Anulada:
                inscripcion.Anular();
                break;

            default:
                throw new ArgumentException(
                    "El estado final indicado no es válido.",
                    nameof(estado));
        }

        await _inscripcionRepository.GuardarCambiosAsync(
            cancellationToken);

        return true;
    }

    private static InscripcionDto MapearADto(Inscripcion inscripcion)
    {
        return new InscripcionDto
        {
            InscripcionId = inscripcion.InscripcionId,
            EstudianteId = inscripcion.EstudianteId,
            PlanCursoId = inscripcion.PlanCursoId,
            FechaInscripcion = inscripcion.FechaInscripcion,
            Estado = inscripcion.Estado,
            FechaResolucion = inscripcion.FechaResolucion
        };
    }
}

