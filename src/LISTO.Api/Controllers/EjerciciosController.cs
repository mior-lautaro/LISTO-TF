
using LISTO.Api.Contracts.Ejercicios;
using LISTO.Application.DTOs;
using LISTO.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LISTO.Api.Controllers;

[ApiController]
[Route("api/ejercicios")]
public class EjerciciosController : ControllerBase
{
    private readonly IEjercicioService _ejercicioService;

    public EjerciciosController(IEjercicioService ejercicioService)
    {
        _ejercicioService = ejercicioService;
    }

    [HttpPost]
    public async Task<ActionResult<EjercicioDto>> Crear(
        [FromBody] CrearEjercicioRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var ejercicio = await _ejercicioService.CrearAsync(
                request.CursoId,
                cancellationToken);

            return CreatedAtAction(
                nameof(ObtenerPorId),
                new { id = ejercicio.EjercicioId },
                ejercicio);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Datos del ejercicio inválidos.",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ProblemDetails
            {
                Title = "No se pudo crear el ejercicio.",
                Detail = ex.Message,
                Status = StatusCodes.Status409Conflict
            });
        }
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<EjercicioDto>> ObtenerPorId(
        long id,
        CancellationToken cancellationToken)
    {
        var ejercicio = await _ejercicioService.ObtenerPorIdAsync(
            id,
            cancellationToken);

        if (ejercicio is null)
            return NotFound();

        return Ok(ejercicio);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EjercicioDto>>> Listar(
        [FromQuery] long? cursoId,
        CancellationToken cancellationToken)
    {
        try
        {
            var ejercicios = await _ejercicioService.ListarAsync(
                cursoId,
                cancellationToken);

            return Ok(ejercicios);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new ProblemDetails
            {
                Title = "No se encontró el curso.",
                Detail = ex.Message,
                Status = StatusCodes.Status404NotFound
            });
        }
    }

    [HttpPatch("{id:long}/retirar")]
    public async Task<IActionResult> Retirar(
        long id,
        CancellationToken cancellationToken)
    {
        var resultado = await _ejercicioService.RetirarAsync(
            id,
            cancellationToken);

        if (!resultado)
            return NotFound();

        return NoContent();
    }
}
