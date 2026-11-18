
namespace LISTO.Domain.Entities;

public class ConfiguracionRespuestaLibre
{
    private readonly List<CriterioEvaluacion> _criterios = new();

    public long PreguntaId { get; private set; }
    public string RespuestaReferencia { get; private set; } = null!;

    public Pregunta Pregunta { get; private set; } = null!;

    public IReadOnlyCollection<CriterioEvaluacion> Criterios =>
        _criterios.AsReadOnly();

    private ConfiguracionRespuestaLibre() { }

    internal ConfiguracionRespuestaLibre(
        Pregunta pregunta,
        string respuestaReferencia)
    {
        ArgumentNullException.ThrowIfNull(pregunta);

        if (string.IsNullOrWhiteSpace(respuestaReferencia))
            throw new ArgumentException(
                "La respuesta de referencia no puede estar vacía.",
                nameof(respuestaReferencia));

        Pregunta = pregunta;
        PreguntaId = pregunta.PreguntaId;
        RespuestaReferencia = respuestaReferencia.Trim();
    }

    public void ModificarRespuestaReferencia(string respuestaReferencia)
    {
        ValidarEdicion();

        if (string.IsNullOrWhiteSpace(respuestaReferencia))
            throw new ArgumentException(
                "La respuesta de referencia no puede estar vacía.",
                nameof(respuestaReferencia));

        RespuestaReferencia = respuestaReferencia.Trim();
    }

    public CriterioEvaluacion AgregarCriterio(
        string descripcion,
        decimal ponderacion,
        int orden)
    {
        ValidarEdicion();

        if (_criterios.Any(c => c.Orden == orden))
            throw new InvalidOperationException(
                "Ya existe un criterio con ese orden.");

        var criterio = new CriterioEvaluacion(
            this,
            descripcion,
            ponderacion,
            orden);

        _criterios.Add(criterio);
        return criterio;
    }

    private void ValidarEdicion()
    {
        if (Pregunta.EjercicioVersion.Estado !=
            Enums.EstadoEjercicioVersion.Borrador)
        {
            throw new InvalidOperationException(
                "Solo se puede modificar la configuración de una pregunta cuya versión esté en estado Borrador.");
        }
    }

    internal bool TienePonderacionValida =>
        _criterios.Count > 0 &&
        _criterios.Sum(c => c.Ponderacion) == 100m;
}
