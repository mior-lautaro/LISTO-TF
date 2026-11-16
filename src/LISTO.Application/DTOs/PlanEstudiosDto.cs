namespace LISTO.Application.DTOs;

public class PlanEstudiosDto
{
    public long PlanEstudiosId { get; set; }

    public string Codigo { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public bool EstaActivo { get; set; }
}