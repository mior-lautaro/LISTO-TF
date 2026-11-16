namespace LISTO.Domain.Entities;

public class Usuario
{
    public long UsuarioId { get; private set; }

    public string Email { get; private set; } = null!;

    public string Nombre { get; private set; } = null!;

    public string Apellido { get; private set; } = null!;

    public string HashContrasena { get; private set; } = null!;

    public bool EstaActivo { get; private set; }

    public DateTime FechaAlta { get; private set; }

    public void ModificarDatosPersonales(
    string nombre,
    string apellido)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException(
                "El nombre es obligatorio.",
                nameof(nombre));

        if (string.IsNullOrWhiteSpace(apellido))
            throw new ArgumentException(
                "El apellido es obligatorio.",
                nameof(apellido));

        var nombreNormalizado = nombre.Trim();
        var apellidoNormalizado = apellido.Trim();

        if (nombreNormalizado.Length > 100)
            throw new ArgumentException(
                "El nombre no puede superar los 100 caracteres.",
                nameof(nombre));

        if (apellidoNormalizado.Length > 100)
            throw new ArgumentException(
                "El apellido no puede superar los 100 caracteres.",
                nameof(apellido));

        Nombre = nombreNormalizado;
        Apellido = apellidoNormalizado;
    }

    public void Desactivar()
    {
        EstaActivo = false;
    }

    public void Activar()
    {
        EstaActivo = true;
    }

    // Constructor requerido por Entity Framework Core.
    private Usuario()
    {
    }

    // Constructor para crear usuarios desde la aplicación.
    public Usuario(
        string email,
        string nombre,
        string apellido,
        string hashContrasena)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException(
                "El correo electrónico es obligatorio.",
                nameof(email));

        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException(
                "El nombre es obligatorio.",
                nameof(nombre));

        if (string.IsNullOrWhiteSpace(apellido))
            throw new ArgumentException(
                "El apellido es obligatorio.",
                nameof(apellido));

        if (string.IsNullOrWhiteSpace(hashContrasena))
            throw new ArgumentException(
                "El hash de contraseña es obligatorio.",
                nameof(hashContrasena));

        var emailNormalizado = email.Trim().ToLowerInvariant();
        var nombreNormalizado = nombre.Trim();
        var apellidoNormalizado = apellido.Trim();

        if (emailNormalizado.Length > 254)
            throw new ArgumentException(
                "El correo electrónico no puede superar los 254 caracteres.",
                nameof(email));

        if (nombreNormalizado.Length > 100)
            throw new ArgumentException(
                "El nombre no puede superar los 100 caracteres.",
                nameof(nombre));

        if (apellidoNormalizado.Length > 100)
            throw new ArgumentException(
                "El apellido no puede superar los 100 caracteres.",
                nameof(apellido));

        if (hashContrasena.Length > 500)
            throw new ArgumentException(
                "El hash de contraseña no puede superar los 500 caracteres.",
                nameof(hashContrasena));

        Email = emailNormalizado;
        Nombre = nombreNormalizado;
        Apellido = apellidoNormalizado;
        HashContrasena = hashContrasena;

        EstaActivo = true;
        FechaAlta = DateTime.UtcNow;
    }
}