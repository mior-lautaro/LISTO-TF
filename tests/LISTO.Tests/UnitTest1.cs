
namespace LISTO.Tests;

public class UnitTest1
{
    [Fact]
    public void LaAplicacionPuedeEjecutarUnaPrueba()
    {
        // Arrange
        var esperado = 2;

        // Act
        var resultado = 1 + 1;

        // Assert
        Assert.Equal(esperado, resultado);
    }
}
