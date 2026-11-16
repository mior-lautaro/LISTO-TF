namespace LISTO.Application.Exceptions;

public class AsignacionRolDuplicadaException : Exception
{
    public AsignacionRolDuplicadaException()
        : base("El usuario ya tiene asignado este rol.")
    {
    }
}
