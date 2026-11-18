
using LISTO.Application.Interfaces;
using LISTO.Domain.Entities;
using LISTO.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LISTO.Infrastructure.Repositories;

public class EjercicioRepository : IEjercicioRepository
{
    private readonly ListoDbContext _context;

    public EjercicioRepository(ListoDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public async Task<Ejercicio?> ObtenerPorIdAsync(
        long ejercicioId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Ejercicios
            .FirstOrDefaultAsync(
                e => e.EjercicioId == ejercicioId,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Ejercicio>> ListarAsync(
        long? cursoId = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Ejercicio> consulta = _context.Ejercicios;

        if (cursoId.HasValue)
        {
            consulta = consulta.Where(
                e => e.CursoId == cursoId.Value);
        }

        return await consulta
            .OrderByDescending(e => e.FechaCreacion)
            .ToListAsync(cancellationToken);
    }

    public async Task AgregarAsync(
        Ejercicio ejercicio,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(ejercicio);

        await _context.Ejercicios.AddAsync(
            ejercicio,
            cancellationToken);
    }

    public async Task<int> GuardarCambiosAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Ejercicio?> ObtenerConVersionesPorIdAsync(
    long ejercicioId,
    CancellationToken cancellationToken = default)
    {
        return await _context.Ejercicios
            .Include(e => e.Versiones)
                .ThenInclude(v => v.Pregunta)
                    .ThenInclude(p => p!.ConfiguracionRespuestaLibre)
                        .ThenInclude(c => c!.Criterios)
            .Include(e => e.Versiones)
                .ThenInclude(v => v.Pregunta)
                    .ThenInclude(p => p!.Opciones)
            .Include(e => e.Versiones)
                .ThenInclude(v => v.Pregunta)
                    .ThenInclude(p => p!.ConfiguracionVerdaderoFalso)
            .Include(e => e.Versiones)
                .ThenInclude(v => v.Pregunta)
                    .ThenInclude(p => p!.EspaciosCompletar)
                        .ThenInclude(esp => esp.Variantes)
            .FirstOrDefaultAsync(
                e => e.EjercicioId == ejercicioId,
                cancellationToken);
    }
}
