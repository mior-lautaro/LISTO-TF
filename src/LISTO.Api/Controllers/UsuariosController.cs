using LISTO.Api.Contracts.Usuarios;
using LISTO.Application.DTOs;
using LISTO.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using LISTO.Application.Exceptions;

namespace LISTO.Api.Controllers;

[ApiController]
[Route("api/usuarios")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;
    private readonly IUsuarioRolService _usuarioRolService;

    public UsuariosController(
    IUsuarioService usuarioService,
    IUsuarioRolService usuarioRolService)
    {
        _usuarioService = usuarioService;
        _usuarioRolService = usuarioRolService;
    }

    [HttpPost]
    public async Task<ActionResult<UsuarioDto>> Registrar(
        [FromBody] RegistrarUsuarioRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var usuario = await _usuarioService.RegistrarAsync(
                request.Email,
                request.Nombre,
                request.Apellido,
                request.Contrasena,
                cancellationToken);

            return CreatedAtAction(
                nameof(ObtenerPorId),
                new { id = usuario.UsuarioId },
                usuario);
        }
        catch (EmailDuplicadoException ex)
        {
            return Conflict(new ProblemDetails
            {
                Title = "Email duplicado.",
                Detail = ex.Message,
                Status = StatusCodes.Status409Conflict
            });
        }
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UsuarioDto>>> Listar(
    [FromQuery] bool? estaActivo,
    CancellationToken cancellationToken)
    {
        var usuarios = await _usuarioService.ListarAsync(
            estaActivo,
            cancellationToken);

        return Ok(usuarios);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<UsuarioDto>> ObtenerPorId(
        long id,
        CancellationToken cancellationToken)
    {
        var usuario = await _usuarioService.ObtenerPorIdAsync(
            id,
            cancellationToken);

        if (usuario is null)
            return NotFound();

        return Ok(usuario);
    }

    [HttpGet("email")]
    public async Task<ActionResult<UsuarioDto>> ObtenerPorEmail(
        [FromQuery] string email,
        CancellationToken cancellationToken)
    {
        var usuario = await _usuarioService.ObtenerPorEmailAsync(
            email,
            cancellationToken);

        if (usuario is null)
            return NotFound();

        return Ok(usuario);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> ModificarDatosPersonales(
    long id,
    [FromBody] ModificarUsuarioRequest request,
    CancellationToken cancellationToken)
    {
        try
        {
            var resultado = await _usuarioService.ModificarDatosPersonalesAsync(
                id,
                request.Nombre,
                request.Apellido,
                cancellationToken);

            if (!resultado)
                return NotFound();

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Datos personales inválidos.",
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
        var resultado = await _usuarioService.DesactivarAsync(
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
        var resultado = await _usuarioService.ActivarAsync(
            id,
            cancellationToken);

        if (!resultado)
            return NotFound();

        return NoContent();
    }

    [HttpPost("{usuarioId:long}/roles/{rolId:long}")]
    public async Task<ActionResult<UsuarioRolDto>> AsignarRol(
    long usuarioId,
    long rolId,
    CancellationToken cancellationToken)
    {
        try
        {
            var asignacion = await _usuarioRolService.AsignarAsync(
                usuarioId,
                rolId,
                cancellationToken);

            if (asignacion is null)
                return NotFound();

            return StatusCode(
                StatusCodes.Status201Created,
                asignacion);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Identificadores inválidos.",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
        catch (AsignacionRolDuplicadaException ex)
        {
            return Conflict(new ProblemDetails
            {
                Title = "Asignación duplicada.",
                Detail = ex.Message,
                Status = StatusCodes.Status409Conflict
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ProblemDetails
            {
                Title = "No se puede asignar el rol.",
                Detail = ex.Message,
                Status = StatusCodes.Status409Conflict
            });
        }
    }

    [HttpDelete("{usuarioId:long}/roles/{rolId:long}")]
    public async Task<IActionResult> RevocarRol(
        long usuarioId,
        long rolId,
        CancellationToken cancellationToken)
    {
        try
        {
            var resultado = await _usuarioRolService.RevocarAsync(
                usuarioId,
                rolId,
                cancellationToken);

            if (!resultado)
                return NotFound();

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Identificadores inválidos.",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
    }

    [HttpGet("{usuarioId:long}/roles")]
    public async Task<ActionResult<IReadOnlyList<UsuarioRolDto>>> ListarRoles(
        long usuarioId,
        [FromQuery] bool? soloVigentes,
        CancellationToken cancellationToken)
    {
        try
        {
            var asignaciones = await _usuarioRolService.ListarPorUsuarioAsync(
                usuarioId,
                soloVigentes,
                cancellationToken);

            if (asignaciones is null)
                return NotFound();

            return Ok(asignaciones);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Identificador de usuario inválido.",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
    }
}
