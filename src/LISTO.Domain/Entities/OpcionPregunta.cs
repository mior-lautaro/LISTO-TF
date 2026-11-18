namespace LISTO.Domain.Entities;

public class OpcionPregunta
{
    public long OpcionPreguntaId { get; private set; }
    public long PreguntaId { get; private set; }

    public string Texto { get; private set; } = null!;
    public int Orden { get; private set; }
    public bool EsCorrecta { get; private set; }

    public Pregunta Pregunta { get; private set; } = null!;

    private OpcionPregunta() { }

    internal OpcionPregunta(
        Pregunta pregunta,
        string texto,
        int orden,
        bool esCorrecta)
    {
        ArgumentNullException.ThrowIfNull(pregunta);

        if (string.IsNullOrWhiteSpace(texto))
            throw new ArgumentException(
                "El texto de la opción no puede estar vacío.",
                nameof(texto));

        if (orden <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(orden),
                "El orden debe ser mayor que cero.");

        Pregunta = pregunta;
        PreguntaId = pregunta.PreguntaId;
        Texto = texto.Trim();
        Orden = orden;
        EsCorrecta = esCorrecta;
    }
}