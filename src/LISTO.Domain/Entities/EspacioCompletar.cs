namespace LISTO.Domain.Entities;

public class EspacioCompletar
{
    private readonly List<VarianteRespuesta> _variantes = new();

    public long EspacioCompletarId { get; private set; }
    public long PreguntaId { get; private set; }

    public string RespuestaEsperada { get; private set; } = null!;
    public int Orden { get; private set; }
    public decimal? Ponderacion { get; private set; }

    public Pregunta Pregunta { get; private set; } = null!;

    public IReadOnlyCollection<VarianteRespuesta> Variantes =>
        _variantes.AsReadOnly();

    private EspacioCompletar() { }

    internal EspacioCompletar(
        Pregunta pregunta,
        string respuestaEsperada,
        int orden,
        decimal? ponderacion)
    {
        ArgumentNullException.ThrowIfNull(pregunta);

        if (string.IsNullOrWhiteSpace(respuestaEsperada))
            throw new ArgumentException(
                "La respuesta esperada no puede estar vacía.",
                nameof(respuestaEsperada));

        if (orden <= 0)
            throw new ArgumentOutOfRangeException(nameof(orden));

        if (ponderacion.HasValue &&
            (ponderacion.Value <= 0 || ponderacion.Value > 100))
            throw new ArgumentOutOfRangeException(nameof(ponderacion));

        Pregunta = pregunta;
        PreguntaId = pregunta.PreguntaId;
        RespuestaEsperada = respuestaEsperada.Trim();
        Orden = orden;
        Ponderacion = ponderacion;
    }

    public VarianteRespuesta AgregarVariante(string texto)
    {
        ValidarEdicion();

        var variante = new VarianteRespuesta(this, texto);

        if (_variantes.Any(v =>
            string.Equals(
                v.Texto,
                variante.Texto,
                StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException(
                "La variante de respuesta ya existe.");
        }

        _variantes.Add(variante);
        return variante;
    }

    public void ModificarRespuestaEsperada(string respuesta)
    {
        ValidarEdicion();

        if (string.IsNullOrWhiteSpace(respuesta))
            throw new ArgumentException(
                "La respuesta esperada no puede estar vacía.",
                nameof(respuesta));

        RespuestaEsperada = respuesta.Trim();
    }

    public void ModificarPonderacion(decimal? ponderacion)
    {
        ValidarEdicion();

        if (ponderacion.HasValue &&
            (ponderacion.Value <= 0 || ponderacion.Value > 100))
            throw new ArgumentOutOfRangeException(nameof(ponderacion));

        Ponderacion = ponderacion;
    }

    private void ValidarEdicion()
    {
        if (Pregunta.EjercicioVersion.Estado !=
            Enums.EstadoEjercicioVersion.Borrador)
        {
            throw new InvalidOperationException(
                "Solo se pueden modificar espacios de versiones en estado Borrador.");
        }
    }
}