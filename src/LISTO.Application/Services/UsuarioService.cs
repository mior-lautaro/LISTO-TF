using System;
using System.Collections.Generic;
using System.Text;
using LISTO.Application.DTOs;
using LISTO.Application.Interfaces;
using LISTO.Domain.Entities;
using LISTO.Application.Exceptions;

namespace LISTO.Application.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher _passwordHasher;

    public UsuarioService(
        IUsuarioRepository usuarioRepository,
        IPasswordHasher passwordHasher)
    {
        ArgumentNullException.ThrowIfNull(usuarioRepository);
        ArgumentNullException.ThrowIfNull(passwordHasher);

        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<UsuarioDto> RegistrarAsync(
        string email,
        string nombre,
        string apellido,
        string contrasena,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(nombre);
        ArgumentException.ThrowIfNullOrWhiteSpace(apellido);
        ArgumentException.ThrowIfNullOrWhiteSpace(contrasena);

        var emailNormalizado = email.Trim().ToLowerInvariant();
        var nombreNormalizado = nombre.Trim();
        var apellidoNormalizado = apellido.Trim();

        if (emailNormalizado.Length > 254)
            throw new ArgumentException(
                "El correo electrónico no puede superar los 254 caracteres.",
                nameof(email));

        if (nombreNormalizado.Length > 100)
            throw new ArgumentException(
                "El nombre no puede superar los 100 caracteres.",
                nameof(nombre));

        if (apellidoNormalizado.Length > 100)
            throw new ArgumentException(
                "El apellido no puede superar los 100 caracteres.",
                nameof(apellido));

        if (contrasena.Length < 8 || contrasena.Length > 128)
        {
            throw new ArgumentException(
                "La contraseña debe tener entre 8 y 128 caracteres.",
                nameof(contrasena));
        }

        if (await _usuarioRepository.ExisteEmailAsync(emailNormalizado, cancellationToken))
        {
            throw new EmailDuplicadoException(emailNormalizado);
        }

        var hashContrasena = _passwordHasher.Hash(contrasena);

        var usuario = new Usuario(
            emailNormalizado,
            nombreNormalizado,
            apellidoNormalizado,
            hashContrasena);

        await _usuarioRepository.AgregarAsync(
            usuario, cancellationToken);

        await _usuarioRepository.GuardarCambiosAsync(
            cancellationToken);

        return MapearADto(usuario);
    }

    public async Task<UsuarioDto?> ObtenerPorIdAsync(
        long usuarioId,
        CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarioRepository.ObtenerPorIdAsync(
            usuarioId, cancellationToken);

        return usuario is null ? null : MapearADto(usuario);
    }

    public async Task<UsuarioDto?> ObtenerPorEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        var emailNormalizado = email.Trim().ToLowerInvariant();

        var usuario = await _usuarioRepository.ObtenerPorEmailAsync(
            emailNormalizado, cancellationToken);

        return usuario is null ? null : MapearADto(usuario);
    }

    public async Task<bool> ModificarDatosPersonalesAsync(
    long usuarioId,
    string nombre,
    string apellido,
    CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarioRepository.ObtenerPorIdAsync(
            usuarioId,
            cancellationToken);

        if (usuario is null)
            return false;

        usuario.ModificarDatosPersonales(nombre, apellido);

        await _usuarioRepository.GuardarCambiosAsync(
            cancellationToken);

        return true;
    }

    public async Task<IReadOnlyList<UsuarioDto>> ListarAsync(
    bool? estaActivo = null,
    CancellationToken cancellationToken = default)
    {
        var usuarios = await _usuarioRepository.ListarAsync(
            estaActivo,
            cancellationToken);

        return usuarios
            .Select(MapearADto)
            .ToList();
    }
    public async Task<bool> DesactivarAsync(
        long usuarioId,
        CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarioRepository.ObtenerPorIdAsync(
            usuarioId, cancellationToken);

        if (usuario is null)
            return false;

        if (!usuario.EstaActivo)
            return true;

        usuario.Desactivar();

        await _usuarioRepository.GuardarCambiosAsync(
            cancellationToken);

        return true;
    }

    public async Task<bool> ActivarAsync(
        long usuarioId,
        CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarioRepository.ObtenerPorIdAsync(
            usuarioId, cancellationToken);

        if (usuario is null)
            return false;

        if (usuario.EstaActivo)
            return true;

        usuario.Activar();

        await _usuarioRepository.GuardarCambiosAsync(
            cancellationToken);

        return true;
    }

    private static UsuarioDto MapearADto(Usuario usuario)
    {
        return new UsuarioDto
        {
            UsuarioId = usuario.UsuarioId,
            Email = usuario.Email,
            Nombre = usuario.Nombre,
            Apellido = usuario.Apellido,
            EstaActivo = usuario.EstaActivo,
            FechaAlta = usuario.FechaAlta
        };
    }
}