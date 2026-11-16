
namespace LISTO.Application.Exceptions;

public class InscripcionActivaDuplicadaException : Exception
{
    public InscripcionActivaDuplicadaException()
        : base("El estudiante ya tiene una inscripción activa para este curso.")
    {
    }
}
