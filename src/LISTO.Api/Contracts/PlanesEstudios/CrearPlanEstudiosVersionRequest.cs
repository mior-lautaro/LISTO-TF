
namespace LISTO.Api.Contracts.PlanesEstudios;

public class CrearPlanEstudiosVersionRequest
{
    public DateOnly? FechaVigenciaDesde { get; set; }

    public string? Observaciones { get; set; }
}
