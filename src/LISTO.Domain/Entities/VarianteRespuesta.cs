namespace LISTO.Domain.Entities;

public class VarianteRespuesta
{
    public long VarianteRespuestaId { get; private set; }
    public long EspacioCompletarId { get; private set; }

    public string Texto { get; private set; } = null!;

    public EspacioCompletar EspacioCompletar { get; private set; } = null!;

    private VarianteRespuesta() { }

    internal VarianteRespuesta(
        EspacioCompletar espacioCompletar,
        string texto)
    {
        ArgumentNullException.ThrowIfNull(espacioCompletar);

        if (string.IsNullOrWhiteSpace(texto))
            throw new ArgumentException(
                "El texto de la variante no puede estar vacío.",
                nameof(texto));

        EspacioCompletar = espacioCompletar;
        EspacioCompletarId = espacioCompletar.EspacioCompletarId;
        Texto = texto.Trim();
    }
}