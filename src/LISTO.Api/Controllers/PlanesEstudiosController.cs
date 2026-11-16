
using LISTO.Api.Contracts.PlanesEstudios;
using LISTO.Application.DTOs;
using LISTO.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace LISTO.Api.Controllers;

[ApiController]
[Route("api/planes-estudios")]
public class PlanesEstudiosController : ControllerBase
{
    private readonly IPlanEstudiosService _planEstudiosService;

    public PlanesEstudiosController(
    IPlanEstudiosService planEstudiosService,
    IPlanEstudiosVersionService planEstudiosVersionService)
    {
        _planEstudiosService = planEstudiosService;
        _planEstudiosVersionService = planEstudiosVersionService;
    }

    private readonly IPlanEstudiosVersionService _planEstudiosVersionService;

    [HttpPost]
    public async Task<ActionResult<PlanEstudiosDto>> Crear(
        [FromBody] CrearPlanEstudiosRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var plan = await _planEstudiosService.CrearAsync(
                request.Nombre,
                request.Descripcion,
                cancellationToken);

            return CreatedAtAction(
                nameof(ObtenerPorId),
                new { id = plan.PlanEstudiosId },
                plan);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Datos del plan de estudios inválidos.",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
    }

    [HttpPost("{planEstudiosId:long}/versiones")]
    public async Task<ActionResult<PlanEstudiosVersionDto>> CrearVersion(
    long planEstudiosId,
    [FromBody] CrearPlanEstudiosVersionRequest request,
    CancellationToken cancellationToken)
    {
        try
        {
            var version = await _planEstudiosVersionService.CrearAsync(
                planEstudiosId,
                request.FechaVigenciaDesde,
                request.Observaciones,
                cancellationToken);

            if (version is null)
                return NotFound();

            return StatusCode(
                StatusCodes.Status201Created,
                version);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Datos de la versión del plan de estudios inválidos.",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ProblemDetails
            {
                Title = "No se puede crear la versión del plan de estudios.",
                Detail = ex.Message,
                Status = StatusCodes.Status409Conflict
            });
        }
    }

    [HttpGet("{planEstudiosId:long}/versiones")]
    public async Task<ActionResult<IReadOnlyList<PlanEstudiosVersionDto>>> ListarVersiones(
    long planEstudiosId,
    CancellationToken cancellationToken)
    {
        var plan = await _planEstudiosService.ObtenerPorIdAsync(
            planEstudiosId,
            cancellationToken);

        if (plan is null)
            return NotFound();

        var versiones = await _planEstudiosVersionService.ListarAsync(
            planEstudiosId,
            cancellationToken);

        return Ok(versiones);
    }

    [HttpPost("{planEstudiosId:long}/versiones/{versionId:long}/cursos")]
    public async Task<ActionResult<PlanCursoDto>> AgregarCurso(
    long planEstudiosId,
    long versionId,
    [FromBody] AgregarCursoAPlanEstudiosVersionRequest request,
    CancellationToken cancellationToken)
    {
        try
        {
            var planCurso = await _planEstudiosVersionService.AgregarCursoAsync(
                planEstudiosId,
                versionId,
                request.CursoId,
                request.AnioNivel,
                request.PeriodoRecomendado,
                request.CargaHoraria,
                cancellationToken);

            if (planCurso is null)
                return NotFound();

            return StatusCode(
                StatusCodes.Status201Created,
                planCurso);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Curso inexistente.",
                Detail = ex.Message,
                Status = StatusCodes.Status404NotFound
            });
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
        catch (InvalidOperationException ex)
        {
            return Conflict(new ProblemDetails
            {
                Title = "No se puede incorporar el curso.",
                Detail = ex.Message,
                Status = StatusCodes.Status409Conflict
            });
        }
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<PlanEstudiosDto>> ObtenerPorId(
        long id,
        CancellationToken cancellationToken)
    {
        var plan = await _planEstudiosService.ObtenerPorIdAsync(
            id,
            cancellationToken);

        if (plan is null)
            return NotFound();

        return Ok(plan);
    }

    [HttpGet("codigo/{codigo}")]
    public async Task<ActionResult<PlanEstudiosDto>> ObtenerPorCodigo(
        string codigo,
        CancellationToken cancellationToken)
    {
        var plan = await _planEstudiosService.ObtenerPorCodigoAsync(
            codigo,
            cancellationToken);

        if (plan is null)
            return NotFound();

        return Ok(plan);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PlanEstudiosDto>>> Listar(
        [FromQuery] bool? estaActivo,
        CancellationToken cancellationToken)
    {
        var planes = await _planEstudiosService.ListarAsync(
            estaActivo,
            cancellationToken);

        return Ok(planes);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> ModificarDatos(
        long id,
        [FromBody] ModificarPlanEstudiosRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var resultado = await _planEstudiosService.ModificarDatosAsync(
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
                Title = "Datos del plan de estudios inválidos.",
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
        try
        {
            var resultado = await _planEstudiosService.DesactivarAsync(
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
                Title = "No se puede desactivar el plan de estudios.",
                Detail = ex.Message,
                Status = StatusCodes.Status409Conflict
            });
        }
    }

    [HttpPatch("{id:long}/activar")]
    public async Task<IActionResult> Activar(
        long id,
        CancellationToken cancellationToken)
    {
        var resultado = await _planEstudiosService.ActivarAsync(
            id,
            cancellationToken);

        if (!resultado)
            return NotFound();

        return NoContent();
    }

    [HttpGet("versiones/{versionId:long}")]
    public async Task<ActionResult<PlanEstudiosVersionDto>> ObtenerVersionPorId(
    long versionId,
    CancellationToken cancellationToken)
    {
        try
        {
            var version = await _planEstudiosVersionService.ObtenerPorIdAsync(
                versionId,
                cancellationToken);

            if (version is null)
                return NotFound();

            return Ok(version);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Identificador de versión inválido.",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
    }

    [HttpPatch("{planEstudiosId:long}/versiones/{versionId:long}/publicar")]
    public async Task<IActionResult> PublicarVersion(
    long planEstudiosId,
    long versionId,
    CancellationToken cancellationToken)
    {
        try
        {
            var resultado = await _planEstudiosVersionService.PublicarAsync(
                planEstudiosId,
                versionId,
                cancellationToken);

            if (!resultado)
                return NotFound();

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Identificadores de versión inválidos.",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ProblemDetails
            {
                Title = "No se puede publicar la versión.",
                Detail = ex.Message,
                Status = StatusCodes.Status409Conflict
            });
        }
    }

    [HttpPatch("{planEstudiosId:long}/versiones/{versionId:long}/finalizar-vigencia")]
    public async Task<IActionResult> FinalizarVigencia(
    long planEstudiosId,
    long versionId,
    [FromBody] FinalizarVigenciaPlanEstudiosVersionRequest request,
    CancellationToken cancellationToken)
    {
        try
        {
            var resultado = await _planEstudiosVersionService.FinalizarVigenciaAsync(
                planEstudiosId,
                versionId,
                request.FechaVigenciaHasta,
                cancellationToken);

            if (!resultado)
                return NotFound();

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Fecha o identificadores de vigencia inválidos.",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ProblemDetails
            {
                Title = "No se puede finalizar la vigencia.",
                Detail = ex.Message,
                Status = StatusCodes.Status409Conflict
            });
        }
    }
}
