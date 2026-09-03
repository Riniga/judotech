using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace judotech.testsupport;

/// <summary>
/// Reproduces the pre-MVP-002 password hash (static salt <c>"AzureWebsite"</c>,
/// 100 000 iterations, raw base64) so migrate-on-login can be tested.
/// </summary>
public static class LegacyPassword
{
    public static string Hash(string password)
        => Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password,
            Encoding.ASCII.GetBytes("AzureWebsite"),
            KeyDerivationPrf.HMACSHA256,
            iterationCount: 100_000,
            numBytesRequested: 32));
}
