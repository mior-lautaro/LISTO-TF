using LISTO.Domain.Enums;

namespace LISTO.Application.DTOs;

public class EjercicioDto
{
    public long EjercicioId { get; set; }

    public long CursoId { get; set; }

    public DateTime FechaCreacion { get; set; }

    public EstadoEjercicio Estado { get; set; }
}