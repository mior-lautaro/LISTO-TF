using System;
using System.Collections.Generic;
using System.Text;
using LISTO.Application.DTOs;
using LISTO.Application.Interfaces;
using LISTO.Application.Services;
using LISTO.Domain.Entities;
using LISTO.Application.Exceptions;

namespace LISTO.Tests;

public class UsuarioServiceTests
{
    private readonly RepositorioFalso _repositorio = new();
    private readonly HasherFalso _hasher = new();
    private readonly UsuarioService _servicio;

    public UsuarioServiceTests()
    {
        _servicio = new UsuarioService(_repositorio, _hasher);
    }

    [Fact]
    public async Task RegistrarAsync_DebeCrearUsuarioYDevolverDto()
    {
        var resultado = await _servicio.RegistrarAsync(
            "  USUARIO@EJEMPLO.COM ",
            " Ana ",
            " Pérez ",
            "Clave123!");

        Assert.Equal(1, resultado.UsuarioId);
        Assert.Equal("usuario@ejemplo.com", resultado.Email);
        Assert.Equal("Ana", resultado.Nombre);
        Assert.Equal("Pérez", resultado.Apellido);
        Assert.True(resultado.EstaActivo);
        Assert.Equal("HASH_SIMULADO", _repositorio.UsuarioGuardado!.HashContrasena);
        Assert.Equal(1, _repositorio.CantidadGuardados);
        Assert.Equal("usuario@ejemplo.com", _repositorio.UltimoEmailConsultado);
        Assert.True(_hasher.FueInvocado);
    }

    [Fact]
    public async Task RegistrarAsync_DebeRechazarEmailDuplicado()
    {
        _repositorio.EmailExistente = true;

        await Assert.ThrowsAsync<EmailDuplicadoException>(() =>
            _servicio.RegistrarAsync(
                "usuario@ejemplo.com",
                "Ana",
                "Pérez",
                "Clave123!"));

        Assert.Equal(0, _repositorio.CantidadGuardados);
        Assert.False(_hasher.FueInvocado);
    }

