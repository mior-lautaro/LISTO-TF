
namespace LISTO.Domain.Entities;

public class CriterioEvaluacion
{
    public long CriterioEvaluacionId { get; private set; }
    public long PreguntaId { get; private set; }

    public string Descripcion { get; private set; } = null!;
    public decimal Ponderacion { get; private set; }
    public int Orden { get; private set; }

    public ConfiguracionRespuestaLibre ConfiguracionRespuestaLibre
    { get; private set; } = null!;

    private CriterioEvaluacion() { }

    internal CriterioEvaluacion(
        ConfiguracionRespuestaLibre configuracion,
        string descripcion,
        decimal ponderacion,
        int orden)
    {
        ArgumentNullException.ThrowIfNull(configuracion);

        if (string.IsNullOrWhiteSpace(descripcion))
            throw new ArgumentException(
                "La descripción del criterio no puede estar vacía.",
                nameof(descripcion));

        if (ponderacion <= 0 || ponderacion > 100)
            throw new ArgumentOutOfRangeException(
                nameof(ponderacion),
                "La ponderación debe ser mayor que cero y no superar 100.");

        if (orden <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(orden),
                "El orden debe ser mayor que cero.");

        ConfiguracionRespuestaLibre = configuracion;
        PreguntaId = configuracion.PreguntaId;
        Descripcion = descripcion.Trim();
        Ponderacion = ponderacion;
        Orden = orden;
    }

    public void ModificarDescripcion(string descripcion)
    {
        ValidarEdicion();

        if (string.IsNullOrWhiteSpace(descripcion))
            throw new ArgumentException(
                "La descripción del criterio no puede estar vacía.",
                nameof(descripcion));

        Descripcion = descripcion.Trim();
    }

    public void ModificarPonderacion(decimal ponderacion)
    {
        ValidarEdicion();

        if (ponderacion <= 0 || ponderacion > 100)
            throw new ArgumentOutOfRangeException(
                nameof(ponderacion),
                "La ponderación debe ser mayor que cero y no superar 100.");

        Ponderacion = ponderacion;
    }

    public void ModificarOrden(int orden)
    {
        ValidarEdicion();

        if (orden <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(orden),
                "El orden debe ser mayor que cero.");

        if (ConfiguracionRespuestaLibre.Criterios
            .Any(c => c != this && c.Orden == orden))
        {
            throw new InvalidOperationException(
                "Ya existe otro criterio con ese orden.");
        }

        Orden = orden;
    }

    private void ValidarEdicion()
    {
        if (ConfiguracionRespuestaLibre.Pregunta.EjercicioVersion.Estado !=
            Enums.EstadoEjercicioVersion.Borrador)
        {
            throw new InvalidOperationException(
                "Solo se pueden modificar criterios de versiones en estado Borrador.");
        }
    }
}
