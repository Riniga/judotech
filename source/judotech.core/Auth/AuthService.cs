using System;
using System.Threading.Tasks;

/// <summary>
/// <see cref="IAuthService"/> implementation (ADR-0008). MVP-002 Phase 2 covers
/// credential verification and migrate-on-login; Phase 3 adds token issue /
/// validate / revoke.
/// </summary>
public sealed class AuthService : IAuthService
{
    private readonly IJudoDatabase _db;
    private readonly IPasswordHasher _hasher;
    private readonly TimeProvider _time;
    private readonly AuthOptions _options;

    public AuthService(
        IJudoDatabase db,
        IPasswordHasher hasher,
        TimeProvider? timeProvider = null,
        AuthOptions? options = null)
    {
        _db = db;
        _hasher = hasher;
        _time = timeProvider ?? TimeProvider.System;
        _options = options ?? new AuthOptions();
    }

    /// <summary>
    /// Not-yet-DI-wired construction for the transitional Active-Record call
    /// sites. Removed in MVP-002 Phase 6.
    /// </summary>
    public static IAuthService CreateDefault()
        => new AuthService(DatabaseBase.GetDefaultDatabase(), new PasswordHasher(new AuthOptions()));

    public async Task<AuthLoginResult> LoginAsync(string email, string password)
    {
        var user = await _db.ReadUser(email);
        if (user is null) return AuthLoginResult.Fail();

        if (!_hasher.Verify(password, user.Password)) return AuthLoginResult.Fail();

        if (_hasher.NeedsRehash(user.Password))
        {
            user.Password = _hasher.Hash(password);
            await _db.UpdateUser(user);
        }

        // Token issuance still goes through the legacy mechanism; MVP-002 Phase 3
        // replaces this with a random, hashed, expiring token.
        var login = await _db.LoginUser(user);
        return new AuthLoginResult(true, login.Token.ToString(), null, user);
    }

    public Task<DbUser?> AuthenticateAsync(string? token)
        => throw new NotImplementedException("MVP-002 Phase 3");

    public Task LogoutAsync(string? token)
        => throw new NotImplementedException("MVP-002 Phase 3");
}
