/// <summary>
/// Process-wide default <see cref="IPasswordHasher"/>. A convenience for call
/// sites that are not yet DI-wired (e.g. the Active-Record <c>*Api</c> classes);
/// replaced by an injected <see cref="IPasswordHasher"/> in MVP-002 Phase 6.
/// </summary>
public static class Passwords
{
    private static readonly IPasswordHasher Hasher = new PasswordHasher(new AuthOptions());

    public static string Hash(string password) => Hasher.Hash(password);

    public static bool Verify(string password, string storedHash) => Hasher.Verify(password, storedHash);

    public static bool NeedsRehash(string storedHash) => Hasher.NeedsRehash(storedHash);
}
