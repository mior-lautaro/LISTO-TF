
using LISTO.Application.DTOs;
using LISTO.Application.Exceptions;
using LISTO.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LISTO.Api.Controllers;

[ApiController]
[Route("api/intentos")]
public class IntentosController : ControllerBase
{
    private readonly IIntentoService _intentoService;

    public IntentosController(IIntentoService intentoService)
    {
        ArgumentNullException.ThrowIfNull(intentoService);
        _intentoService = intentoService;
    }

    [HttpPost(
        "~/api/estudiantes/{estudianteId:long}/ejercicios/{ejercicioId:long}/intentos")]
    public async Task<ActionResult<IntentoDto>> Iniciar(
        long estudianteId,
        long ejercicioId,
        CancellationToken cancellationToken)
    {
        try
        {
            var intento = await _intentoService.IniciarAsync(
                estudianteId,
                ejercicioId,
                cancellationToken);

            return CreatedAtAction(
                nameof(ObtenerPorId),
                new { id = intento.IntentoId },
                intento);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Datos del intento inválidos.",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
        catch (IntentoEnCursoExistenteException ex)
        {
            return Conflict(new ProblemDetails
            {
                Title = "Ya existe un intento en curso.",
                Detail = ex.Message,
                Status = StatusCodes.Status409Conflict
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ProblemDetails
            {
                Title = "No se pudo iniciar el intento.",
                Detail = ex.Message,
                Status = StatusCodes.Status409Conflict
            });
        }
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<IntentoDto>> ObtenerPorId(
        long id,
        CancellationToken cancellationToken)
    {
        var intento = await _intentoService.ObtenerPorIdAsync(
            id,
            cancellationToken);

        if (intento is null)
            return NotFound();

        return Ok(intento);
    }


    [HttpPost("{id:long}/respuesta")]
    public async Task<ActionResult<RespuestaLibreDto>> RegistrarRespuesta(
        long id,
        [FromBody] RespuestaLibreRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var respuesta = await _intentoService
                .RegistrarRespuestaLibreAsync(
                    id,
                    request.Texto,
                    cancellationToken);

            return Ok(respuesta);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Datos de respuesta inválidos.",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ProblemDetails
            {
                Title = "No se pudo registrar la respuesta.",
                Detail = ex.Message,
                Status = StatusCodes.Status409Conflict
            });
        }
    }

    [HttpPatch("{id:long}/respuesta")]
    public async Task<ActionResult<RespuestaLibreDto>> ModificarRespuesta(
        long id,
        [FromBody] RespuestaLibreRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var respuesta = await _intentoService
                .ModificarRespuestaLibreAsync(
                    id,
                    request.Texto,
                    cancellationToken);

            return Ok(respuesta);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Datos de respuesta inválidos.",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ProblemDetails
            {
                Title = "No se pudo modificar la respuesta.",
                Detail = ex.Message,
                Status = StatusCodes.Status409Conflict
            });
        }
    }

    [HttpPost("{id:long}/enviar")]
    public async Task<ActionResult<IntentoDto>> Enviar(
        long id,
        CancellationToken cancellationToken)
    {
        try
        {
            var intento = await _intentoService.EnviarAsync(
                id,
                cancellationToken);

            return Ok(intento);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Intento inexistente.",
                Detail = ex.Message,
                Status = StatusCodes.Status404NotFound
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ProblemDetails
            {
                Title = "No se pudo enviar el intento.",
                Detail = ex.Message,
                Status = StatusCodes.Status409Conflict
            });
        }
    }

}
