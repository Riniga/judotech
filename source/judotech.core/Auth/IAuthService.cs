using System;
using System.Threading.Tasks;

/// <summary>
/// Authentication operations for the API (ADR-0008). The implementation
/// (<c>AuthService</c>) arrives in later MVP-002 phases; this contract lets the
/// API functions and tests be written against it now.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Verify credentials and issue a session token. The plaintext token is
    /// returned once (only its hash is stored). A failed login returns
    /// <see cref="AuthLoginResult.Fail"/>.
    /// </summary>
    Task<AuthLoginResult> LoginAsync(string email, string password);

    /// <summary>
    /// Resolve a bearer token to the caller, or <c>null</c> if it is missing,
    /// unknown, expired, or revoked.
    /// </summary>
    Task<DbUser?> AuthenticateAsync(string? token);

    /// <summary>Revoke a session token. Safe to call with an unknown token.</summary>
    Task LogoutAsync(string? token);
}

/// <summary>Outcome of <see cref="IAuthService.LoginAsync"/>.</summary>
public sealed record AuthLoginResult(
    bool Succeeded,
    string? Token,
    DateTimeOffset? ExpiresUtc,
    DbUser? User)
{
    public static AuthLoginResult Fail() => new(false, null, null, null);
}
