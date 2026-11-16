namespace LISTO.Application.DTOs;

public class UsuarioRolDto
{
    public long UsuarioRolId { get; set; }

    public long UsuarioId { get; set; }

    public long RolId { get; set; }

    public string NombreRol { get; set; } = null!;

    public DateTime FechaAsignacion { get; set; }

    public DateTime? FechaRevocacion { get; set; }
}