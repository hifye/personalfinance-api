using System;
using System.Security.Cryptography;
using System.Text;
using Auth.Application.Abstractions.Authentication;
using Konscious.Security.Cryptography;

namespace Auth.Infrastructure.Security;

/// <summary>
/// Provedor de hashing de senhas que utiliza o algoritmo Argon2id para garantir alta segurança.
/// </summary>
public sealed class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 32;
    private const int HashSize = 32;
    private const int Iterations = 4;
    private const int Memory = 65536;
    private const int Parallelism = 8;
    
    public string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);

        var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            DegreeOfParallelism = Parallelism,
            Iterations = Iterations,
            MemorySize = Memory,
        };

        var hash = argon2.GetBytes(HashSize);

        return string.Join(
            "$",
            "argon2id",
            Iterations,
            Memory,
            Parallelism,
            Convert.ToBase64String(salt),
            Convert.ToBase64String(hash)
        );
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        var parts = hashedPassword.Split("$");
        if (parts.Length != 6)
            return false;
        if (parts[0] != "argon2id")
            return false;

        try
        {
            var iterations = int.Parse(parts[1]);
            var memory = int.Parse(parts[2]);
            var parallelism = int.Parse(parts[3]);
            var salt = Convert.FromBase64String(parts[4]);
            var expectedHash = Convert.FromBase64String(parts[5]);

            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                DegreeOfParallelism = parallelism,
                Iterations = iterations,
                MemorySize = memory,
            };

            var computedHash = argon2.GetBytes(HashSize);

            return CryptographicOperations.FixedTimeEquals(expectedHash, computedHash);
        }
        catch
        {
            return false;
        }
    }

    public bool NeedsRehash(string hashedPassword)
    {
        var parts = hashedPassword.Split("$");
        if (parts.Length != 6)
            return false;
        if (parts[0] != "argon2id")
            return false;

        var iterations = int.Parse(parts[1]);
        var memory = int.Parse(parts[2]);
        var parallelism = int.Parse(parts[3]);

        return iterations != Iterations
            || memory != Memory
            || parallelism != Parallelism;
    }
}
