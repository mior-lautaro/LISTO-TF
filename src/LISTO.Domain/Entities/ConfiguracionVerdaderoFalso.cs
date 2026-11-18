namespace LISTO.Domain.Entities;

public class ConfiguracionVerdaderoFalso
{
    public long PreguntaId { get; private set; }

    public bool RespuestaCorrecta { get; private set; }

    public Pregunta Pregunta { get; private set; } = null!;

    private ConfiguracionVerdaderoFalso() { }

    internal ConfiguracionVerdaderoFalso(
        Pregunta pregunta,
        bool respuestaCorrecta)
    {
        ArgumentNullException.ThrowIfNull(pregunta);

        Pregunta = pregunta;
        PreguntaId = pregunta.PreguntaId;
        RespuestaCorrecta = respuestaCorrecta;
    }
}