
using LISTO.Application.Interfaces;
using LISTO.Domain.Entities;
using LISTO.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace LISTO.Infrastructure.Repositories;

public class PlanEstudiosRepository : IPlanEstudiosRepository
{
    private readonly ListoDbContext _context;

    public PlanEstudiosRepository(ListoDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public async Task<PlanEstudios?> ObtenerPorIdAsync(
        long planEstudiosId,
        CancellationToken cancellationToken = default)
    {
        return await _context.PlanesEstudios
            .Include(p => p.Versiones)
                .ThenInclude(v => v.Cursos)
                    .ThenInclude(pc => pc.Curso)
            .AsSplitQuery()
            .FirstOrDefaultAsync(
                p => p.PlanEstudiosId == planEstudiosId,
                cancellationToken);
    }

    public async Task<PlanEstudios?> ObtenerPorCodigoAsync(
        string codigo,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(codigo);

        var codigoNormalizado = codigo.Trim();

        return await _context.PlanesEstudios
            .Include(p => p.Versiones)
                .ThenInclude(v => v.Cursos)
                    .ThenInclude(pc => pc.Curso)
            .AsSplitQuery()
            .FirstOrDefaultAsync(
                p => p.Codigo == codigoNormalizado,
                cancellationToken);
    }

    public async Task<IReadOnlyList<PlanEstudios>> ListarAsync(
        bool? estaActivo = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<PlanEstudios> consulta =
            _context.PlanesEstudios.AsNoTracking();

        if (estaActivo.HasValue)
        {
            consulta = consulta.Where(
                p => p.EstaActivo == estaActivo.Value);
        }

        return await consulta
            .OrderBy(p => p.Nombre)
            .ToListAsync(cancellationToken);
    }

    public async Task AgregarAsync(
        PlanEstudios planEstudios,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(planEstudios);

        await _context.PlanesEstudios.AddAsync(
            planEstudios,
            cancellationToken);
    }

    public async Task<IReadOnlyList<PlanEstudiosVersion>> ListarVersionesAsync(
    long planEstudiosId,
    CancellationToken cancellationToken = default)
    {
        return await _context.PlanesEstudiosVersiones
            .AsNoTracking()
            .Include(v => v.Cursos)
                .ThenInclude(pc => pc.Curso)
            .AsSplitQuery()
            .Where(v => v.PlanEstudiosId == planEstudiosId)
            .OrderBy(v => v.NumeroVersion)
            .ToListAsync(cancellationToken);
    }

    public async Task<PlanEstudiosVersion?> CrearVersionAsync(
         long planEstudiosId,
        DateOnly? fechaVigenciaDesde,
         string? observaciones,
         CancellationToken cancellationToken = default)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync(cancellationToken);

        var connection = _context.Database.GetDbConnection();

        await using var command = connection.CreateCommand();

        command.Transaction =
            _context.Database.CurrentTransaction!.GetDbTransaction();

        command.CommandText = """
        SELECT plan_estudios_id
        FROM plan_estudios
        WHERE plan_estudios_id = @planEstudiosId
        FOR UPDATE
        """;

        var parameter = command.CreateParameter();
        parameter.ParameterName = "planEstudiosId";
        parameter.Value = planEstudiosId;
        command.Parameters.Add(parameter);

        var resultado = await command.ExecuteScalarAsync(cancellationToken);

        if (resultado is null)
            return null;

        var planEstudios = await _context.PlanesEstudios
            .Include(p => p.Versiones)
                .ThenInclude(v => v.Cursos)
                    .ThenInclude(pc => pc.Curso)
            .AsSplitQuery()
            .FirstOrDefaultAsync(
                p => p.PlanEstudiosId == planEstudiosId,
                cancellationToken);

        if (planEstudios is null)
            return null;

        var numeroVersion = planEstudios.Versiones.Count == 0
            ? 1
            : planEstudios.Versiones.Max(v => v.NumeroVersion) + 1;

        var version = planEstudios.CrearVersion(
            numeroVersion,
            fechaVigenciaDesde,
            observaciones);

        await _context.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return version;
    }
    public async Task<int> GuardarCambiosAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }


    public async Task<PlanEstudiosVersion?> ObtenerVersionPorPlanCursoIdAsync(
        long planCursoId,
        CancellationToken cancellationToken = default)
    {
        return await _context.PlanesEstudiosVersiones
            .AsNoTracking()
            .Include(v => v.Cursos)
                .ThenInclude(pc => pc.Curso)
            .AsSplitQuery()
            .FirstOrDefaultAsync(
                v => v.Cursos.Any(pc => pc.PlanCursoId == planCursoId),
                cancellationToken);
    }


    public async Task<PlanEstudiosVersion?> ObtenerVersionPorIdAsync(
    long versionId,
    CancellationToken cancellationToken = default)
    {
        return await _context.PlanesEstudiosVersiones
            .AsNoTracking()
            .Include(v => v.Cursos)
                .ThenInclude(pc => pc.Curso)
            .AsSplitQuery()
            .FirstOrDefaultAsync(
                v => v.PlanEstudiosVersionId == versionId,
                cancellationToken);
    }
}
