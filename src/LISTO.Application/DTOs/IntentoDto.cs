using LISTO.Domain.Enums;

namespace LISTO.Application.DTOs;

public class IntentoDto
{
    public long IntentoId { get; set; }

    public long EstudianteId { get; set; }

    public long EjercicioId { get; set; }

    public long EjercicioVersionId { get; set; }

    public EstadoIntento Estado { get; set; }

    public DateTime FechaInicio { get; set; }

    public DateTime? FechaEnvio { get; set; }

    public DateTime? FechaEvaluacion { get; set; }
}