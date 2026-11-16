using System.ComponentModel.DataAnnotations;

namespace LISTO.Api.Contracts.Roles;

public class CrearRolRequest
{
    [Required(ErrorMessage = "El nombre del rol es obligatorio.")]
    [StringLength(
        100,
        ErrorMessage = "El nombre del rol no puede superar los 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(
        500,
        ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
    public string? Descripcion { get; set; }
}
