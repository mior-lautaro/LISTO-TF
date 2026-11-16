using LISTO.Domain.Enums;

namespace LISTO.Domain.Entities;

public class PlanEstudios
{
    private readonly List<PlanEstudiosVersion> _versiones = new();

    public long PlanEstudiosId { get; private set; }
    public string Codigo { get; private set; } = null!;
    public string Nombre { get; private set; } = null!;
    public string? Descripcion { get; private set; }
    public bool EstaActivo { get; private set; }

    public IReadOnlyCollection<PlanEstudiosVersion> Versiones =>
        _versiones.AsReadOnly();

    private PlanEstudios()
    {
        // Constructor requerido por Entity Framework Core.
    }

    public PlanEstudios(
        string codigo,
        string nombre,
        string? descripcion = null)
    {
        ValidarCodigo(codigo);
        ValidarNombre(nombre);
        ValidarDescripcion(descripcion);

        Codigo = codigo.Trim();
        Nombre = nombre.Trim();
        Descripcion = NormalizarDescripcion(descripcion);
        EstaActivo = true;
    }

    public PlanEstudiosVersion CrearVersion(
        int numeroVersion,
        DateOnly? fechaVigenciaDesde = null,
        string? observaciones = null)
    {
        if (!EstaActivo)
            throw new InvalidOperationException(
                "No se pueden crear versiones de un plan inactivo.");

        if (numeroVersion <= 0)
            throw new ArgumentException(
                "El número de versión debe ser mayor que cero.");

        if (_versiones.Any(v => v.NumeroVersion == numeroVersion))
            throw new InvalidOperationException(
                "El número de versión ya existe en este plan.");

        if (_versiones.Any() &&
            numeroVersion <= _versiones.Max(v => v.NumeroVersion))
            throw new InvalidOperationException(
                "El número de versión debe ser superior al de las versiones anteriores.");

        var version = new PlanEstudiosVersion(
            PlanEstudiosId,
            numeroVersion,
            fechaVigenciaDesde,
            observaciones);

        _versiones.Add(version);

        return version;
    }

    public void ModificarDatos(
        string nombre,
        string? descripcion)
    {
        ValidarNombre(nombre);
        ValidarDescripcion(descripcion);

        Nombre = nombre.Trim();
        Descripcion = NormalizarDescripcion(descripcion);
    }

    public void Activar()
    {
        EstaActivo = true;
    }

    public void Desactivar()
    {
        if (_versiones.Any(v =>
            v.Estado == EstadoPlanEstudiosVersion.Vigente))
            throw new InvalidOperationException(
                "No se puede desactivar un plan que tiene una versión vigente. Primero debe finalizarse su vigencia.");

        EstaActivo = false;
    }

    public PlanEstudiosVersion? ObtenerVersion(int numeroVersion)
    {
        return _versiones.FirstOrDefault(
            v => v.NumeroVersion == numeroVersion);
    }

    public void PublicarVersion(int numeroVersion)
    {
        if (!EstaActivo)
            throw new InvalidOperationException(
                "No se pueden publicar versiones de un plan inactivo.");

        var version = ObtenerVersion(numeroVersion);

        if (version is null)
            throw new InvalidOperationException(
                "La versión indicada no existe en este plan.");

        if (_versiones.Any(v =>
            v.Estado == EstadoPlanEstudiosVersion.Vigente))
            throw new InvalidOperationException(
                "El plan ya tiene una versión vigente.");

        version.Publicar();
    }

    public void FinalizarVersionVigente(DateOnly fechaVigenciaHasta)
    {
        var versionVigente = _versiones.FirstOrDefault(
            v => v.Estado == EstadoPlanEstudiosVersion.Vigente);

        if (versionVigente is null)
            throw new InvalidOperationException(
                "El plan no tiene una versión vigente.");

        versionVigente.FinalizarVigencia(fechaVigenciaHasta);
    }

    private static void ValidarCodigo(string codigo)
    {
        if (string.IsNullOrWhiteSpace(codigo))
            throw new ArgumentException(
                "El código del plan es obligatorio.");

        if (codigo.Trim().Length > 20)
            throw new ArgumentException(
                "El código del plan no puede superar los 20 caracteres.");
    }

    private static void ValidarNombre(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException(
                "El nombre del plan es obligatorio.");

        if (nombre.Trim().Length > 150)
            throw new ArgumentException(
                "El nombre del plan no puede superar los 150 caracteres.");
    }

    private static void ValidarDescripcion(string? descripcion)
    {
        if (descripcion?.Trim().Length > 500)
            throw new ArgumentException(
                "La descripción no puede superar los 500 caracteres.");
    }

    private static string? NormalizarDescripcion(string? descripcion)
    {
        return string.IsNullOrWhiteSpace(descripcion)
            ? null
            : descripcion.Trim();
    }
}
