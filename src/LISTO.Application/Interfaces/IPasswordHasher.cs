using System;
using System.Collections.Generic;
using System.Text;


namespace LISTO.Application.Interfaces;

public interface IPasswordHasher
{
    string Hash(string contrasena);

    bool Verificar(string contrasena, string hashContrasena);
}
