using LISTO.Application.Exceptions;
using LISTO.Application.Interfaces;
using LISTO.Domain.Entities;
using LISTO.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Npgsql;

namespace LISTO.Infrastructure.Repositories;

public class UsuarioRolRepository : IUsuarioRolRepository
{
    private readonly ListoDbContext _context;

    public UsuarioRolRepository(ListoDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public async Task<UsuarioRol?> ObtenerPorIdAsync(
        long usuarioRolId,
        CancellationToken cancellationToken = default)
    {
        return await _context.UsuariosRoles
            .FirstOrDefaultAsync(
                ur => ur.UsuarioRolId == usuarioRolId,
                cancellationToken);
    }

    public async Task<UsuarioRol?> ObtenerAsignacionVigenteAsync(
        long usuarioId,
        long rolId,
        CancellationToken cancellationToken = default)
    {
        return await _context.UsuariosRoles
            .FirstOrDefaultAsync(
                ur => ur.UsuarioId == usuarioId
                    && ur.RolId == rolId
                    && ur.FechaRevocacion == null,
                cancellationToken);
    }

    public async Task<IReadOnlyList<UsuarioRol>> ListarPorUsuarioAsync(
        long usuarioId,
        bool? soloVigentes = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<UsuarioRol> consulta = _context.UsuariosRoles
            .AsNoTracking()
            .Include(ur => ur.Rol)
            .Where(ur => ur.UsuarioId == usuarioId);

        if (soloVigentes.HasValue && soloVigentes.Value)
        {
            consulta = consulta.Where(
                ur => ur.FechaRevocacion == null);
        }
        else if (soloVigentes.HasValue && !soloVigentes.Value)
        {
            consulta = consulta.Where(
                ur => ur.FechaRevocacion != null);
        }

        return await consulta
            .OrderByDescending(ur => ur.FechaAsignacion)
            .ToListAsync(cancellationToken);
    }

    public async Task AgregarAsync(
        UsuarioRol usuarioRol,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(usuarioRol);

        await _context.UsuariosRoles.AddAsync(
            usuarioRol,
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
                && postgresException.ConstraintName == "ux_usuario_rol_vigente")
        {
            throw new AsignacionRolDuplicadaException();
        }
    }
}