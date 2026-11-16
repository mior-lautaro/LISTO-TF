using System.ComponentModel.DataAnnotations;

namespace LISTO.Api.Contracts.Estudiantes;

public class CrearEstudianteRequest
{
    [Required]
    public long UsuarioId { get; set; }

    [Required]
    [RegularExpression(@"^[0-9]{7,8}$",
        ErrorMessage = "El DNI debe contener entre 7 y 8 dígitos.")]
    public string DNI { get; set; } = string.Empty;
}