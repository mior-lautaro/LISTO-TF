
using System;
using LISTO.Domain.Enums;

namespace LISTO.Application.DTOs;

public class InscripcionDto
{
    public long InscripcionId { get; set; }

    public long EstudianteId { get; set; }

    public long PlanCursoId { get; set; }

    public DateTime FechaInscripcion { get; set; }

    public EstadoInscripcion Estado { get; set; }

    public DateTime? FechaResolucion { get; set; }
}
