using System.ComponentModel.DataAnnotations;

namespace LISTO.Api.Contracts.Usuarios;

public class ModificarUsuarioRequest
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [StringLength(100, ErrorMessage = "El apellido no puede superar los 100 caracteres.")]
    public string Apellido { get; set; } = string.Empty;
}
