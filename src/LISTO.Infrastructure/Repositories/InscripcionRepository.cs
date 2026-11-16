
using LISTO.Application.Exceptions;
using LISTO.Application.Interfaces;
using LISTO.Domain.Entities;
using LISTO.Domain.Enums;
using LISTO.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace LISTO.Infrastructure.Repositories;

public class InscripcionRepository : IInscripcionRepository
{
    private readonly ListoDbContext _context;

    public InscripcionRepository(ListoDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public async Task<Inscripcion?> ObtenerPorIdAsync(
        long inscripcionId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Inscripciones
            .FirstOrDefaultAsync(
                i => i.InscripcionId == inscripcionId,
                cancellationToken);
    }

    public async Task<Inscripcion?> ObtenerActivaAsync(
        long estudianteId,
        long planCursoId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Inscripciones
            .FirstOrDefaultAsync(
                i => i.EstudianteId == estudianteId
                    && i.PlanCursoId == planCursoId
                    && i.Estado == EstadoInscripcion.Activa,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Inscripcion>> ListarPorEstudianteAsync(
        long estudianteId,
        EstadoInscripcion? estado = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Inscripcion> consulta = _context.Inscripciones
            .AsNoTracking()
            .Where(i => i.EstudianteId == estudianteId);

        if (estado.HasValue)
        {
            consulta = consulta.Where(
                i => i.Estado == estado.Value);
        }

        return await consulta
            .OrderByDescending(i => i.FechaInscripcion)
            .ThenBy(i => i.InscripcionId)
            .ToListAsync(cancellationToken);
    }

    public async Task AgregarAsync(
    Inscripcion inscripcion,
    CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(inscripcion);

        // Las entidades relacionadas ya existen en la base de datos.
        // Attach las incorpora al contexto sin marcarlas para inserción.
        _context.Attach(inscripcion.Estudiante);
        _context.Attach(inscripcion.PlanCurso);

        // La única entidad nueva que debe insertarse es Inscripcion.
        await _context.Inscripciones.AddAsync(
            inscripcion,
            cancellationToken);
    }

    public async Task<int> GuardarCambiosAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
            when (ex.GetBaseException() is PostgresException postgresException
                && postgresException.SqlState == PostgresErrorCodes.UniqueViolation
                && postgresException.ConstraintName ==
                    "ux_inscripcion_estudiante_plan_curso_activa")
        {
            throw new InscripcionActivaDuplicadaException();
        }
    }
}
