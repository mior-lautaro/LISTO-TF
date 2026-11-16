
namespace LISTO.Application.Exceptions;

public class EstudianteDuplicadoException : Exception
{
    public EstudianteDuplicadoException(string mensaje)
        : base(mensaje)
    {
    }
}
