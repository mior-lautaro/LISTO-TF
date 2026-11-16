using System;
using System.Collections.Generic;
using System.Text;

namespace LISTO.Application.Exceptions;

public class EmailDuplicadoException : Exception
{
    public EmailDuplicadoException(string email)
        : base($"Ya existe un usuario registrado con el correo electrónico '{email}'.")
    {
    }
}
