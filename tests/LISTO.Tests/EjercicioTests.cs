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


    [Fact]
    public void CrearVersion_PrimeraVersion_NumeroUno()
    {
        var ejercicio = new Ejercicio(
            new Curso("CUR-000001", "Matemática"));

        var version = ejercicio.CrearVersion();

        Assert.Equal(1, version.NumeroVersion);
        Assert.Single(ejercicio.Versiones);
    }

    [Fact]
    public void CrearVersion_VariasVersiones_NumeraSecuencialmente()
    {
        var ejercicio = new Ejercicio(
            new Curso("CUR-000001", "Matemática"));

        var primera = ejercicio.CrearVersion();
        var segunda = ejercicio.CrearVersion();
        var tercera = ejercicio.CrearVersion();

        Assert.Equal(1, primera.NumeroVersion);
        Assert.Equal(2, segunda.NumeroVersion);
        Assert.Equal(3, tercera.NumeroVersion);

        Assert.Equal(3, ejercicio.Versiones.Count);
    }

    [Fact]
    public void CrearVersion_EjercicioRetirado_LanzaExcepcion()
    {
        var ejercicio = new Ejercicio(
            new Curso("CUR-000001", "Matemática"));

        ejercicio.Retirar();

        Assert.Throws<InvalidOperationException>(
            () => ejercicio.CrearVersion());
    }

    [Fact]
    public void ObtenerVersion_NumeroExistente_DevuelveVersion()
    {
        var ejercicio = new Ejercicio(
            new Curso("CUR-000001", "Matemática"));

        var creada = ejercicio.CrearVersion();

        var resultado = ejercicio.ObtenerVersion(1);

        Assert.Same(creada, resultado);
    }

    [Fact]
    public void ObtenerVersion_NumeroInexistente_DevuelveNull()
    {
        var ejercicio = new Ejercicio(
            new Curso("CUR-000001", "Matemática"));

        Assert.Null(ejercicio.ObtenerVersion(999));
    }


    [Fact]
    public void VersionPublicada_SinVersionesPublicadas_DevuelveNull()
    {
        var ejercicio = new Ejercicio(
            new Curso("CUR-000001", "Matemática"));

        ejercicio.CrearVersion();

        Assert.Null(ejercicio.VersionPublicada);
    }

    [Fact]
    public void PublicarVersion_VersionEnRevision_PublicaCorrectamente()
    {
        var ejercicio = new Ejercicio(
            new Curso("CUR-000001", "Matemática"));

        var version = ejercicio.CrearVersion();

        var pregunta = version.CrearPregunta(
            "¿La afirmación es verdadera?",
            TipoPregunta.VerdaderoFalso);

        pregunta.ConfigurarVerdaderoFalso(true);

        version.EnviarARevision();

        ejercicio.PublicarVersion(1);

        Assert.Equal(
            EstadoEjercicioVersion.Publicado,
            version.Estado);

        Assert.NotNull(ejercicio.VersionPublicada);
        Assert.Same(version, ejercicio.VersionPublicada);
        Assert.NotNull(version.FechaPublicacion);
    }

    [Fact]
    public void PublicarVersion_VersionInexistente_LanzaExcepcion()
    {
        var ejercicio = new Ejercicio(
            new Curso("CUR-000001", "Matemática"));

        Assert.Throws<InvalidOperationException>(
            () => ejercicio.PublicarVersion(999));
    }

    [Fact]
    public void PublicarVersion_SegundaVersionPublicada_LanzaExcepcion()
    {
        var ejercicio = new Ejercicio(
            new Curso("CUR-000001", "Matemática"));

        var primera = ejercicio.CrearVersion();

        var preguntaPrimera = primera.CrearPregunta(
            "Pregunta de la primera versión",
            TipoPregunta.VerdaderoFalso);

        preguntaPrimera.ConfigurarVerdaderoFalso(true);

        primera.EnviarARevision();
        ejercicio.PublicarVersion(1);

        var segunda = ejercicio.CrearVersion();

        var preguntaSegunda = segunda.CrearPregunta(
            "Pregunta de la segunda versión",
            TipoPregunta.VerdaderoFalso);

        preguntaSegunda.ConfigurarVerdaderoFalso(false);

        segunda.EnviarARevision();

        Assert.Throws<InvalidOperationException>(
            () => ejercicio.PublicarVersion(2));

        Assert.Equal(
            EstadoEjercicioVersion.Publicado,
            primera.Estado);

        Assert.Equal(
            EstadoEjercicioVersion.EnRevision,
            segunda.Estado);
    }

    [Fact]
    public void RetirarVersion_VersionPublicada_RetiraCorrectamente()
    {
        var ejercicio = new Ejercicio(
            new Curso("CUR-000001", "Matemática"));

        var version = ejercicio.CrearVersion();

        var pregunta = version.CrearPregunta(
            "¿La afirmación es verdadera?",
            TipoPregunta.VerdaderoFalso);

        pregunta.ConfigurarVerdaderoFalso(true);

        version.EnviarARevision();
        ejercicio.PublicarVersion(1);

        ejercicio.RetirarVersion(1);

        Assert.Equal(
            EstadoEjercicioVersion.Retirado,
            version.Estado);

        Assert.Null(ejercicio.VersionPublicada);
    }

    [Fact]
    public void RetirarVersion_VersionInexistente_LanzaExcepcion()
    {
        var ejercicio = new Ejercicio(
            new Curso("CUR-000001", "Matemática"));

        Assert.Throws<InvalidOperationException>(
            () => ejercicio.RetirarVersion(999));
    }

    [Fact]
    public void RetirarVersion_VersionBorrador_LanzaExcepcion()
    {
        var ejercicio = new Ejercicio(
            new Curso("CUR-000001", "Matemática"));

        ejercicio.CrearVersion();

        Assert.Throws<InvalidOperationException>(
            () => ejercicio.RetirarVersion(1));
    }

    [Fact]
    public void PublicarVersion_EjercicioRetirado_LanzaExcepcion()
    {
        var ejercicio = new Ejercicio(
            new Curso("CUR-000001", "Matemática"));

        var version = ejercicio.CrearVersion();
        version.EnviarARevision();

        ejercicio.Retirar();

        Assert.Throws<InvalidOperationException>(
            () => ejercicio.PublicarVersion(1));
    }

}