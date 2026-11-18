
using LISTO.Domain.Enums;

namespace LISTO.Domain.Entities;

public class Ejercicio
{
    private readonly List<EjercicioVersion> _versiones = new();

    public long EjercicioId { get; private set; }

    public long CursoId { get; private set; }

    public DateTime FechaCreacion { get; private set; }

    public EstadoEjercicio Estado { get; private set; }

    public Curso Curso { get; private set; } = null!;

    public IReadOnlyCollection<EjercicioVersion> Versiones =>
        _versiones.AsReadOnly();

    private Ejercicio() { }

    public Ejercicio(Curso curso)
    {
        ArgumentNullException.ThrowIfNull(curso);

        if (!curso.EstaActivo)
            throw new InvalidOperationException(
                "No se puede crear un ejercicio para un curso inactivo.");

        Curso = curso;
        CursoId = curso.CursoId;

        FechaCreacion = DateTime.UtcNow;
        Estado = EstadoEjercicio.Activo;
    }

    public EjercicioVersion CrearVersion()
    {
        if (Estado != EstadoEjercicio.Activo)
            throw new InvalidOperationException(
                "No se pueden crear versiones de un ejercicio retirado.");

        int numeroVersion = _versiones.Count == 0
            ? 1
            : _versiones.Max(v => v.NumeroVersion) + 1;

        var version = new EjercicioVersion(
            EjercicioId,
            numeroVersion);

        _versiones.Add(version);

        return version;
    }

    public void PublicarVersion(int numeroVersion)
    {
        if (Estado != EstadoEjercicio.Activo)
            throw new InvalidOperationException(
                "No se pueden publicar versiones de un ejercicio retirado.");

        var version = ObtenerVersion(numeroVersion);

        if (version is null)
            throw new InvalidOperationException(
                "La versión indicada no existe en este ejercicio.");

        if (VersionPublicada is not null)
            throw new InvalidOperationException(
                "El ejercicio ya tiene una versión publicada.");

        version.Publicar();
    }

    public void RetirarVersion(int numeroVersion)
    {
        var version = ObtenerVersion(numeroVersion);

        if (version is null)
            throw new InvalidOperationException(
                "La versión indicada no existe en este ejercicio.");

        version.Retirar();
    }

    public EjercicioVersion? ObtenerVersion(int numeroVersion)
    {
        return _versiones.FirstOrDefault(
            v => v.NumeroVersion == numeroVersion);
    }

    public void Retirar()
    {
        Estado = EstadoEjercicio.Retirado;
    }

    public EjercicioVersion? VersionPublicada =>
    _versiones.FirstOrDefault(
        v => v.Estado == EstadoEjercicioVersion.Publicado);
}
