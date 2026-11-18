namespace LISTO.Application.Exceptions;

public class IntentoEnCursoExistenteException : InvalidOperationException
{
    public IntentoEnCursoExistenteException()
        : base(
            "El estudiante ya tiene un intento en curso para este ejercicio. " +
            "Debe enviarlo antes de iniciar otro.")
    {
    }
}
