using System;
using System.Collections.Generic;
using System.Text;


namespace LISTO.Application.DTOs;

public class UsuarioDto
{
    public long UsuarioId { get; set; }

    public string Email { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public bool EstaActivo { get; set; }

    public DateTime FechaAlta { get; set; }
}
