
using System;

namespace LISTO.Application.DTOs;

public class EstudianteDto
{
    public long EstudianteId { get; set; }

    public long UsuarioId { get; set; }

    public string DNI { get; set; } = null!;

    public bool EstaActivo { get; set; }

    public DateTime FechaAlta { get; set; }
}
