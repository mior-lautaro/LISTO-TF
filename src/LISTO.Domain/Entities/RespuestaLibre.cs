
namespace LISTO.Domain.Entities;

public class RespuestaLibre
{
    public long RespuestaId { get; private set; }

    public string Texto { get; private set; } = null!;

    public Respuesta Respuesta { get; private set; } = null!;

    private RespuestaLibre() { }

    internal RespuestaLibre(Respuesta respuesta, string texto)
    {
        ArgumentNullException.ThrowIfNull(respuesta);

        ValidarTexto(texto);

        Respuesta = respuesta;
        RespuestaId = respuesta.RespuestaId;
        Texto = texto.Trim();
    }

    public void ModificarTexto(string texto)
    {
        if (Respuesta.Intento.Estado != Enums.EstadoIntento.EnCurso)
        {
            throw new InvalidOperationException(
                "No se puede modificar una respuesta después de enviar el intento.");
        }

        ValidarTexto(texto);
        Texto = texto.Trim();
    }

    private static void ValidarTexto(string texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
        {
            throw new ArgumentException(
                "La respuesta no puede estar vacía.",
                nameof(texto));
        }
    }
}
