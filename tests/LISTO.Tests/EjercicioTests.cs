using LISTO.Domain.Entities;
using LISTO.Domain.Enums;
using Xunit;

namespace LISTO.Tests;

public class EjercicioTests
{
    [Fact]
    public void Crear_ConCursoActivo_CreaEjercicioActivo()
    {
        // Arrange
        var curso = new Curso("CUR-TEST", "Inglés");

        // Act
        var ejercicio = new Ejercicio(curso);

        // Assert
        Assert.Equal(curso.CursoId, ejercicio.CursoId);
        Assert.Same(curso, ejercicio.Curso);
        Assert.Equal(EstadoEjercicio.Activo, ejercicio.Estado);
        Assert.True(ejercicio.FechaCreacion <= DateTime.UtcNow);
    }

    [Fact]
    public void Crear_ConCursoNulo_LanzaExcepcion()
    {
        Assert.Throws<ArgumentNullException>(
            () => new Ejercicio(null!));
    }

    [Fact]
    public void Crear_ConCursoInactivo_LanzaExcepcion()
    {
        // Arrange
        var curso = new Curso("CUR-TEST", "Inglés");
        curso.Desactivar();

        // Act y Assert
        Assert.Throws<InvalidOperationException>(
            () => new Ejercicio(curso));
    }

    [Fact]
    public void Retirar_CambiaEstadoARetirado()
    {
        // Arrange
        var curso = new Curso("CUR-TEST", "Inglés");
        var ejercicio = new Ejercicio(curso);

        // Act
        ejercicio.Retirar();

        // Assert
        Assert.Equal(EstadoEjercicio.Retirado, ejercicio.Estado);
    }

    [Fact]
    public void Retirar_EjercicioYaRetirado_NoProduceError()
    {
        // Arrange
        var curso = new Curso("CUR-TEST", "Inglés");
        var ejercicio = new Ejercicio(curso);

        // Act
        ejercicio.Retirar();
        ejercicio.Retirar();

        // Assert
        Assert.Equal(EstadoEjercicio.Retirado, ejercicio.Estado);
    }
}