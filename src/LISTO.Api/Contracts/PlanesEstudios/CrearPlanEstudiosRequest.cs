namespace LISTO.Api.Contracts.PlanesEstudios;

public class CrearPlanEstudiosRequest
{
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
}