using System;
using System.Collections.Generic;
using System.Text;
using LISTO.Application.Interfaces;
using LISTO.Domain.Entities;
using LISTO.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using LISTO.Application.Exceptions;
using Npgsql;

namespace LISTO.Infrastructure.Repositories;

public class RolRepository : IRolRepository
{
    private readonly ListoDbContext _context;

    public RolRepository(ListoDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public async Task<Rol?> ObtenerPorIdAsync(
        long rolId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Roles
            .FirstOrDefaultAsync(
                r => r.RolId == rolId,
                cancellationToken);
    }

    public async Task<Rol?> ObtenerPorNombreAsync(
        string nombre,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nombre);

        var nombreNormalizado = nombre.Trim().ToLowerInvariant();

        return await _context.Roles
            .FirstOrDefaultAsync(
                r => EF.Property<string>(
                    r, "NombreNormalizado") == nombreNormalizado,
                cancellationToken);
    }

    public async Task<bool> ExisteNombreAsync(
        string nombre,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nombre);

        var nombreNormalizado = nombre.Trim().ToLowerInvariant();

        return await _context.Roles
            .AnyAsync(
                r => EF.Property<string>(
                    r, "NombreNormalizado") == nombreNormalizado,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Rol>> ListarAsync(
        bool? estaActivo = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Rol> consulta = _context.Roles;

        if (estaActivo.HasValue)
        {
            consulta = consulta.Where(
                r => r.EstaActivo == estaActivo.Value);
        }

        return await consulta
            .OrderBy(r => r.Nombre)
            .ToListAsync(cancellationToken);
    }

    public async Task AgregarAsync(
        Rol rol,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(rol);

        await _context.Roles.AddAsync(
            rol,
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
                && postgresException.ConstraintName == "ux_rol_nombre_normalizado")
        {
            throw new NombreRolDuplicadoException(
                "el nombre indicado");
        }
    }
}
