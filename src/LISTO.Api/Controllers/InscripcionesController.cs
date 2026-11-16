
using LISTO.Api.Contracts.Inscripciones;
using LISTO.Application.DTOs;
using LISTO.Application.Exceptions;
using LISTO.Application.Interfaces;
using LISTO.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace LISTO.Api.Controllers;

[ApiController]
[Route("api/inscripciones")]
public class InscripcionesController : ControllerBase
{
    private readonly IInscripcionService _inscripcionService;

    public InscripcionesController(IInscripcionService inscripcionService)
    {
        _inscripcionService = inscripcionService;
    }

    [HttpPost]
    public async Task<ActionResult<InscripcionDto>> Inscribir(
        [FromBody] CrearInscripcionRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var inscripcion = await _inscripcionService.InscribirAsync(
                request.EstudianteId,
                request.PlanCursoId,
                cancellationToken);

            return CreatedAtAction(
                nameof(ObtenerPorId),
                new { id = inscripcion.InscripcionId },
                inscripcion);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Datos de inscripción inválidos.",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
        catch (InscripcionActivaDuplicadaException ex)
        {
            return Conflict(new ProblemDetails
            {
                Title = "Ya existe una inscripción activa.",
                Detail = ex.Message,
                Status = StatusCodes.Status409Conflict
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ProblemDetails
            {
                Title = "No se pudo realizar la inscripción.",
                Detail = ex.Message,
                Status = StatusCodes.Status409Conflict
            });
        }
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<InscripcionDto>> ObtenerPorId(
        long id,
        CancellationToken cancellationToken)
    {
        var inscripcion = await _inscripcionService.ObtenerPorIdAsync(
            id,
            cancellationToken);

        if (inscripcion is null)
            return NotFound();

        return Ok(inscripcion);
    }

    [HttpGet("estudiante/{estudianteId:long}")]
    public async Task<ActionResult<IReadOnlyList<InscripcionDto>>> ListarPorEstudiante(
        long estudianteId,
        [FromQuery] EstadoInscripcion? estado,
        CancellationToken cancellationToken)
    {
        try
        {
            var inscripciones = await _inscripcionService.ListarPorEstudianteAsync(
                estudianteId,
                estado,
                cancellationToken);

            return Ok(inscripciones);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new ProblemDetails
            {
                Title = "No se encontró el estudiante.",
                Detail = ex.Message,
                Status = StatusCodes.Status404NotFound
            });
        }
    }

    [HttpPatch("{id:long}/aprobar")]
    public async Task<IActionResult> Aprobar(
        long id,
        CancellationToken cancellationToken)
    {
        try
        {
            var resultado = await _inscripcionService.AprobarAsync(
                id,
                cancellationToken);

            if (!resultado)
                return NotFound();

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ProblemDetails
            {
                Title = "No se pudo aprobar la inscripción.",
                Detail = ex.Message,
                Status = StatusCodes.Status409Conflict
            });
        }
    }

    [HttpPatch("{id:long}/desaprobar")]
    public async Task<IActionResult> Desaprobar(
        long id,
        CancellationToken cancellationToken)
    {
        try
        {
            var resultado = await _inscripcionService.DesaprobarAsync(
                id,
                cancellationToken);

            if (!resultado)
                return NotFound();

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ProblemDetails
            {
                Title = "No se pudo desaprobar la inscripción.",
                Detail = ex.Message,
                Status = StatusCodes.Status409Conflict
            });
        }
    }

    [HttpPatch("{id:long}/abandonar")]
    public async Task<IActionResult> Abandonar(
        long id,
        CancellationToken cancellationToken)
    {
        try
        {
            var resultado = await _inscripcionService.AbandonarAsync(
                id,
                cancellationToken);

            if (!resultado)
                return NotFound();

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ProblemDetails
            {
                Title = "No se pudo registrar el abandono.",
                Detail = ex.Message,
                Status = StatusCodes.Status409Conflict
            });
        }
    }

    [HttpPatch("{id:long}/anular")]
    public async Task<IActionResult> Anular(
        long id,
        CancellationToken cancellationToken)
    {
        try
        {
            var resultado = await _inscripcionService.AnularAsync(
                id,
                cancellationToken);

            if (!resultado)
                return NotFound();

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ProblemDetails
            {
                Title = "No se pudo anular la inscripción.",
                Detail = ex.Message,
                Status = StatusCodes.Status409Conflict
            });
        }
    }
}
