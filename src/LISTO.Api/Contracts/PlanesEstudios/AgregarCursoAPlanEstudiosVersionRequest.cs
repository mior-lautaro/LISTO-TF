using LISTO.Domain.Enums;

namespace LISTO.Api.Contracts.PlanesEstudios;

public class AgregarCursoAPlanEstudiosVersionRequest
{
    public long CursoId { get; set; }

    public int AnioNivel { get; set; }

    public PeriodoRecomendado PeriodoRecomendado { get; set; }

    public decimal CargaHoraria { get; set; }
}