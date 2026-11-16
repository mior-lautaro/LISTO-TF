namespace LISTO.Domain.Entities;

public class UsuarioRol
{
    public long UsuarioRolId { get; private set; }

    public long UsuarioId { get; private set; }

    public long RolId { get; private set; }

    public Usuario Usuario { get; private set; } = null!;

    public Rol Rol { get; private set; } = null!;

    public DateTime FechaAsignacion { get; private set; }

    public DateTime? FechaRevocacion { get; private set; }

    // Constructor requerido por Entity Framework Core.
    private UsuarioRol()
    {
    }

    // Constructor para crear una asignación.
    public UsuarioRol(Usuario usuario, Rol rol)
    {
        ArgumentNullException.ThrowIfNull(usuario);
        ArgumentNullException.ThrowIfNull(rol);

        if (!usuario.EstaActivo)
            throw new InvalidOperationException(
                "No se puede asignar un rol a un usuario inactivo.");

        if (!rol.EstaActivo)
            throw new InvalidOperationException(
                "No se puede asignar un rol inactivo.");

        Usuario = usuario;
        Rol = rol;

        FechaAsignacion = DateTime.UtcNow;
    }

    public void Revocar()
    {
        if (FechaRevocacion.HasValue)
            throw new InvalidOperationException(
                "La asignación del rol ya fue revocada.");

        FechaRevocacion = DateTime.UtcNow;
    }
}