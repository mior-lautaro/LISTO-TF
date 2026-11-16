using System;
using System.Collections.Generic;
using System.Text;
namespace LISTO.Application.DTOs;

public class RolDto
{
    public long RolId { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public bool EstaActivo { get; set; }
}
