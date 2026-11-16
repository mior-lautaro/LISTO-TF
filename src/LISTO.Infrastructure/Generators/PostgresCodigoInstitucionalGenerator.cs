using System;
using System.Collections.Generic;
using System.Text;
using LISTO.Application.Enums;
using LISTO.Application.Interfaces;
using LISTO.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LISTO.Infrastructure.Generators;

public class PostgresCodigoInstitucionalGenerator
    : ICodigoInstitucionalGenerator
{
    private readonly ListoDbContext _context;

    public PostgresCodigoInstitucionalGenerator(ListoDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public async Task<string> GenerarAsync(
        TipoCodigoInstitucional tipo,
        CancellationToken cancellationToken = default)
    {
        var (prefijo, secuencia) =
            CodigoInstitucionalDefinition.ObtenerConfiguracion(tipo);

        var conexion = _context.Database.GetDbConnection();

        await _context.Database.OpenConnectionAsync(cancellationToken);

        try
        {
            await using var comando = conexion.CreateCommand();

            comando.CommandText = $"SELECT nextval('{secuencia}')";

            var resultado = await comando.ExecuteScalarAsync(
                cancellationToken);

            if (resultado is null || resultado is DBNull)
            {
                throw new InvalidOperationException(
                    "PostgreSQL no devolvió un número para el código institucional.");
            }

            var numero = Convert.ToInt64(resultado);

            return CodigoInstitucionalDefinition.Formatear(
                prefijo,
                numero);
        }
        finally
        {
            await _context.Database.CloseConnectionAsync();
        }
    }
}