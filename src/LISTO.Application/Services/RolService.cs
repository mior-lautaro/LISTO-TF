using System;
using System.Collections.Generic;
using System.Text;
using LISTO.Application.DTOs;
using LISTO.Application.Exceptions;
using LISTO.Application.Interfaces;
using LISTO.Domain.Entities;

namespace LISTO.Application.Services;

public class RolService : IRolService
{
    private readonly IRolRepository _rolRepository;

    public RolService(IRolRepository rolRepository)
    {
        ArgumentNullException.ThrowIfNull(rolRepository);
        _rolRepository = rolRepository;
    }

    public async Task<RolDto> CrearAsync(
        string nombre,
        string? descripcion = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nombre);

        var nombreNormalizado = nombre.Trim();
        var descripcionNormalizada = descripcion?.Trim();

        if (nombreNormalizado.Length > 100)
        {
            throw new ArgumentException(
                "El nombre del rol no puede superar los 100 caracteres.",
                nameof(nombre));
        }

        if (descripcionNormalizada?.Length > 500)
        {
            throw new ArgumentException(
                "La descripción del rol no puede superar los 500 caracteres.",
                nameof(descripcion));
        }

        if (await _rolRepository.ExisteNombreAsync(
            nombreNormalizado, cancellationToken))
        {
            throw new NombreRolDuplicadoException(nombreNormalizado);
        }

        var rol = new Rol(
            nombreNormalizado,
            descripcionNormalizada);

        await _rolRepository.AgregarAsync(
            rol, cancellationToken);

        await _rolRepository.GuardarCambiosAsync(
            cancellationToken);

        return MapearADto(rol);
    }

    public async Task<RolDto?> ObtenerPorIdAsync(
        long rolId,
        CancellationToken cancellationToken = default)
    {
        var rol = await _rolRepository.ObtenerPorIdAsync(
            rolId, cancellationToken);

        return rol is null ? null : MapearADto(rol);
    }

    public async Task<RolDto?> ObtenerPorNombreAsync(
        string nombre,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nombre);

        var rol = await _rolRepository.ObtenerPorNombreAsync(
            nombre.Trim(), cancellationToken);

        return rol is null ? null : MapearADto(rol);
    }

    public async Task<IReadOnlyList<RolDto>> ListarAsync(
        bool? estaActivo = null,
        CancellationToken cancellationToken = default)
    {
        var roles = await _rolRepository.ListarAsync(
            estaActivo, cancellationToken);

        return roles
            .Select(MapearADto)
            .ToList();
    }

    public async Task<bool> DesactivarAsync(
        long rolId,
        CancellationToken cancellationToken = default)
    {
        var rol = await _rolRepository.ObtenerPorIdAsync(
            rolId, cancellationToken);

        if (rol is null)
            return false;

        if (!rol.EstaActivo)
            return true;

        rol.Desactivar();

        await _rolRepository.GuardarCambiosAsync(
            cancellationToken);

        return true;
    }

    public async Task<bool> ActivarAsync(
        long rolId,
        CancellationToken cancellationToken = default)
    {
        var rol = await _rolRepository.ObtenerPorIdAsync(
            rolId, cancellationToken);

        if (rol is null)
            return false;

        if (rol.EstaActivo)
            return true;

        rol.Activar();

        await _rolRepository.GuardarCambiosAsync(
            cancellationToken);

        return true;
    }

    private static RolDto MapearADto(Rol rol)
    {
        return new RolDto
        {
            RolId = rol.RolId,
            Nombre = rol.Nombre,
            Descripcion = rol.Descripcion,
            EstaActivo = rol.EstaActivo
        };
    }
}