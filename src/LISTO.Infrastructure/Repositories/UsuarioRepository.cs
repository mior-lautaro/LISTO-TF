using LISTO.Application.Interfaces;
using LISTO.Domain.Entities;
using LISTO.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using LISTO.Application.Exceptions;
using Npgsql;

namespace LISTO.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly ListoDbContext _context;

    public UsuarioRepository(ListoDbContext context)
    {
        _context = context;
    }

    public async Task<Usuario?> ObtenerPorIdAsync(
        long usuarioId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Usuarios
            .FirstOrDefaultAsync(
                u => u.UsuarioId == usuarioId,
                cancellationToken);
    }

    public async Task<Usuario?> ObtenerPorEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        var emailNormalizado = email.Trim().ToLowerInvariant();

        return await _context.Usuarios
            .FirstOrDefaultAsync(
                u => u.Email == emailNormalizado,
                cancellationToken);
    }

    public async Task<bool> ExisteEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        var emailNormalizado = email.Trim().ToLowerInvariant();

        return await _context.Usuarios
            .AnyAsync(
                u => u.Email == emailNormalizado,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Usuario>> ListarAsync(
    bool? estaActivo = null,
    CancellationToken cancellationToken = default)
    {
        var query = _context.Usuarios
            .AsNoTracking()
            .AsQueryable();

        if (estaActivo.HasValue)
        {
            query = query.Where(
                u => u.EstaActivo == estaActivo.Value);
        }

        return await query
            .OrderByDescending(u => u.FechaAlta)
            .ThenBy(u => u.UsuarioId)
            .ToListAsync(cancellationToken);
    }
    public async Task AgregarAsync(
        Usuario usuario,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(usuario);

        await _context.Usuarios.AddAsync(usuario, cancellationToken);
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
                  && postgresException.ConstraintName == "ux_usuario_email")
        {
            throw new EmailDuplicadoException("El correo electrónico");
        }
    }
}
