
using LISTO.Application.DTOs;

namespace LISTO.Application.Interfaces;

public interface IIntentoService
{
    Task<IntentoDto> IniciarAsync(
        long estudianteId,
        long ejercicioId,
        CancellationToken cancellationToken = default);

    Task<IntentoDto?> ObtenerPorIdAsync(
        long intentoId,
        CancellationToken cancellationToken = default);

    Task<RespuestaLibreDto> RegistrarRespuestaLibreAsync(
        long intentoId,
        string texto,
        CancellationToken cancellationToken = default);

    Task<RespuestaLibreDto> ModificarRespuestaLibreAsync(
        long intentoId,
        string texto,
        CancellationToken cancellationToken = default);

    Task<IntentoDto> EnviarAsync(
        long intentoId,
        CancellationToken cancellationToken = default);
}
