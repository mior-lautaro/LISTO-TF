namespace LISTO.Api.Contracts.Cursos;

public class CrearCursoRequest
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
}
