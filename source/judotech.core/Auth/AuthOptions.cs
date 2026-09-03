using System;

/// <summary>
/// Auth configuration, bound from the <c>Auth</c> config section in
/// <c>Program.cs</c> (MVP-002 Phase 6). Defaults are safe for local use.
/// </summary>
public sealed class AuthOptions
{
    public const string SectionName = "Auth";

    /// <summary>Absolute session-token lifetime, in minutes. Env:
    /// <c>Auth__TokenLifetimeMinutes</c>.</summary>
    public int TokenLifetimeMinutes { get; set; } = 720;

    /// <summary>PBKDF2 iteration count for new password hashes (OWASP 2023).</summary>
    public int Pbkdf2Iterations { get; set; } = 600_000;

    /// <summary>Comma-separated CORS origins. Env: <c>AllowedOrigins</c>.</summary>
    public string AllowedOrigins { get; set; } = "http://localhost:5173";

    public TimeSpan TokenLifetime => TimeSpan.FromMinutes(TokenLifetimeMinutes);
}
