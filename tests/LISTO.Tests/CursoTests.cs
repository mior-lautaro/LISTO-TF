using System;
using System.Collections.Generic;
using System.Text;
using LISTO.Domain.Entities;
using Xunit;

namespace LISTO.Tests.Domain;

public class CursoTests
{
    [Fact]
    public void Constructor_DatosValidos_CreaCursoActivo()
    {
        // Arrange
        var codigo = "CUR-000001";
        var nombre = "Inglés Técnico";
        var descripcion = "Asignatura de inglés.";

        // Act
        var curso = new Curso(codigo, nombre, descripcion);

        // Assert
        Assert.Equal(codigo, curso.Codigo);
        Assert.Equal(nombre, curso.Nombre);
        Assert.Equal(descripcion, curso.Descripcion);
        Assert.True(curso.EstaActivo);
        Assert.Equal(0, curso.CursoId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_CodigoVacio_LanzaExcepcion(
        string codigo)
    {
        Assert.Throws<ArgumentException>(
            () => new Curso(codigo, "Inglés Técnico"));
    }

    [Fact]
    public void Constructor_CodigoNulo_LanzaExcepcion()
    {
        Assert.Throws<ArgumentException>(
            () => new Curso(null!, "Inglés Técnico"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_NombreVacio_LanzaExcepcion(
        string nombre)
    {
        Assert.Throws<ArgumentException>(
            () => new Curso("CUR-000001", nombre));
    }

    [Fact]
    public void Constructor_NombreNulo_LanzaExcepcion()
    {
        Assert.Throws<ArgumentException>(
            () => new Curso("CUR-000001", null!));
    }

    [Fact]
    public void Constructor_CodigoDemasiadoLargo_LanzaExcepcion()
    {
        var codigo = new string('A', 21);

        Assert.Throws<ArgumentException>(
            () => new Curso(codigo, "Inglés Técnico"));
    }

    [Fact]
    public void Constructor_NombreDemasiadoLargo_LanzaExcepcion()
    {
        var nombre = new string('A', 151);

        Assert.Throws<ArgumentException>(
            () => new Curso("CUR-000001", nombre));
    }

    [Fact]
    public void Constructor_DescripcionDemasiadoLarga_LanzaExcepcion()
    {
        var descripcion = new string('A', 501);

        Assert.Throws<ArgumentException>(
            () => new Curso(
                "CUR-000001",
                "Inglés Técnico",
                descripcion));
    }

    [Fact]
    public void Constructor_NormalizaEspacios()
    {
        var curso = new Curso(
            " CUR-000001 ",
            " Inglés Técnico ",
            " Descripción de prueba ");

        Assert.Equal("CUR-000001", curso.Codigo);
        Assert.Equal("Inglés Técnico", curso.Nombre);
        Assert.Equal("Descripción de prueba", curso.Descripcion);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_DescripcionVacia_AsignaNull(
        string? descripcion)
    {
        var curso = new Curso(
            "CUR-000001",
            "Inglés Técnico",
            descripcion);

        Assert.Null(curso.Descripcion);
    }

    [Fact]
    public void ModificarDatos_DatosValidos_ActualizaDatos()
    {
        // Arrange
        var curso = new Curso(
            "CUR-000001",
            "Inglés Técnico",
            "Descripción original");

        // Act
        curso.ModificarDatos(
            "Inglés Técnico II",
            "Nueva descripción");

        // Assert
        Assert.Equal("CUR-000001", curso.Codigo);
        Assert.Equal("Inglés Técnico II", curso.Nombre);
        Assert.Equal("Nueva descripción", curso.Descripcion);
    }

    [Fact]
    public void ModificarDatos_NombreVacio_LanzaExcepcion()
    {
        var curso = new Curso(
            "CUR-000001",
            "Inglés Técnico");

        Assert.Throws<ArgumentException>(
            () => curso.ModificarDatos("", null));
    }

    [Fact]
    public void ModificarDatos_DescripcionVacia_AsignaNull()
    {
        var curso = new Curso(
            "CUR-000001",
            "Inglés Técnico",
            "Descripción original");

        curso.ModificarDatos("Inglés Técnico", "   ");

        Assert.Null(curso.Descripcion);
    }

    [Fact]
    public void Desactivar_CursoActivo_DesactivaCurso()
    {
        var curso = new Curso(
            "CUR-000001",
            "Inglés Técnico");

        curso.Desactivar();

        Assert.False(curso.EstaActivo);
    }

    [Fact]
    public void Activar_CursoInactivo_ActivaCurso()
    {
        var curso = new Curso(
            "CUR-000001",
            "Inglés Técnico");

        curso.Desactivar();
        curso.Activar();

        Assert.True(curso.EstaActivo);
    }

    [Fact]
    public void Desactivar_CursoYaInactivo_PermaneceInactivo()
    {
        var curso = new Curso(
            "CUR-000001",
            "Inglés Técnico");

        curso.Desactivar();
        curso.Desactivar();

        Assert.False(curso.EstaActivo);
    }

    [Fact]
    public void Activar_CursoYaActivo_PermaneceActivo()
    {
        var curso = new Curso(
            "CUR-000001",
            "Inglés Técnico");

        curso.Activar();

        Assert.True(curso.EstaActivo);
    }
}
