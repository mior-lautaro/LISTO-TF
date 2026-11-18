using LISTO.Application.DTOs;
using LISTO.Application.Exceptions;
using LISTO.Application.Interfaces;
using LISTO.Domain.Entities;

namespace LISTO.Application.Services;

public class IntentoService : IIntentoService
{
    private readonly IEstudianteRepository _estudianteRepository;
    private readonly IEjercicioRepository _ejercicioRepository;
    private readonly IInscripcionRepository _inscripcionRepository;
    private readonly IIntentoRepository _intentoRepository;

    public IntentoService(
        IEstudianteRepository estudianteRepository,
        IEjercicioRepository ejercicioRepository,
        IInscripcionRepository inscripcionRepository,
        IIntentoRepository intentoRepository)
    {
        ArgumentNullException.ThrowIfNull(estudianteRepository);
        ArgumentNullException.ThrowIfNull(ejercicioRepository);
        ArgumentNullException.ThrowIfNull(inscripcionRepository);
        ArgumentNullException.ThrowIfNull(intentoRepository);

        _estudianteRepository = estudianteRepository;
        _ejercicioRepository = ejercicioRepository;
        _inscripcionRepository = inscripcionRepository;
        _intentoRepository = intentoRepository;
    }

    public async Task<IntentoDto> IniciarAsync(
        long estudianteId,
        long ejercicioId,
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
                "No se puede iniciar un intento para un estudiante inactivo.");

        var ejercicio =
            await _ejercicioRepository.ObtenerConVersionesPorIdAsync(
                ejercicioId,
                cancellationToken);

        if (ejercicio is null)
            throw new ArgumentException(
                "El ejercicio indicado no existe.",
                nameof(ejercicioId));

        if (ejercicio.Estado != LISTO.Domain.Enums.EstadoEjercicio.Activo)
            throw new InvalidOperationException(
                "No se puede iniciar un intento para un ejercicio retirado.");

        var versionPublicada = ejercicio.VersionPublicada;

        if (versionPublicada is null)
            throw new InvalidOperationException(
                "El ejercicio no tiene una versión publicada.");

        var tieneInscripcion = await
            _inscripcionRepository.TieneInscripcionActivaEnCursoAsync(
                estudianteId,
                ejercicio.CursoId,
                cancellationToken);

        if (!tieneInscripcion)
            throw new InvalidOperationException(
                "El estudiante no tiene una inscripción activa en el curso del ejercicio.");

        var existeIntentoEnCurso =
            await _intentoRepository.ExisteEnCursoAsync(
                estudianteId,
                ejercicioId,
                cancellationToken);

        if (existeIntentoEnCurso)
            throw new IntentoEnCursoExistenteException();

        var intento = new Intento(
            estudiante,
            ejercicio,
            versionPublicada);

        await _intentoRepository.AgregarAsync(
            intento,
            cancellationToken);

        await _intentoRepository.GuardarCambiosAsync(
            cancellationToken);

        return MapearADto(intento);
    }

    public async Task<IntentoDto?> ObtenerPorIdAsync(
        long intentoId,
        CancellationToken cancellationToken = default)
    {
        var intento = await _intentoRepository.ObtenerPorIdAsync(
            intentoId,
            cancellationToken);

        return intento is null ? null : MapearADto(intento);
    }


    public async Task<RespuestaLibreDto> RegistrarRespuestaLibreAsync(
        long intentoId,
        string texto,
        CancellationToken cancellationToken = default)
    {
        var intento = await ObtenerIntentoCompletoAsync(
            intentoId,
            cancellationToken);

        var respuesta = intento.RegistrarRespuestaLibre(texto);

        await _intentoRepository.GuardarCambiosAsync(
            cancellationToken);

        return MapearRespuestaADto(intento, respuesta);
    }

    public async Task<RespuestaLibreDto> ModificarRespuestaLibreAsync(
        long intentoId,
        string texto,
        CancellationToken cancellationToken = default)
    {
        var intento = await ObtenerIntentoCompletoAsync(
            intentoId,
            cancellationToken);

        intento.ModificarRespuestaLibre(texto);

        await _intentoRepository.GuardarCambiosAsync(
            cancellationToken);

        return MapearRespuestaADto(
            intento,
            intento.Respuesta!.RespuestaLibre!);
    }

    public async Task<IntentoDto> EnviarAsync(
        long intentoId,
        CancellationToken cancellationToken = default)
    {
        var intento = await ObtenerIntentoCompletoAsync(
            intentoId,
            cancellationToken);

        intento.Enviar();

        await _intentoRepository.GuardarCambiosAsync(
            cancellationToken);

        return MapearADto(intento);
    }

    private async Task<Intento> ObtenerIntentoCompletoAsync(
        long intentoId,
        CancellationToken cancellationToken)
    {
        var intento = await _intentoRepository
            .ObtenerConRespuestaPorIdAsync(
                intentoId,
                cancellationToken);

        return intento ?? throw new ArgumentException(
            "El intento indicado no existe.",
            nameof(intentoId));
    }

    private static RespuestaLibreDto MapearRespuestaADto(
        Intento intento,
        RespuestaLibre respuesta)
    {
        return new RespuestaLibreDto
        {
            RespuestaId = respuesta.RespuestaId,
            IntentoId = intento.IntentoId,
            PreguntaId = intento.Respuesta!.PreguntaId,
            Texto = respuesta.Texto,
            FechaRegistro = intento.Respuesta.FechaRegistro
        };
    }


    private static IntentoDto MapearADto(Intento intento)
    {
        return new IntentoDto
        {
            IntentoId = intento.IntentoId,
            EstudianteId = intento.EstudianteId,
            EjercicioId = intento.EjercicioId,
            EjercicioVersionId = intento.EjercicioVersionId,
            Estado = intento.Estado,
            FechaInicio = intento.FechaInicio,
            FechaEnvio = intento.FechaEnvio,
            FechaEvaluacion = intento.FechaEvaluacion
        };
    }
}