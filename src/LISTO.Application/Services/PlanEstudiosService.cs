using LISTO.Application.DTOs;
using LISTO.Application.Enums;
using LISTO.Application.Interfaces;
using LISTO.Domain.Entities;

namespace LISTO.Application.Services;

public class PlanEstudiosService : IPlanEstudiosService
{
    private readonly IPlanEstudiosRepository _planEstudiosRepository;
    private readonly ICodigoInstitucionalGenerator _codigoGenerator;

    public PlanEstudiosService(
        IPlanEstudiosRepository planEstudiosRepository,
        ICodigoInstitucionalGenerator codigoGenerator)
    {
        ArgumentNullException.ThrowIfNull(planEstudiosRepository);
        ArgumentNullException.ThrowIfNull(codigoGenerator);

        _planEstudiosRepository = planEstudiosRepository;
        _codigoGenerator = codigoGenerator;
    }

    public async Task<PlanEstudiosDto> CrearAsync(
        string nombre,
        string? descripcion = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nombre);

        var nombreNormalizado = nombre.Trim();
        var descripcionNormalizada = NormalizarDescripcion(descripcion);

        ValidarDatos(nombreNormalizado, descripcionNormalizada);

        var codigo = await _codigoGenerator.GenerarAsync(
            TipoCodigoInstitucional.PlanEstudios,
            cancellationToken);

        var planEstudios = new PlanEstudios(
            codigo,
            nombreNormalizado,
            descripcionNormalizada);

        await _planEstudiosRepository.AgregarAsync(
            planEstudios,
            cancellationToken);

        await _planEstudiosRepository.GuardarCambiosAsync(
            cancellationToken);

        return MapearADto(planEstudios);
    }

    public async Task<PlanEstudiosDto?> ObtenerPorIdAsync(
        long planEstudiosId,
        CancellationToken cancellationToken = default)
    {
        var planEstudios =
            await _planEstudiosRepository.ObtenerPorIdAsync(
                planEstudiosId,
                cancellationToken);

        return planEstudios is null
            ? null
            : MapearADto(planEstudios);
    }

    public async Task<PlanEstudiosDto?> ObtenerPorCodigoAsync(
        string codigo,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(codigo);

        var planEstudios =
            await _planEstudiosRepository.ObtenerPorCodigoAsync(
                codigo.Trim(),
                cancellationToken);

        return planEstudios is null
            ? null
            : MapearADto(planEstudios);
    }

    public async Task<IReadOnlyList<PlanEstudiosDto>> ListarAsync(
        bool? estaActivo = null,
        CancellationToken cancellationToken = default)
    {
        var planes = await _planEstudiosRepository.ListarAsync(
            estaActivo,
            cancellationToken);

        return planes
            .Select(MapearADto)
            .ToList();
    }

    public async Task<bool> ModificarDatosAsync(
        long planEstudiosId,
        string nombre,
        string? descripcion,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nombre);

        var nombreNormalizado = nombre.Trim();
        var descripcionNormalizada = NormalizarDescripcion(descripcion);

        ValidarDatos(nombreNormalizado, descripcionNormalizada);

        var planEstudios =
            await _planEstudiosRepository.ObtenerPorIdAsync(
                planEstudiosId,
                cancellationToken);

        if (planEstudios is null)
            return false;

        planEstudios.ModificarDatos(
            nombreNormalizado,
            descripcionNormalizada);

        await _planEstudiosRepository.GuardarCambiosAsync(
            cancellationToken);

        return true;
    }

    public async Task<bool> DesactivarAsync(
        long planEstudiosId,
        CancellationToken cancellationToken = default)
    {
        var planEstudios =
            await _planEstudiosRepository.ObtenerPorIdAsync(
                planEstudiosId,
                cancellationToken);

        if (planEstudios is null)
            return false;

        if (!planEstudios.EstaActivo)
            return true;

        planEstudios.Desactivar();

        await _planEstudiosRepository.GuardarCambiosAsync(
            cancellationToken);

        return true;
    }

    public async Task<bool> ActivarAsync(
        long planEstudiosId,
        CancellationToken cancellationToken = default)
    {
        var planEstudios =
            await _planEstudiosRepository.ObtenerPorIdAsync(
                planEstudiosId,
                cancellationToken);

        if (planEstudios is null)
            return false;

        if (planEstudios.EstaActivo)
            return true;

        planEstudios.Activar();

        await _planEstudiosRepository.GuardarCambiosAsync(
            cancellationToken);

        return true;
    }

    private static void ValidarDatos(
        string nombre,
        string? descripcion)
    {
        if (nombre.Length > 150)
        {
            throw new ArgumentException(
                "El nombre del plan no puede superar los 150 caracteres.",
                nameof(nombre));
        }

        if (descripcion?.Length > 500)
        {
            throw new ArgumentException(
                "La descripción del plan no puede superar los 500 caracteres.",
                nameof(descripcion));
        }
    }

    private static string? NormalizarDescripcion(string? descripcion)
    {
        return string.IsNullOrWhiteSpace(descripcion)
            ? null
            : descripcion.Trim();
    }

    private static PlanEstudiosDto MapearADto(
        PlanEstudios planEstudios)
    {
        return new PlanEstudiosDto
        {
            PlanEstudiosId = planEstudios.PlanEstudiosId,
            Codigo = planEstudios.Codigo,
            Nombre = planEstudios.Nombre,
            Descripcion = planEstudios.Descripcion,
            EstaActivo = planEstudios.EstaActivo
        };
    }
}