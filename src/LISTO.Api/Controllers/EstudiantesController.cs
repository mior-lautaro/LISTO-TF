
using LISTO.Api.Contracts.Estudiantes;
using LISTO.Application.DTOs;
using LISTO.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LISTO.Api.Controllers;

[ApiController]
[Route("api/estudiantes")]
public class EstudiantesController : ControllerBase
{
    private readonly IEstudianteService _estudianteService;

    public EstudiantesController(IEstudianteService estudianteService)
    {
        _estudianteService = estudianteService;
    }

    [HttpPost]
    public async Task<ActionResult<EstudianteDto>> Registrar(
        [FromBody] CrearEstudianteRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var estudiante = await _estudianteService.RegistrarAsync(
                request.UsuarioId,
                request.DNI,
                cancellationToken);

            return CreatedAtAction(
                nameof(ObtenerPorId),
                new { id = estudiante.EstudianteId },
                estudiante);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Datos del estudiante inválidos.",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ProblemDetails
            {
                Title = "No se pudo registrar el estudiante.",
                Detail = ex.Message,
                Status = StatusCodes.Status409Conflict
            });
        }
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<EstudianteDto>> ObtenerPorId(
        long id,
        CancellationToken cancellationToken)
    {
        var estudiante = await _estudianteService.ObtenerPorIdAsync(
            id,
            cancellationToken);

        if (estudiante is null)
            return NotFound();

        return Ok(estudiante);
    }

    [HttpGet("usuario/{usuarioId:long}")]
    public async Task<ActionResult<EstudianteDto>> ObtenerPorUsuario(
        long usuarioId,
        CancellationToken cancellationToken)
    {
        var estudiante = await _estudianteService.ObtenerPorUsuarioIdAsync(
            usuarioId,
            cancellationToken);

        if (estudiante is null)
            return NotFound();

        return Ok(estudiante);
    }

    [HttpGet("dni/{dni}")]
    public async Task<ActionResult<EstudianteDto>> ObtenerPorDNI(
        string dni,
        CancellationToken cancellationToken)
    {
        var estudiante = await _estudianteService.ObtenerPorDNIAsync(
            dni,
            cancellationToken);

        if (estudiante is null)
            return NotFound();

        return Ok(estudiante);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EstudianteDto>>> Listar(
        [FromQuery] bool? estaActivo,
        CancellationToken cancellationToken)
    {
        var estudiantes = await _estudianteService.ListarAsync(
            estaActivo,
            cancellationToken);

        return Ok(estudiantes);
    }

    [HttpPatch("{id:long}/desactivar")]
    public async Task<IActionResult> Desactivar(
        long id,
        CancellationToken cancellationToken)
    {
        var resultado = await _estudianteService.DesactivarAsync(
            id,
            cancellationToken);

        if (!resultado)
            return NotFound();

        return NoContent();
    }

    [HttpPatch("{id:long}/activar")]
    public async Task<IActionResult> Activar(
        long id,
        CancellationToken cancellationToken)
    {
        var resultado = await _estudianteService.ActivarAsync(
            id,
            cancellationToken);

        if (!resultado)
            return NotFound();

        return NoContent();
    }
}
