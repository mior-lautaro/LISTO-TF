using System;
using System.Collections.Generic;
using System.Text;
using LISTO.Application.Interfaces;
using Microsoft.AspNetCore.Identity;


namespace LISTO.Infrastructure.Security;

public class IdentityPasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<object> _hasher;

    public IdentityPasswordHasher()
    {
        _hasher = new PasswordHasher<object>();
    }

    public string Hash(string contrasena)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(contrasena);

        return _hasher.HashPassword(new object(), contrasena);
    }

    public bool Verificar(string contrasena, string hashContrasena)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(contrasena);
        ArgumentException.ThrowIfNullOrWhiteSpace(hashContrasena);

        var resultado = _hasher.VerifyHashedPassword(
            new object(),
            hashContrasena,
            contrasena);

        return resultado == PasswordVerificationResult.Success
            || resultado == PasswordVerificationResult.SuccessRehashNeeded;
    }
}
