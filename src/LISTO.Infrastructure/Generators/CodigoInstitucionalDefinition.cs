using System;
using System.Collections.Generic;
using System.Text;

using System.Globalization;
using LISTO.Application.Enums;

namespace LISTO.Infrastructure.Generators;

internal static class CodigoInstitucionalDefinition
{
    internal static (string Prefijo, string Secuencia) ObtenerConfiguracion(
        TipoCodigoInstitucional tipo)
    {
        return tipo switch
        {
            TipoCodigoInstitucional.Curso =>
                ("CUR", "curso_codigo_seq"),

            TipoCodigoInstitucional.PlanEstudios =>
                ("PLA", "plan_estudios_codigo_seq"),

            _ => throw new ArgumentOutOfRangeException(
                nameof(tipo),
                tipo,
                "El tipo de código institucional no está contemplado.")
        };
    }

    internal static string Formatear(string prefijo, long numero)
    {
        if (string.IsNullOrWhiteSpace(prefijo))
            throw new ArgumentException(
                "El prefijo es obligatorio.",
                nameof(prefijo));

        if (numero <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(numero),
                "El número debe ser positivo.");

        return $"{prefijo}-{numero.ToString("D6", CultureInfo.InvariantCulture)}";
    }
}