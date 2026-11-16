using System;
using System.Collections.Generic;
using System.Text;
using LISTO.Application.DTOs;

namespace LISTO.Application.Interfaces;

public interface IUsuarioService
{
    Task<UsuarioDto> RegistrarAsync(
        string email,
        string nombre,
        string apellido,
        string contrasena,
        CancellationToken cancellationToken = default);


    Task<UsuarioDto?> ObtenerPorIdAsync(
        long usuarioId,
        CancellationToken cancellationToken = default);

    Task<UsuarioDto?> ObtenerPorEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task<bool> ModificarDatosPersonalesAsync(
    long usuarioId,
    string nombre,
    string apellido,
    CancellationToken cancellationToken = default);

    Task<IReadOnlyList<UsuarioDto>> ListarAsync(
    bool? estaActivo = null,
    CancellationToken cancellationToken = default);

    Task<bool> DesactivarAsync(
        long usuarioId,
        CancellationToken cancellationToken = default);

    Task<bool> ActivarAsync(
        long usuarioId,
        CancellationToken cancellationToken = default);
}