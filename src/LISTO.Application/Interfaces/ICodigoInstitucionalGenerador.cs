using System;
using System.Collections.Generic;
using System.Text;
using LISTO.Application.Enums;

namespace LISTO.Application.Interfaces;

/// <summary>
/// Define el contrato para generar códigos institucionales
/// únicos para las entidades de LISTO.
/// </summary>
public interface ICodigoInstitucionalGenerator
{
    Task<string> GenerarAsync(
        TipoCodigoInstitucional tipo,
        CancellationToken cancellationToken = default);
}