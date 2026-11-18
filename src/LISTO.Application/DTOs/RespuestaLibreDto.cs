
namespace LISTO.Application.DTOs;

public class RespuestaLibreDto
{
    public long RespuestaId { get; set; }

    public long IntentoId { get; set; }

    public long PreguntaId { get; set; }

    public string Texto { get; set; } = string.Empty;

    public DateTime FechaRegistro { get; set; }
}
