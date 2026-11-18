
using LISTO.Domain.Enums;

namespace LISTO.Domain.Entities;

public class Intento
{
    public long IntentoId { get; private set; }

    public long EstudianteId { get; private set; }

    public long EjercicioId { get; private set; }

    public long EjercicioVersionId { get; private set; }

    public EstadoIntento Estado { get; private set; }

    public DateTime FechaInicio { get; private set; }

    public DateTime? FechaEnvio { get; private set; }

    public DateTime? FechaEvaluacion { get; private set; }

    public Estudiante Estudiante { get; private set; } = null!;

    public Ejercicio Ejercicio { get; private set; } = null!;

    public EjercicioVersion EjercicioVersion { get; private set; } = null!;

    public Respuesta? Respuesta { get; private set; }

    private Intento() { }

    public Intento(
        Estudiante estudiante,
        Ejercicio ejercicio,
        EjercicioVersion ejercicioVersion)
    {
        ArgumentNullException.ThrowIfNull(estudiante);
        ArgumentNullException.ThrowIfNull(ejercicio);
        ArgumentNullException.ThrowIfNull(ejercicioVersion);

        if (!estudiante.EstaActivo)
            throw new InvalidOperationException(
                "No se puede iniciar un intento para un estudiante inactivo.");

        if (ejercicio.Estado != EstadoEjercicio.Activo)
            throw new InvalidOperationException(
                "No se puede iniciar un intento para un ejercicio retirado.");

        if (ejercicio.VersionPublicada is null ||
            !ReferenceEquals(
                ejercicio.VersionPublicada,
                ejercicioVersion))
        {
            throw new InvalidOperationException(
                "El intento debe utilizar la versión actualmente publicada del ejercicio.");
        }

        Estudiante = estudiante;
        EstudianteId = estudiante.EstudianteId;

        Ejercicio = ejercicio;
        EjercicioId = ejercicio.EjercicioId;

        EjercicioVersion = ejercicioVersion;
        EjercicioVersionId = ejercicioVersion.EjercicioVersionId;

        Estado = EstadoIntento.EnCurso;
        FechaInicio = DateTime.UtcNow;
    }

    public RespuestaLibre RegistrarRespuestaLibre(string texto)
    {
        if (Estado != EstadoIntento.EnCurso)
            throw new InvalidOperationException(
                "Solo se pueden registrar respuestas en intentos en curso.");

        if (Respuesta is not null)
            throw new InvalidOperationException(
                "El intento ya tiene una respuesta registrada.");

        var pregunta = EjercicioVersion.Pregunta
            ?? throw new InvalidOperationException(
                "El ejercicio no tiene una pregunta configurada.");

        if (pregunta.Tipo != TipoPregunta.RespuestaLibre)
            throw new InvalidOperationException(
                "La pregunta no es de tipo Respuesta Libre.");

        var respuesta = new Respuesta(this, pregunta);
        var respuestaLibre = respuesta.CrearRespuestaLibre(texto);

        Respuesta = respuesta;

        return respuestaLibre;
    }

    public void ModificarRespuestaLibre(string texto)
    {
        if (Estado != EstadoIntento.EnCurso)
            throw new InvalidOperationException(
                "No se puede modificar la respuesta de un intento enviado.");

        if (Respuesta?.RespuestaLibre is null)
            throw new InvalidOperationException(
                "El intento no tiene una respuesta libre registrada.");

        Respuesta.RespuestaLibre.ModificarTexto(texto);
    }

    public void Enviar()
    {
        if (Estado != EstadoIntento.EnCurso)
            throw new InvalidOperationException(
                "Solo se pueden enviar intentos que estén en curso.");

        if (Respuesta is null)
            throw new InvalidOperationException(
                "No se puede enviar un intento sin respuesta.");

        Estado = EstadoIntento.Enviado;
        FechaEnvio = DateTime.UtcNow;
    }

    public void RegistrarEvaluacion()
    {
        if (Estado != EstadoIntento.Enviado)
            throw new InvalidOperationException(
                "Solo se pueden evaluar intentos enviados.");

        Estado = EstadoIntento.Evaluado;
        FechaEvaluacion = DateTime.UtcNow;
    }
}
