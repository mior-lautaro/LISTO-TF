using LISTO.Application.Exceptions;
using LISTO.Application.Interfaces;
using LISTO.Domain.Entities;
using LISTO.Domain.Enums;
using LISTO.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace LISTO.Infrastructure.Repositories;

public class IntentoRepository : IIntentoRepository
{
    private readonly ListoDbContext _context;

    public IntentoRepository(ListoDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public async Task<Intento?> ObtenerPorIdAsync(
        long intentoId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Intentos
            .FirstOrDefaultAsync(
                i => i.IntentoId == intentoId,
                cancellationToken);
    }

    public async Task<Intento?> ObtenerConRespuestaPorIdAsync(
    long intentoId,
    CancellationToken cancellationToken = default)
    {
        return await _context.Intentos
            .Include(i => i.EjercicioVersion)
                .ThenInclude(v => v.Pregunta)
            .Include(i => i.Respuesta)
                .ThenInclude(r => r!.RespuestaLibre)
            .FirstOrDefaultAsync(
                i => i.IntentoId == intentoId,
                cancellationToken);
    }

    public async Task<bool> ExisteEnCursoAsync(
        long estudianteId,
        long ejercicioId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Intentos
            .AnyAsync(
                i => i.EstudianteId == estudianteId
                    && i.EjercicioId == ejercicioId
                    && i.Estado == EstadoIntento.EnCurso,
                cancellationToken);
    }

    public async Task AgregarAsync(
        Intento intento,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(intento);

        await _context.Intentos.AddAsync(
            intento,
            cancellationToken);
    }

    public async Task<int> GuardarCambiosAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.SaveChangesAsync(
                cancellationToken);
        }
        catch (DbUpdateException ex)
            when (ex.GetBaseException() is PostgresException postgresException
                && postgresException.SqlState ==
                    PostgresErrorCodes.UniqueViolation
                && postgresException.ConstraintName ==
                    "ux_intento_estudiante_ejercicio_en_curso")
        {
            throw new IntentoEnCursoExistenteException();
        }
    }
}