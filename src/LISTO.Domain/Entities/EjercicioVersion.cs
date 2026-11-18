
using LISTO.Domain.Enums;

namespace LISTO.Domain.Entities;

public class EjercicioVersion
{
    public long EjercicioVersionId { get; private set; }

    public long EjercicioId { get; private set; }

    public int NumeroVersion { get; private set; }

    public DateTime FechaCreacion { get; private set; }

    public DateTime? FechaPublicacion { get; private set; }

    public EstadoEjercicioVersion Estado { get; private set; }

    public Ejercicio Ejercicio { get; private set; } = null!;

    private EjercicioVersion() { }

    private Pregunta? _pregunta;

    public Pregunta? Pregunta => _pregunta;

    internal EjercicioVersion(
        long ejercicioId,
        int numeroVersion)
    {
        if (numeroVersion <= 0)
            throw new ArgumentException(
                "El número de versión debe ser mayor que cero.");

        EjercicioId = ejercicioId;
        NumeroVersion = numeroVersion;

        FechaCreacion = DateTime.UtcNow;
        Estado = EstadoEjercicioVersion.Borrador;
    }

    public void EnviarARevision()
    {
        if (Estado != EstadoEjercicioVersion.Borrador)
            throw new InvalidOperationException(
                "Solo se pueden enviar a revisión versiones en estado Borrador.");

        Estado = EstadoEjercicioVersion.EnRevision;
    }

    public void DevolverABorrador()
    {
        if (Estado != EstadoEjercicioVersion.EnRevision)
            throw new InvalidOperationException(
                "Solo se pueden devolver a borrador versiones en revisión.");

        Estado = EstadoEjercicioVersion.Borrador;
    }

    internal void Publicar()
    {
        if (Estado != EstadoEjercicioVersion.EnRevision)
            throw new InvalidOperationException(
                "Solo se pueden publicar versiones en revisión.");

        ValidarPreguntaParaPublicacion();

        FechaPublicacion = DateTime.UtcNow;
        Estado = EstadoEjercicioVersion.Publicado;
    }

    internal void Retirar()
    {
        if (Estado != EstadoEjercicioVersion.Publicado)
            throw new InvalidOperationException(
                "Solo se pueden retirar versiones publicadas.");

        Estado = EstadoEjercicioVersion.Retirado;
    }

    public Pregunta CrearPregunta(
    string enunciado,
    TipoPregunta tipo)
    {
        if (Estado != EstadoEjercicioVersion.Borrador)
            throw new InvalidOperationException(
                "Solo se pueden agregar preguntas a versiones en estado Borrador.");

        if (_pregunta is not null)
            throw new InvalidOperationException(
                "La versión ya tiene una pregunta asociada.");

        var pregunta = new Pregunta(this, enunciado, tipo);

        _pregunta = pregunta;

        return pregunta;
    }

    private void ValidarPreguntaParaPublicacion()
    {
        if (Pregunta is null)
            throw new InvalidOperationException(
                "No se puede publicar una versión sin una pregunta.");

        switch (Pregunta.Tipo)
        {
            case TipoPregunta.OpcionMultiple:

                if (Pregunta.Opciones.Count < 2)
                    throw new InvalidOperationException(
                        "Una pregunta de opción múltiple debe tener al menos dos opciones.");

                if (Pregunta.Opciones.Count(o => o.EsCorrecta) != 1)
                    throw new InvalidOperationException(
                        "Una pregunta de opción múltiple debe tener exactamente una opción correcta.");

                break;

            case TipoPregunta.VerdaderoFalso:

                if (Pregunta.ConfiguracionVerdaderoFalso is null)
                    throw new InvalidOperationException(
                        "La pregunta de Verdadero/Falso requiere una configuración.");

                break;

            case TipoPregunta.RespuestaLibre:

                if (Pregunta.ConfiguracionRespuestaLibre is null)
                    throw new InvalidOperationException(
                        "La pregunta de respuesta libre requiere una configuración.");

                if (!Pregunta.ConfiguracionRespuestaLibre.TienePonderacionValida)
                    throw new InvalidOperationException(
                        "Los criterios de evaluación deben sumar exactamente 100 y debe existir al menos uno.");

                break;

            case TipoPregunta.Completar:
                if (Pregunta.ModoCorreccionCompletar is null)
                    throw new InvalidOperationException(
                        "La pregunta de tipo Completar requiere un modo de corrección.");

                if (Pregunta.EspaciosCompletar.Count == 0)
                    throw new InvalidOperationException(
                        "Una pregunta de tipo Completar debe tener al menos un espacio.");

                bool hayPonderaciones =
                    Pregunta.EspaciosCompletar.Any(e => e.Ponderacion.HasValue);

                bool todasTienenPonderacion =
                    Pregunta.EspaciosCompletar.All(e => e.Ponderacion.HasValue);

                if (hayPonderaciones && !todasTienenPonderacion)
                    throw new InvalidOperationException(
                        "Todos los espacios deben tener ponderación si se especifica alguna.");

                if (todasTienenPonderacion &&
                    Pregunta.EspaciosCompletar.Sum(e => e.Ponderacion!.Value) != 100m)
                    throw new InvalidOperationException(
                        "Las ponderaciones de los espacios deben sumar exactamente 100.");

                break;

            default:

                throw new InvalidOperationException(
                    "El tipo de pregunta no es válido.");
        }
    }
}
