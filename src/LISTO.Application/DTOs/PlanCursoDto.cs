using System;
using System.Collections.Generic;
using System.Text;
using LISTO.Domain.Enums;

namespace LISTO.Application.DTOs;

public class PlanCursoDto
{
    public long PlanCursoId { get; set; }

    public long CursoId { get; set; }

    public string CodigoCurso { get; set; } = null!;

    public string NombreCurso { get; set; } = null!;

    public int AnioNivel { get; set; }

    public PeriodoRecomendado PeriodoRecomendado { get; set; }

    public decimal CargaHoraria { get; set; }
}
