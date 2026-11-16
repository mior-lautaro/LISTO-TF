using LISTO.Domain.Entities;

namespace LISTO.Tests;

public class RolTests
{
    [Fact]
    public void CrearRol_ConDatosValidos_CreaRolActivo()
    {
        var rol = new Rol(
            "Docente",
            "Responsable de la gestión académica.");

        Assert.Equal("Docente", rol.Nombre);
        Assert.Equal(
            "Responsable de la gestión académica.",
            rol.Descripcion);
        Assert.True(rol.EstaActivo);
        Assert.Equal(0, rol.RolId);
    }

    [Fact]
    public void CrearRol_SinNombre_LanzaExcepcion()
    {
        Assert.Throws<ArgumentException>(() =>
            new Rol(""));
    }

    [Fact]
    public void CrearRol_ConEspaciosEnNombre_NormalizaNombre()
    {
        var rol = new Rol("  Docente  ");

        Assert.Equal("Docente", rol.Nombre);
    }

    [Fact]
    public void CrearRol_SinDescripcion_AceptaValorNulo()
    {
        var rol = new Rol("Estudiante");

        Assert.Null(rol.Descripcion);
    }

    [Fact]
    public void DesactivarRol_CambiaEstadoAInactivo()
    {
        var rol = new Rol("Docente");

        rol.Desactivar();

        Assert.False(rol.EstaActivo);
    }

    [Fact]
    public void ActivarRol_CambiaEstadoAActivo()
    {
        var rol = new Rol("Docente");

        rol.Desactivar();
        rol.Activar();

        Assert.True(rol.EstaActivo);
    }

    [Fact]
    public void CrearRol_ConNombreDe100Caracteres_AceptaNombre()
    {
        var nombre = new string('A', 100);

        var rol = new Rol(nombre);

        Assert.Equal(100, rol.Nombre.Length);
    }

    [Fact]
    public void CrearRol_ConNombreDe101Caracteres_LanzaExcepcion()
    {
        var nombre = new string('A', 101);

        Assert.Throws<ArgumentException>(() =>
            new Rol(nombre));
    }

    [Fact]
    public void CrearRol_ConDescripcionDe500Caracteres_AceptaDescripcion()
    {
        var descripcion = new string('A', 500);

        var rol = new Rol("Docente", descripcion);

        Assert.Equal(500, rol.Descripcion!.Length);
    }

    [Fact]
    public void CrearRol_ConDescripcionDe501Caracteres_LanzaExcepcion()
    {
        var descripcion = new string('A', 501);

        Assert.Throws<ArgumentException>(() =>
            new Rol("Docente", descripcion));
    }

    [Fact]
    public void CrearRol_ConDescripcionVacia_ConvierteDescripcionEnNulo()
    {
        var rol = new Rol("Docente", "   ");

        Assert.Null(rol.Descripcion);
    }

    [Fact]
    public void CrearRol_ConEspaciosEnDescripcion_NormalizaDescripcion()
    {
        var rol = new Rol(
            "Docente",
            "  Responsable de la gestión académica.  ");

        Assert.Equal(
            "Responsable de la gestión académica.",
            rol.Descripcion);
    }
}