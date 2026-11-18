
using LISTO.Domain.Entities;
using LISTO.Domain.Enums;

namespace LISTO.Application.Interfaces;

public interface IInscripcionRepository
{
    Task<Inscripcion?> ObtenerPorIdAsync(
        long inscripcionId,
        CancellationToken cancellationToken = default);

    Task<Inscripcion?> ObtenerActivaAsync(
        long estudianteId,
        long planCursoId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Inscripcion>> ListarPorEstudianteAsync(
        long estudianteId,
        EstadoInscripcion? estado = null,
        CancellationToken cancellationToken = default);

    Task AgregarAsync(
        Inscripcion inscripcion,
        CancellationToken cancellationToken = default);

    Task<int> GuardarCambiosAsync(
        CancellationToken cancellationToken = default);

    Task<bool> TieneInscripcionActivaEnCursoAsync(
    long estudianteId,
    long cursoId,
    CancellationToken cancellationToken = default);
}
