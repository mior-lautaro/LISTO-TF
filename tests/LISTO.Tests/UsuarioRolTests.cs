using LISTO.Domain.Entities;

namespace LISTO.Tests;

public class UsuarioRolTests
{
    [Fact]
    public void CrearAsignacion_ConUsuarioYRolValidos_CreaVinculo()
    {
        var usuario = new Usuario(
            "usuario@ejemplo.com",
            "Juan",
            "Pérez",
            "hash-seguro");

        var rol = new Rol("Docente");

        var asignacion = new UsuarioRol(usuario, rol);

        Assert.Same(usuario, asignacion.Usuario);
        Assert.Same(rol, asignacion.Rol);
    }

    [Fact]
    public void CrearAsignacion_ConRolInactivo_LanzaExcepcion()
    {
        var usuario = new Usuario(
            "usuario@ejemplo.com",
            "Juan",
            "Pérez",
            "hash-seguro");

        var rol = new Rol("Docente");
        rol.Desactivar();

        Assert.Throws<InvalidOperationException>(() =>
            new UsuarioRol(usuario, rol));
    }

    [Fact]
    public void CrearAsignacion_SinUsuario_LanzaExcepcion()
    {
        var rol = new Rol("Docente");

        Assert.Throws<ArgumentNullException>(() =>
            new UsuarioRol(null!, rol));
    }

    [Fact]
    public void CrearAsignacion_SinRol_LanzaExcepcion()
    {
        var usuario = new Usuario(
            "usuario@ejemplo.com",
            "Juan",
            "Pérez",
            "hash-seguro");

        Assert.Throws<ArgumentNullException>(() =>
            new UsuarioRol(usuario, null!));
    }
    [Fact]
    public void RevocarAsignacion_Vigente_RegistraFechaRevocacion()
    {
        var usuario = new Usuario(
            "usuario@ejemplo.com",
            "Juan",
            "Pérez",
            "hash-seguro");

        var rol = new Rol("Docente");
        var asignacion = new UsuarioRol(usuario, rol);

        var fechaAntes = DateTime.UtcNow;

        asignacion.Revocar();

        var fechaDespues = DateTime.UtcNow;

        Assert.NotNull(asignacion.FechaRevocacion);
        Assert.InRange(
            asignacion.FechaRevocacion.Value,
            fechaAntes,
            fechaDespues);

        Assert.Equal(
            DateTimeKind.Utc,
            asignacion.FechaRevocacion.Value.Kind);

        Assert.True(
            asignacion.FechaRevocacion >= asignacion.FechaAsignacion);
    }

    [Fact]
    public void RevocarAsignacion_YaRevocada_LanzaExcepcion()
    {
        var usuario = new Usuario(
            "usuario@ejemplo.com",
            "Juan",
            "Pérez",
            "hash-seguro");

        var rol = new Rol("Docente");
        var asignacion = new UsuarioRol(usuario, rol);

        asignacion.Revocar();

        Assert.Throws<InvalidOperationException>(() =>
            asignacion.Revocar());
    }

    [Fact]
    public void CrearNuevaAsignacion_DespuesDeRevocar_PermiteNuevoCiclo()
    {
        var usuario = new Usuario(
            "usuario@ejemplo.com",
            "Juan",
            "Pérez",
            "hash-seguro");

        var rol = new Rol("Docente");

        var asignacionAnterior = new UsuarioRol(usuario, rol);

        asignacionAnterior.Revocar();

        var nuevaAsignacion = new UsuarioRol(usuario, rol);

        Assert.NotNull(asignacionAnterior.FechaRevocacion);
        Assert.Null(nuevaAsignacion.FechaRevocacion);

        Assert.NotSame(asignacionAnterior, nuevaAsignacion);
        Assert.Same(usuario, nuevaAsignacion.Usuario);
        Assert.Same(rol, nuevaAsignacion.Rol);
    }

    [Fact]
    public void CrearAsignacion_ConUsuarioInactivo_LanzaExcepcion()
    {
        var usuario = new Usuario(
            "usuario@ejemplo.com",
            "Juan",
            "Pérez",
            "hash-seguro");

        var rol = new Rol("Docente");

        usuario.Desactivar();

        Assert.Throws<InvalidOperationException>(() =>
            new UsuarioRol(usuario, rol));
    }
}