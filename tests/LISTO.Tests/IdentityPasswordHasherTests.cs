using System;
using System.Collections.Generic;
using System.Text;
using LISTO.Infrastructure.Security;

namespace LISTO.Tests;

public class IdentityPasswordHasherTests
{
    private readonly IdentityPasswordHasher _hasher = new();

    [Fact]
    public void Hash_DebeGenerarUnHashNoVacio()
    {
        var hash = _hasher.Hash("ClaveSegura123!");

        Assert.False(string.IsNullOrWhiteSpace(hash));
        Assert.NotEqual("ClaveSegura123!", hash);
    }

    [Fact]
    public void Hash_DebeGenerarHashesDiferentesParaLaMismaContrasena()
    {
        var hash1 = _hasher.Hash("ClaveSegura123!");
        var hash2 = _hasher.Hash("ClaveSegura123!");

        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void Verificar_DebeAceptarLaContrasenaCorrecta()
    {
        var contrasena = "ClaveSegura123!";
        var hash = _hasher.Hash(contrasena);

        var resultado = _hasher.Verificar(contrasena, hash);

        Assert.True(resultado);
    }

    [Fact]
    public void Verificar_DebeRechazarUnaContrasenaIncorrecta()
    {
        var hash = _hasher.Hash("ClaveSegura123!");

        var resultado = _hasher.Verificar("OtraClave456!", hash);

        Assert.False(resultado);
    }
}
