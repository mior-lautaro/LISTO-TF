using Microsoft.AspNetCore.Mvc;

namespace LISTO.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EstadoController : ControllerBase
{
    [HttpGet]
    public IActionResult ObtenerEstado()
    {
        return Ok(new
        {
            aplicacion = "LISTO",
            estado = "Operativa",
            version = "0.1.0"
        });
    }
}