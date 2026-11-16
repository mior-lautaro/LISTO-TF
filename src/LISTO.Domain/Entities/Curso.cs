namespace LISTO.Domain.Entities;

public class Curso
{
    public long CursoId { get; private set; }

    public string Codigo { get; private set; } = null!;
    public string Nombre { get; private set; } = null!;
    public string? Descripcion { get; private set; }

    public bool EstaActivo { get; private set; }

    // Constructor requerido por EF Core.
    private Curso() { }

    public Curso(
        string codigo,
        string nombre,
        string? descripcion = null)
    {
        Codigo = ValidarCodigo(codigo);

        ValidarDatos(nombre, descripcion);

        Nombre = nombre.Trim();
        Descripcion = NormalizarDescripcion(descripcion);

        EstaActivo = true;
    }

    public void ModificarDatos(
        string nombre,
        string? descripcion)
    {
        ValidarDatos(nombre, descripcion);

        Nombre = nombre.Trim();
        Descripcion = NormalizarDescripcion(descripcion);
    }

    public void Desactivar()
    {
        EstaActivo = false;
    }

    public void Activar()
    {
        EstaActivo = true;
    }

    private static string ValidarCodigo(string codigo)
    {
        if (string.IsNullOrWhiteSpace(codigo))
            throw new ArgumentException(
                "El código es obligatorio.",
                nameof(codigo));

        codigo = codigo.Trim();

        if (codigo.Length > 20)
            throw new ArgumentException(
                "El código no puede superar los 20 caracteres.",
                nameof(codigo));

        return codigo;
    }

    private static void ValidarDatos(
        string nombre,
        string? descripcion)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException(
                "El nombre es obligatorio.",
                nameof(nombre));

        if (nombre.Trim().Length > 150)
            throw new ArgumentException(
                "El nombre no puede superar los 150 caracteres.",
                nameof(nombre));

        if (descripcion?.Trim().Length > 500)
            throw new ArgumentException(
                "La descripción no puede superar los 500 caracteres.",
                nameof(descripcion));
    }

    private static string? NormalizarDescripcion(
        string? descripcion)
    {
        return string.IsNullOrWhiteSpace(descripcion)
            ? null
            : descripcion.Trim();
    }
}
