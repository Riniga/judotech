/// <summary>
/// Password hashing and verification (ADR-0008). Implementations produce a
/// self-describing string and can still verify (and flag for re-hash) values
/// created by an older scheme.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>Hash a plaintext password for storage.</summary>
    string Hash(string password);

    /// <summary>
    /// Constant-time check of a plaintext password against a stored value.
    /// Accepts both the current format and the pre-MVP-002 legacy format.
    /// </summary>
    bool Verify(string password, string storedHash);

    /// <summary>
    /// True if <paramref name="storedHash"/> should be replaced on the next
    /// successful login (legacy format, or weaker than the current parameters).
    /// </summary>
    bool NeedsRehash(string storedHash);
}
