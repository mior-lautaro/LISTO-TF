using System;
using System.Collections.Generic;
using System.Text;
namespace LISTO.Application.Exceptions;

public class NombreRolDuplicadoException : Exception
{
    public NombreRolDuplicadoException(string nombre)
        : base($"Ya existe un rol registrado con el nombre '{nombre}'.")
    {
    }
}
