using System;
using System.Collections.Generic;
using System.Text;
using LISTO.Application.DTOs;

namespace LISTO.Application.Interfaces;

public interface ICursoService
{
    Task<CursoDto> CrearAsync(
        string nombre,
        string? descripcion = null,
        CancellationToken cancellationToken = default);

    Task<CursoDto?> ObtenerPorIdAsync(
        long cursoId,
        CancellationToken cancellationToken = default);

    Task<CursoDto?> ObtenerPorCodigoAsync(
        string codigo,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CursoDto>> ListarAsync(
        bool? estaActivo = null,
        CancellationToken cancellationToken = default);

    Task<bool> ModificarDatosAsync(
        long cursoId,
        string nombre,
        string? descripcion,
        CancellationToken cancellationToken = default);

    Task<bool> DesactivarAsync(
        long cursoId,
        CancellationToken cancellationToken = default);

    Task<bool> ActivarAsync(
        long cursoId,
        CancellationToken cancellationToken = default);
}
