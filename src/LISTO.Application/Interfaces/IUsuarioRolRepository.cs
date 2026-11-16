using System;
using System.Collections.Generic;
using System.Text;
using LISTO.Domain.Entities;

namespace LISTO.Application.Interfaces;

public interface IUsuarioRolRepository
{
    Task<UsuarioRol?> ObtenerPorIdAsync(
        long usuarioRolId,
        CancellationToken cancellationToken = default);

    Task<UsuarioRol?> ObtenerAsignacionVigenteAsync(
        long usuarioId,
        long rolId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<UsuarioRol>> ListarPorUsuarioAsync(
        long usuarioId,
        bool? soloVigentes = null,
        CancellationToken cancellationToken = default);

    Task AgregarAsync(
        UsuarioRol usuarioRol,
        CancellationToken cancellationToken = default);

    Task<int> GuardarCambiosAsync(
        CancellationToken cancellationToken = default);
}