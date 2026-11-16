
using LISTO.Application.Exceptions;
using LISTO.Application.Interfaces;
using LISTO.Domain.Entities;
using LISTO.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace LISTO.Infrastructure.Repositories;

public class EstudianteRepository : IEstudianteRepository
{
    private readonly ListoDbContext _context;

    public EstudianteRepository(ListoDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public async Task<Estudiante?> ObtenerPorIdAsync(
        long estudianteId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Estudiantes
            .FirstOrDefaultAsync(
                e => e.EstudianteId == estudianteId,
                cancellationToken);
    }

    public async Task<Estudiante?> ObtenerPorUsuarioIdAsync(
        long usuarioId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Estudiantes
            .FirstOrDefaultAsync(
                e => e.UsuarioId == usuarioId,
                cancellationToken);
    }

    public async Task<Estudiante?> ObtenerPorDNIAsync(
        string dni,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dni);

        var dniNormalizado = dni.Trim();

        return await _context.Estudiantes
            .FirstOrDefaultAsync(
                e => e.DNI == dniNormalizado,
                cancellationToken);
    }

    public async Task<bool> ExisteDNIAsync(
        string dni,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dni);

        var dniNormalizado = dni.Trim();

        return await _context.Estudiantes
            .AnyAsync(
                e => e.DNI == dniNormalizado,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Estudiante>> ListarAsync(
        bool? estaActivo = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Estudiante> consulta = _context.Estudiantes
            .AsNoTracking();

        if (estaActivo.HasValue)
        {
            consulta = consulta.Where(
                e => e.EstaActivo == estaActivo.Value);
        }

        return await consulta
            .OrderByDescending(e => e.FechaAlta)
            .ThenBy(e => e.EstudianteId)
            .ToListAsync(cancellationToken);
    }

    public async Task AgregarAsync(
        Estudiante estudiante,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(estudiante);

        await _context.Estudiantes.AddAsync(
            estudiante,
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
                && postgresException.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            if (postgresException.ConstraintName == "ux_estudiante_dni")
            {
                throw new EstudianteDuplicadoException(
                    "Ya existe un estudiante registrado con ese DNI.");
            }

            if (postgresException.ConstraintName == "ux_estudiante_usuario")
            {
                throw new EstudianteDuplicadoException(
                    "El usuario ya está registrado como estudiante.");
            }

            throw;
        }
    }
}
