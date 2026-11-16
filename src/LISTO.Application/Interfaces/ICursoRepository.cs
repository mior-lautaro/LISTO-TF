using System;
using System.Collections.Generic;
using System.Text;
using LISTO.Domain.Entities;

namespace LISTO.Application.Interfaces;

public interface ICursoRepository
{
    Task<Curso?> ObtenerPorIdAsync(
        long cursoId,
        CancellationToken cancellationToken = default);

    Task<Curso?> ObtenerPorCodigoAsync(
        string codigo,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Curso>> ListarAsync(
        bool? estaActivo = null,
        CancellationToken cancellationToken = default);

    Task AgregarAsync(
        Curso curso,
        CancellationToken cancellationToken = default);

    Task<int> GuardarCambiosAsync(
        CancellationToken cancellationToken = default);
}
