using System;
using System.Collections.Generic;
using System.Text;
using LISTO.Application.DTOs;

namespace LISTO.Application.Interfaces;

public interface IRolService
{
    Task<RolDto> CrearAsync(
        string nombre,
        string? descripcion = null,
        CancellationToken cancellationToken = default);

    Task<RolDto?> ObtenerPorIdAsync(
        long rolId,
        CancellationToken cancellationToken = default);

    Task<RolDto?> ObtenerPorNombreAsync(
        string nombre,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RolDto>> ListarAsync(
        bool? estaActivo = null,
        CancellationToken cancellationToken = default);

    Task<bool> DesactivarAsync(
        long rolId,
        CancellationToken cancellationToken = default);

    Task<bool> ActivarAsync(
        long rolId,
        CancellationToken cancellationToken = default);
}
