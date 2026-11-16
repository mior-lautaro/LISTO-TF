using LISTO.Domain.Enums;

namespace LISTO.Domain.Entities;

public class Ejercicio
{
    public long EjercicioId { get; private set; }

    public long CursoId { get; private set; }

    public DateTime FechaCreacion { get; private set; }

    public EstadoEjercicio Estado { get; private set; }

    public Curso Curso { get; private set; } = null!;

    // Constructor requerido por Entity Framework Core.
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

    public void Retirar()
    {
        Estado = EstadoEjercicio.Retirado;
    }
}