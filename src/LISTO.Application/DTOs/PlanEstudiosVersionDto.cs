
using LISTO.Domain.Enums;

namespace LISTO.Application.DTOs;

public class PlanEstudiosVersionDto
{
    public long PlanEstudiosVersionId { get; set; }

    public long PlanEstudiosId { get; set; }

    public int NumeroVersion { get; set; }

    public DateOnly? FechaVigenciaDesde { get; set; }

    public DateOnly? FechaVigenciaHasta { get; set; }

    public EstadoPlanEstudiosVersion Estado { get; set; }

    public string? Observaciones { get; set; }

    public IReadOnlyList<PlanCursoDto> Cursos { get; set; } = [];
}
