using LISTO.Api.Contracts.Roles;
using LISTO.Application.DTOs;
using LISTO.Application.Exceptions;
using LISTO.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LISTO.Api.Controllers;

[ApiController]
[Route("api/roles")]
public class RolesController : ControllerBase
{
    private readonly IRolService _rolService;

    public RolesController(IRolService rolService)
    {
        ArgumentNullException.ThrowIfNull(rolService);
        _rolService = rolService;
    }

    [HttpPost]
    public async Task<ActionResult<RolDto>> Crear(
        [FromBody] CrearRolRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var rol = await _rolService.CrearAsync(
                request.Nombre,
                request.Descripcion,
                cancellationToken);

            return CreatedAtAction(
                nameof(ObtenerPorId),
                new { id = rol.RolId },
                rol);
        }
        catch (NombreRolDuplicadoException ex)
        {
            return Conflict(new ProblemDetails
            {
                Title = "Nombre de rol duplicado.",
                Detail = ex.Message,
                Status = StatusCodes.Status409Conflict
            });
        }
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RolDto>>> Listar(
        [FromQuery] bool? estaActivo,
        CancellationToken cancellationToken)
    {
        var roles = await _rolService.ListarAsync(
            estaActivo,
            cancellationToken);

        return Ok(roles);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<RolDto>> ObtenerPorId(
        long id,
        CancellationToken cancellationToken)
    {
        var rol = await _rolService.ObtenerPorIdAsync(
            id,
            cancellationToken);

        if (rol is null)
            return NotFound();

        return Ok(rol);
    }

    [HttpGet("nombre")]
    public async Task<ActionResult<RolDto>> ObtenerPorNombre(
        [FromQuery] string nombre,
        CancellationToken cancellationToken)
    {
        var rol = await _rolService.ObtenerPorNombreAsync(
            nombre,
            cancellationToken);

        if (rol is null)
            return NotFound();

        return Ok(rol);
    }

    [HttpPatch("{id:long}/desactivar")]
    public async Task<IActionResult> Desactivar(
        long id,
        CancellationToken cancellationToken)
    {
        var resultado = await _rolService.DesactivarAsync(
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
        var resultado = await _rolService.ActivarAsync(
            id,
            cancellationToken);

        if (!resultado)
            return NotFound();

        return NoContent();
    }
}