    [Theory]
    [InlineData("", "Ana", "Pérez", "Clave123!")]
    [InlineData("   ", "Ana", "Pérez", "Clave123!")]
    [InlineData("usuario@ejemplo.com", "", "Pérez", "Clave123!")]
    [InlineData("usuario@ejemplo.com", "   ", "Pérez", "Clave123!")]
    [InlineData("usuario@ejemplo.com", "Ana", "", "Clave123!")]
    [InlineData("usuario@ejemplo.com", "Ana", "   ", "Clave123!")]
    [InlineData("usuario@ejemplo.com", "Ana", "Pérez", "")]
    [InlineData("usuario@ejemplo.com", "Ana", "Pérez", "   ")]
    public async Task RegistrarAsync_DebeRechazarCamposObligatoriosVacios(
        string email,
        string nombre,
        string apellido,
        string contrasena)
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _servicio.RegistrarAsync(
                email,
                nombre,
                apellido,
                contrasena));

        Assert.Equal(0, _repositorio.CantidadConsultasEmail);
        Assert.Equal(0, _repositorio.CantidadGuardados);
        Assert.False(_hasher.FueInvocado);
    }

    [Fact]
    public async Task RegistrarAsync_DebeRechazarEmailDemasiadoLargo()
    {
        var email = new string('a', 245) + "@ejemplo.com";

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _servicio.RegistrarAsync(
                email,
                "Ana",
                "Pérez",
                "Clave123!"));

        Assert.Equal(0, _repositorio.CantidadConsultasEmail);
        Assert.False(_hasher.FueInvocado);
    }

    [Fact]
    public async Task RegistrarAsync_DebeRechazarNombreDemasiadoLargo()
    {
        var nombre = new string('A', 101);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _servicio.RegistrarAsync(
                "usuario@ejemplo.com",
                nombre,
                "Pérez",
                "Clave123!"));

        Assert.Equal(0, _repositorio.CantidadConsultasEmail);
        Assert.False(_hasher.FueInvocado);
    }

    [Fact]
    public async Task RegistrarAsync_DebeRechazarApellidoDemasiadoLargo()
    {
        var apellido = new string('A', 101);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _servicio.RegistrarAsync(
                "usuario@ejemplo.com",
                "Ana",
                apellido,
                "Clave123!"));

        Assert.Equal(0, _repositorio.CantidadConsultasEmail);
        Assert.False(_hasher.FueInvocado);
    }
    [Theory]
    [InlineData("Abc123!", 7)]
    [InlineData(
    "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA",
    129)]
    public async Task RegistrarAsync_DebeRechazarContrasenaFueraDeLimites(
    string contrasena,
    int longitudEsperada)
    {
        Assert.Equal(longitudEsperada, contrasena.Length);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _servicio.RegistrarAsync(
                "usuario@ejemplo.com",
                "Ana",
                "Pérez",
                contrasena));

        Assert.Equal(0, _repositorio.CantidadConsultasEmail);
        Assert.Equal(0, _repositorio.CantidadGuardados);
        Assert.False(_hasher.FueInvocado);
    }
    [Theory]
    [InlineData(8)]
    [InlineData(128)]
    public async Task RegistrarAsync_DebeAceptarContrasenaEnLosLimites(
    int longitud)
    {
        var contrasena = new string('A', longitud);

        var resultado = await _servicio.RegistrarAsync(
            "usuario@ejemplo.com",
            "Ana",
            "Pérez",
            contrasena);

        Assert.NotNull(resultado);
        Assert.True(_hasher.FueInvocado);
        Assert.Equal(1, _repositorio.CantidadConsultasEmail);
        Assert.Equal(1, _repositorio.CantidadGuardados);
    }

    [Fact]
    public async Task RegistrarAsync_DebeConsultarEmailNormalizado()
    {
        await _servicio.RegistrarAsync(
            "  USUARIO@EJEMPLO.COM ",
            "Ana",
            "Pérez",
            "Clave123!");

        Assert.Equal(
            "usuario@ejemplo.com",
            _repositorio.UltimoEmailConsultado);
    }

    [Fact]
    public async Task ObtenerPorEmailAsync_DebeConsultarEmailNormalizado()
    {
        var resultado = await _servicio.ObtenerPorEmailAsync(
            "  USUARIO@EJEMPLO.COM ");

        Assert.Null(resultado);

        Assert.Equal(
            "usuario@ejemplo.com",
            _repositorio.UltimoEmailConsultado);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task ObtenerPorEmailAsync_DebeRechazarEmailVacio(
        string email)
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _servicio.ObtenerPorEmailAsync(email));

        Assert.Equal(0, _repositorio.CantidadConsultasEmail);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_DebeDevolverDtoCuandoExiste()
    {
        var usuario = new Usuario(
            "usuario@ejemplo.com",
            "Ana",
            "Pérez",
            "HASH_SIMULADO");

        _repositorio.UsuarioExistente = usuario;

        var resultado = await _servicio.ObtenerPorIdAsync(1);

        Assert.NotNull(resultado);
        Assert.Equal(usuario.Email, resultado.Email);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_DebeDevolverNullCuandoNoExiste()
    {
        var resultado = await _servicio.ObtenerPorIdAsync(999);

        Assert.Null(resultado);
    }

    private sealed class HasherFalso : IPasswordHasher
    {
        public bool FueInvocado { get; private set; }

        public string Hash(string contrasena)
        {
            FueInvocado = true;
            return "HASH_SIMULADO";
        }

        public bool Verificar(string contrasena, string hashContrasena)
        {
            return hashContrasena == "HASH_SIMULADO";
        }
    }

    private sealed class RepositorioFalso : IUsuarioRepository
    {
        public Usuario? UsuarioGuardado { get; private set; }
        public Usuario? UsuarioExistente { get; set; }
        public bool EmailExistente { get; set; }
        public int CantidadGuardados { get; private set; }
        public int CantidadConsultasEmail { get; private set; }
        public string? UltimoEmailConsultado { get; private set; }

        public Task<Usuario?> ObtenerPorIdAsync(
            long usuarioId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(UsuarioExistente);
        }

        public Task<Usuario?> ObtenerPorEmailAsync(
            string email,
            CancellationToken cancellationToken = default)
        {
            CantidadConsultasEmail++;
            UltimoEmailConsultado = email;

            return Task.FromResult<Usuario?>(null);
        }

        public Task<IReadOnlyList<Usuario>> ListarAsync(
    bool? estaActivo = null,
    CancellationToken cancellationToken = default)
        {
            IReadOnlyList<Usuario> usuarios = UsuarioExistente is null
                ? Array.Empty<Usuario>()
                : new[] { UsuarioExistente };

            if (estaActivo.HasValue)
            {
                usuarios = usuarios
                    .Where(u => u.EstaActivo == estaActivo.Value)
                    .ToList();
            }

            return Task.FromResult(usuarios);
        }

        public Task<bool> ExisteEmailAsync(
            string email,
            CancellationToken cancellationToken = default)
        {
            CantidadConsultasEmail++;
            UltimoEmailConsultado = email;

            return Task.FromResult(EmailExistente);
        }

        public Task AgregarAsync(
            Usuario usuario,
            CancellationToken cancellationToken = default)
        {
            UsuarioGuardado = usuario;
            return Task.CompletedTask;
        }

        public Task<int> GuardarCambiosAsync(
            CancellationToken cancellationToken = default)
        {
            CantidadGuardados++;

            if (UsuarioGuardado is not null)
            {
                // Simula la generación del identificador por la base de datos.
                typeof(Usuario)
                    .GetProperty(nameof(Usuario.UsuarioId))!
                    .SetValue(UsuarioGuardado, 1L);
            }

            return Task.FromResult(1);
        }
    }
}
