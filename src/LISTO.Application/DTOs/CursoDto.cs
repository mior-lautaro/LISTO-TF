using System;
using System.Collections.Generic;
using System.Text;

namespace LISTO.Application.DTOs;

public class CursoDto
{
    public long CursoId { get; set; }

    public string Codigo { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public bool EstaActivo { get; set; }
}
