
namespace LISTO.Domain.Entities;

public class Estudiante
{
    public long EstudianteId { get; private set; }

    public long UsuarioId { get; private set; }

    public string DNI { get; private set; } = string.Empty;

    public DateTime FechaAlta { get; private set; }

    public bool EstaActivo { get; private set; }

    public Usuario Usuario { get; private set; } = null!;

    private Estudiante()
    {
        // Constructor requerido por Entity Framework Core.
    }

    public Estudiante(Usuario usuario, string dni)
    {
        ArgumentNullException.ThrowIfNull(usuario);

        if (!usuario.EstaActivo)
            throw new InvalidOperationException(
                "No se puede registrar como estudiante a un usuario inactivo.");

        ValidarDNI(dni);

        Usuario = usuario;
        UsuarioId = usuario.UsuarioId;
        DNI = dni.Trim();
        FechaAlta = DateTime.UtcNow;
        EstaActivo = true;
    }

    public void Desactivar()
    {
        EstaActivo = false;
    }

    public void Activar()
    {
        EstaActivo = true;
    }

    private static void ValidarDNI(string dni)
    {
        if (string.IsNullOrWhiteSpace(dni))
            throw new ArgumentException(
                "El DNI es obligatorio.",
                nameof(dni));

        dni = dni.Trim();

        if (dni.Length < 7 || dni.Length > 8 ||
            !dni.All(char.IsDigit))
        {
            throw new ArgumentException(
                "El DNI debe contener entre 7 y 8 dígitos.",
                nameof(dni));
        }
    }
}
