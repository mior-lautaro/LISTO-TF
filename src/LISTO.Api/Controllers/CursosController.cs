using LISTO.Api.Contracts.Cursos;
using LISTO.Application.DTOs;
using LISTO.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LISTO.Api.Controllers;

[ApiController]
[Route("api/cursos")]
public class CursosController : ControllerBase
{
    private readonly ICursoService _cursoService;

    public CursosController(ICursoService cursoService)
    {
        _cursoService = cursoService;
    }

    [HttpPost]
    public async Task<ActionResult<CursoDto>> Crear(
        [FromBody] CrearCursoRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var curso = await _cursoService.CrearAsync(
                request.Nombre,
                request.Descripcion,
                cancellationToken);

            return CreatedAtAction(
                nameof(ObtenerPorId),
                new { id = curso.CursoId },
                curso);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Datos del curso inválidos.",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<CursoDto>> ObtenerPorId(
        long id,
        CancellationToken cancellationToken)
    {
        var curso = await _cursoService.ObtenerPorIdAsync(
            id,
            cancellationToken);

        if (curso is null)
            return NotFound();

        return Ok(curso);
    }

    [HttpGet("codigo/{codigo}")]
    public async Task<ActionResult<CursoDto>> ObtenerPorCodigo(
        string codigo,
        CancellationToken cancellationToken)
    {
        var curso = await _cursoService.ObtenerPorCodigoAsync(
            codigo,
            cancellationToken);

        if (curso is null)
            return NotFound();

        return Ok(curso);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CursoDto>>> Listar(
        [FromQuery] bool? estaActivo,
        CancellationToken cancellationToken)
    {
        var cursos = await _cursoService.ListarAsync(
            estaActivo,
            cancellationToken);

        return Ok(cursos);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> ModificarDatos(
        long id,
        [FromBody] ModificarCursoRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var resultado = await _cursoService.ModificarDatosAsync(
                id,
                request.Nombre,
                request.Descripcion,
                cancellationToken);

            if (!resultado)
                return NotFound();

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Datos del curso inválidos.",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
    }

    [HttpPatch("{id:long}/desactivar")]
    public async Task<IActionResult> Desactivar(
        long id,
        CancellationToken cancellationToken)
    {
        var resultado = await _cursoService.DesactivarAsync(
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
        var resultado = await _cursoService.ActivarAsync(
            id,
            cancellationToken);

        if (!resultado)
            return NotFound();

        return NoContent();
    }
}
