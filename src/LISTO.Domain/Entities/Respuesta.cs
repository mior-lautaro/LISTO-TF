
namespace LISTO.Domain.Entities;

public class Respuesta
{
    public long RespuestaId { get; private set; }

    public long IntentoId { get; private set; }

    public long PreguntaId { get; private set; }

    public DateTime FechaRegistro { get; private set; }

    public Intento Intento { get; private set; } = null!;

    public Pregunta Pregunta { get; private set; } = null!;

    public RespuestaLibre? RespuestaLibre { get; private set; }

    private Respuesta() { }

    internal Respuesta(Intento intento, Pregunta pregunta)
    {
        ArgumentNullException.ThrowIfNull(intento);
        ArgumentNullException.ThrowIfNull(pregunta);

        Intento = intento;
        IntentoId = intento.IntentoId;

        Pregunta = pregunta;
        PreguntaId = pregunta.PreguntaId;

        FechaRegistro = DateTime.UtcNow;
    }

    internal RespuestaLibre CrearRespuestaLibre(string texto)
    {
        if (RespuestaLibre is not null)
        {
            throw new InvalidOperationException(
                "La respuesta libre ya fue registrada.");
        }

        RespuestaLibre = new RespuestaLibre(this, texto);

        return RespuestaLibre;
    }
}
