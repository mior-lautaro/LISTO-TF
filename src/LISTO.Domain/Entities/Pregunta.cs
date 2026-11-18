using LISTO.Domain.Enums;

namespace LISTO.Domain.Entities;

public class Pregunta
{
    public long PreguntaId { get; private set; }
    public long EjercicioVersionId { get; private set; }

    public string Enunciado { get; private set; } = null!;
    public TipoPregunta Tipo { get; private set; }

    public EjercicioVersion EjercicioVersion { get; private set; } = null!;

    private Pregunta() { }

    private ConfiguracionRespuestaLibre? _configuracionRespuestaLibre;

    private readonly List<OpcionPregunta> _opciones = new();

    public IReadOnlyCollection<OpcionPregunta> Opciones =>
        _opciones.AsReadOnly();

    private ConfiguracionVerdaderoFalso? _configuracionVerdaderoFalso;

    public ConfiguracionVerdaderoFalso? ConfiguracionVerdaderoFalso =>
        _configuracionVerdaderoFalso;

    public ConfiguracionRespuestaLibre? ConfiguracionRespuestaLibre =>
    _configuracionRespuestaLibre;

    private readonly List<EspacioCompletar> _espaciosCompletar = new();

    public IReadOnlyCollection<EspacioCompletar> EspaciosCompletar =>
        _espaciosCompletar.AsReadOnly();

    public ModoCorreccionCompletar? ModoCorreccionCompletar
    {
        get;
        private set;
    }

    internal Pregunta(
        EjercicioVersion ejercicioVersion,
        string enunciado,
        TipoPregunta tipo)
    {
        ArgumentNullException.ThrowIfNull(ejercicioVersion);

        if (string.IsNullOrWhiteSpace(enunciado))
            throw new ArgumentException(
                "El enunciado de la pregunta no puede estar vacío.",
                nameof(enunciado));

        if (!Enum.IsDefined(tipo))
            throw new ArgumentOutOfRangeException(
                nameof(tipo),
                "El tipo de pregunta indicado no es válido.");

        EjercicioVersion = ejercicioVersion;
        EjercicioVersionId = ejercicioVersion.EjercicioVersionId;
        Enunciado = enunciado.Trim();
        Tipo = tipo;
    }

    public OpcionPregunta CrearOpcion(
    string texto,
    int orden,
    bool esCorrecta)
    {
        if (EjercicioVersion.Estado !=
            Enums.EstadoEjercicioVersion.Borrador)
        {
            throw new InvalidOperationException(
                "Solo se pueden agregar opciones a preguntas de versiones en estado Borrador.");
        }

        if (Tipo != Enums.TipoPregunta.OpcionMultiple)
            throw new InvalidOperationException(
                "Solo las preguntas de opción múltiple pueden tener opciones.");

        if (_opciones.Any(o => o.Orden == orden))
            throw new InvalidOperationException(
                "Ya existe una opción con ese orden.");

        if (esCorrecta && _opciones.Any(o => o.EsCorrecta))
            throw new InvalidOperationException(
                "La pregunta ya tiene una opción correcta.");

        var opcion = new OpcionPregunta(
            this,
            texto,
            orden,
            esCorrecta);

        _opciones.Add(opcion);

        return opcion;
    }

    public ConfiguracionVerdaderoFalso ConfigurarVerdaderoFalso(
    bool respuestaCorrecta)
    {
        if (EjercicioVersion.Estado != EstadoEjercicioVersion.Borrador)
            throw new InvalidOperationException(
                "Solo se pueden configurar preguntas de versiones en estado Borrador.");

        if (Tipo != TipoPregunta.VerdaderoFalso)
            throw new InvalidOperationException(
                "Esta configuración solo corresponde a preguntas de Verdadero/Falso.");

        if (_configuracionVerdaderoFalso is not null)
            throw new InvalidOperationException(
                "La pregunta ya tiene una configuración de Verdadero/Falso.");

        var configuracion = new ConfiguracionVerdaderoFalso(
            this,
            respuestaCorrecta);

        _configuracionVerdaderoFalso = configuracion;

        return configuracion;
    }

    public ConfiguracionRespuestaLibre ConfigurarRespuestaLibre(
    string respuestaReferencia)
    {
        if (EjercicioVersion.Estado != EstadoEjercicioVersion.Borrador)
            throw new InvalidOperationException(
                "Solo se pueden configurar preguntas de versiones en estado Borrador.");

        if (Tipo != TipoPregunta.RespuestaLibre)
            throw new InvalidOperationException(
                "Esta configuración solo corresponde a preguntas de respuesta libre.");

        if (_configuracionRespuestaLibre is not null)
            throw new InvalidOperationException(
                "La pregunta ya tiene una configuración de respuesta libre.");

        var configuracion = new ConfiguracionRespuestaLibre(
            this,
            respuestaReferencia);

        _configuracionRespuestaLibre = configuracion;
        return configuracion;
    }

    public void ConfigurarModoCorreccionCompletar(
    ModoCorreccionCompletar modo)
    {
        ValidarEdicionCompletar();

        if (Tipo != TipoPregunta.Completar)
            throw new InvalidOperationException(
                "Esta configuración solo corresponde a preguntas de tipo Completar.");

        if (!Enum.IsDefined(modo))
            throw new ArgumentOutOfRangeException(nameof(modo));

        ModoCorreccionCompletar = modo;
    }

    public EspacioCompletar AgregarEspacioCompletar(
        string respuestaEsperada,
        int orden,
        decimal? ponderacion = null)
    {
        ValidarEdicionCompletar();

        if (Tipo != TipoPregunta.Completar)
            throw new InvalidOperationException(
                "Solo las preguntas de tipo Completar pueden tener espacios.");

        if (_espaciosCompletar.Any(e => e.Orden == orden))
            throw new InvalidOperationException(
                "Ya existe un espacio con ese orden.");

        var espacio = new EspacioCompletar(
            this,
            respuestaEsperada,
            orden,
            ponderacion);

        _espaciosCompletar.Add(espacio);

        return espacio;
    }

    private void ValidarEdicionCompletar()
    {
        if (EjercicioVersion.Estado != EstadoEjercicioVersion.Borrador)
            throw new InvalidOperationException(
                "Solo se pueden modificar preguntas de versiones en estado Borrador.");
    }
}