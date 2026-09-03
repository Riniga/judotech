using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

/// <summary>
/// PBKDF2-HMAC-SHA256 with a per-password random salt (ADR-0008).
///
/// Stored format: <c>pbkdf2$sha256$&lt;iterations&gt;$&lt;salt-b64&gt;$&lt;hash-b64&gt;</c>.
/// Legacy values (raw base64, static salt <c>"AzureWebsite"</c>, 100 000
/// iterations) still verify, and <see cref="NeedsRehash"/> flags them.
/// </summary>
public sealed class PasswordHasher : IPasswordHasher
{
    private const int SaltBytes = 16;
    private const int HashBytes = 32;
    private const string Prefix = "pbkdf2$sha256$";

    private const string LegacySalt = "AzureWebsite";
    private const int LegacyIterations = 100_000;

    private readonly int _iterations;

    public PasswordHasher(AuthOptions options)
    {
        _iterations = options.Pbkdf2Iterations;
    }

    public string Hash(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltBytes);
        byte[] hash = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, _iterations, HashBytes);
        return $"{Prefix}{_iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    public bool Verify(string password, string storedHash)
    {
        if (string.IsNullOrEmpty(storedHash)) return false;

        if (!TryParse(storedHash, out int iterations, out byte[] salt, out byte[] expected))
            return VerifyLegacy(password, storedHash);

        byte[] actual = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, iterations, expected.Length);
        return CryptographicOperations.FixedTimeEquals(actual, expected);
    }

    public bool NeedsRehash(string storedHash)
    {
        if (string.IsNullOrEmpty(storedHash)) return true;
        if (!TryParse(storedHash, out int iterations, out _, out _)) return true; // legacy / unrecognised
        return iterations < _iterations;
    }

    private static bool TryParse(string stored, out int iterations, out byte[] salt, out byte[] hash)
    {
        iterations = 0;
        salt = Array.Empty<byte>();
        hash = Array.Empty<byte>();

        string[] parts = stored.Split('$');
        if (parts.Length != 5 || parts[0] != "pbkdf2" || parts[1] != "sha256") return false;
        if (!int.TryParse(parts[2], out iterations) || iterations <= 0) return false;

        try
        {
            salt = Convert.FromBase64String(parts[3]);
            hash = Convert.FromBase64String(parts[4]);
        }
        catch (FormatException)
        {
            return false;
        }

        return salt.Length > 0 && hash.Length > 0;
    }

    private static bool VerifyLegacy(string password, string storedHash)
    {
        byte[] stored;
        try
        {
            stored = Convert.FromBase64String(storedHash);
        }
        catch (FormatException)
        {
            return false;
        }

        byte[] actual = KeyDerivation.Pbkdf2(
            password,
            Encoding.ASCII.GetBytes(LegacySalt),
            KeyDerivationPrf.HMACSHA256,
            LegacyIterations,
            stored.Length);

        return CryptographicOperations.FixedTimeEquals(actual, stored);
    }
}
