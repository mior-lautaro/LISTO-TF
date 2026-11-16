
using System.ComponentModel.DataAnnotations;

namespace LISTO.Api.Contracts.Usuarios;

public class RegistrarUsuarioRequest
{
    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
    [StringLength(254, ErrorMessage = "El correo electrónico no puede superar los 254 caracteres.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [StringLength(100, ErrorMessage = "El apellido no puede superar los 100 caracteres.")]
    public string Apellido { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [StringLength(
    128,
    MinimumLength = 8,
    ErrorMessage = "La contraseña debe tener entre 8 y 128 caracteres.")]
    public string Contrasena { get; set; } = string.Empty;
}
