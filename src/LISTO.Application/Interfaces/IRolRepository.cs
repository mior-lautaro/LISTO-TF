using System;
using System.Collections.Generic;
using System.Text;
using LISTO.Domain.Entities;

namespace LISTO.Application.Interfaces;

public interface IRolRepository
{
    Task<Rol?> ObtenerPorIdAsync(
        long rolId,
        CancellationToken cancellationToken = default);

    Task<Rol?> ObtenerPorNombreAsync(
        string nombre,
        CancellationToken cancellationToken = default);

    Task<bool> ExisteNombreAsync(
        string nombre,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Rol>> ListarAsync(
        bool? estaActivo = null,
        CancellationToken cancellationToken = default);

    Task AgregarAsync(
        Rol rol,
        CancellationToken cancellationToken = default);

    Task<int> GuardarCambiosAsync(
        CancellationToken cancellationToken = default);
}
