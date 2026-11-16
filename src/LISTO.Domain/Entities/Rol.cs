namespace LISTO.Domain.Entities;

public class Rol
{
    public long RolId { get; private set; }

    public string Nombre { get; private set; } = null!;

    public string? Descripcion { get; private set; }

    public bool EstaActivo { get; private set; }

    // Constructor requerido por Entity Framework Core.
    private Rol()
    {
    }

    // Constructor para crear roles desde la aplicación.
    public Rol(string nombre, string? descripcion = null)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException(
                "El nombre del rol es obligatorio.",
                nameof(nombre));

        var nombreNormalizado = nombre.Trim();

        if (nombreNormalizado.Length > 100)
            throw new ArgumentException(
                "El nombre del rol no puede superar los 100 caracteres.",
                nameof(nombre));

        var descripcionNormalizada = string.IsNullOrWhiteSpace(descripcion)
            ? null
            : descripcion.Trim();

        if (descripcionNormalizada is not null &&
            descripcionNormalizada.Length > 500)
            throw new ArgumentException(
                "La descripción no puede superar los 500 caracteres.",
                nameof(descripcion));

        Nombre = nombreNormalizado;
        Descripcion = descripcionNormalizada;
        EstaActivo = true;
    }

    public void Activar()
    {
        EstaActivo = true;
    }

    public void Desactivar()
    {
        EstaActivo = false;
    }
}