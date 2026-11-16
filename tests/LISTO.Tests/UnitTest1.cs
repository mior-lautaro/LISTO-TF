using LISTO.Domain.Entities;

namespace LISTO.Tests;

public class UsuarioTests
{
    [Fact]
    public void CrearUsuario_ConDatosValidos_CreaUsuarioActivo()
    {
        // Arrange
        var fechaAntes = DateTime.UtcNow;

        // Act
        var usuario = new Usuario(
            "usuario@ejemplo.com",
            "Juan",
            "Pérez",
            "hash-seguro");

        var fechaDespues = DateTime.UtcNow;

        // Assert
        Assert.True(usuario.UsuarioId == 0);
        Assert.Equal("usuario@ejemplo.com", usuario.Email);
        Assert.Equal("Juan", usuario.Nombre);
        Assert.Equal("Pérez", usuario.Apellido);
        Assert.Equal("hash-seguro", usuario.HashContrasena);
        Assert.True(usuario.EstaActivo);
        Assert.InRange(usuario.FechaAlta, fechaAntes, fechaDespues);
        Assert.Equal(DateTimeKind.Utc, usuario.FechaAlta.Kind);
    }

    [Fact]
    public void CrearUsuario_ConCorreoEnMayusculasYEspacios_NormalizaCorreo()
    {
        // Act
        var usuario = new Usuario(
            "  Usuario@Ejemplo.COM  ",
            "Juan",
            "Pérez",
            "hash-seguro");

        // Assert
        Assert.Equal("usuario@ejemplo.com", usuario.Email);
    }

    [Fact]
    public void CrearUsuario_SinCorreo_LanzaExcepcion()
    {
        Assert.Throws<ArgumentException>(() =>
            new Usuario("", "Juan", "Pérez", "hash-seguro"));
    }

    [Fact]
    public void CrearUsuario_SinNombre_LanzaExcepcion()
    {
        Assert.Throws<ArgumentException>(() =>
            new Usuario("usuario@ejemplo.com", "", "Pérez", "hash-seguro"));
    }

    [Fact]
    public void CrearUsuario_SinApellido_LanzaExcepcion()
    {
        Assert.Throws<ArgumentException>(() =>
            new Usuario("usuario@ejemplo.com", "Juan", "", "hash-seguro"));
    }

    [Fact]
    public void CrearUsuario_SinHashContrasena_LanzaExcepcion()
    {
        Assert.Throws<ArgumentException>(() =>
            new Usuario("usuario@ejemplo.com", "Juan", "Pérez", ""));
    }

    [Fact]
    public void CrearUsuario_ConEmailDe254Caracteres_AceptaEmail()
    {
        var email = new string('a', 254);

        var usuario = new Usuario(
            email, "Juan", "Pérez", "hash-seguro");

        Assert.Equal(254, usuario.Email.Length);
    }

    [Fact]
    public void CrearUsuario_ConEmailDe255Caracteres_LanzaExcepcion()
    {
        var email = new string('a', 255);

        Assert.Throws<ArgumentException>(() =>
            new Usuario(email, "Juan", "Pérez", "hash-seguro"));
    }

    [Fact]
    public void CrearUsuario_ConNombreDe100Caracteres_AceptaNombre()
    {
        var nombre = new string('A', 100);

        var usuario = new Usuario(
            "usuario@ejemplo.com", nombre, "Pérez", "hash-seguro");

        Assert.Equal(100, usuario.Nombre.Length);
    }

    [Fact]
    public void CrearUsuario_ConNombreDe101Caracteres_LanzaExcepcion()
    {
        var nombre = new string('A', 101);

        Assert.Throws<ArgumentException>(() =>
            new Usuario("usuario@ejemplo.com", nombre, "Pérez", "hash-seguro"));
    }

    [Fact]
    public void CrearUsuario_ConApellidoDe100Caracteres_AceptaApellido()
    {
        var apellido = new string('A', 100);

        var usuario = new Usuario(
            "usuario@ejemplo.com", "Juan", apellido, "hash-seguro");

        Assert.Equal(100, usuario.Apellido.Length);
    }

    [Fact]
    public void CrearUsuario_ConApellidoDe101Caracteres_LanzaExcepcion()
    {
        var apellido = new string('A', 101);

        Assert.Throws<ArgumentException>(() =>
            new Usuario("usuario@ejemplo.com", "Juan", apellido, "hash-seguro"));
    }

    [Fact]
    public void CrearUsuario_ConHashDe500Caracteres_AceptaHash()
    {
        var hash = new string('h', 500);

        var usuario = new Usuario(
            "usuario@ejemplo.com", "Juan", "Pérez", hash);

        Assert.Equal(500, usuario.HashContrasena.Length);
    }

    [Fact]
    public void CrearUsuario_ConHashDe501Caracteres_LanzaExcepcion()
    {
        var hash = new string('h', 501);

        Assert.Throws<ArgumentException>(() =>
            new Usuario("usuario@ejemplo.com", "Juan", "Pérez", hash));
    }
}
