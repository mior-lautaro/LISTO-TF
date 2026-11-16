namespace LISTO.Api.Contracts.PlanesEstudios;

public class ModificarPlanEstudiosRequest
{
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
}