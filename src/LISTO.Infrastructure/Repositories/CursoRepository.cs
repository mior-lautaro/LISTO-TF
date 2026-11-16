using System;
using System.Collections.Generic;
using System.Text;

using System;
using System.Collections.Generic;
using LISTO.Application.Interfaces;
using LISTO.Domain.Entities;
using LISTO.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LISTO.Infrastructure.Repositories;

public class CursoRepository : ICursoRepository
{
    private readonly ListoDbContext _context;

    public CursoRepository(ListoDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public async Task<Curso?> ObtenerPorIdAsync(
        long cursoId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Cursos
            .FirstOrDefaultAsync(
                c => c.CursoId == cursoId,
                cancellationToken);
    }

    public async Task<Curso?> ObtenerPorCodigoAsync(
        string codigo,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(codigo);

        var codigoNormalizado = codigo.Trim();

        return await _context.Cursos
            .FirstOrDefaultAsync(
                c => c.Codigo == codigoNormalizado,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Curso>> ListarAsync(
        bool? estaActivo = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Curso> consulta = _context.Cursos;

        if (estaActivo.HasValue)
        {
            consulta = consulta.Where(
                c => c.EstaActivo == estaActivo.Value);
        }

        return await consulta
            .OrderBy(c => c.Nombre)
            .ToListAsync(cancellationToken);
    }

    public async Task AgregarAsync(
        Curso curso,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(curso);

        await _context.Cursos.AddAsync(
            curso,
            cancellationToken);
    }

    public async Task<int> GuardarCambiosAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}