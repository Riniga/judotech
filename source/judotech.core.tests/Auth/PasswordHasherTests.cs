using judotech.testsupport;

namespace judotech.core.tests.Auth;

public class PasswordHasherTests
{
    // Low iteration count keeps the tests fast; production uses AuthOptions' default.
    private static PasswordHasher Fast() => new(new AuthOptions { Pbkdf2Iterations = 1_000 });

    [Fact]
    public void Hash_UsesAFreshSalt_SoEqualPasswordsHashDifferently()
    {
        var hasher = Fast();
        Assert.NotEqual(hasher.Hash("pw"), hasher.Hash("pw"));
    }

    [Fact]
    public void Hash_ProducesTheVersionedFormat()
    {
        var stored = Fast().Hash("pw");

        Assert.StartsWith("pbkdf2$sha256$1000$", stored);
        Assert.Equal(5, stored.Split('$').Length);
    }

    [Fact]
    public void Verify_TrueForCorrect_FalseForWrong()
    {
        var hasher = Fast();
        var stored = hasher.Hash("correct horse");

        Assert.True(hasher.Verify("correct horse", stored));
        Assert.False(hasher.Verify("battery staple", stored));
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-a-hash")]
    [InlineData("pbkdf2$sha256$notanumber$c2FsdA==$aGFzaA==")]
    [InlineData("pbkdf2$sha256$1000$!!!$aGFzaA==")]
    public void Verify_FalseForAMalformedStoredValue(string stored)
    {
        Assert.False(Fast().Verify("pw", stored));
    }

    [Fact]
    public void Verify_AcceptsALegacyHash()
    {
        var legacy = LegacyPassword.Hash("secret");

        Assert.True(Fast().Verify("secret", legacy));
        Assert.False(Fast().Verify("wrong", legacy));
    }

    [Fact]
    public void NeedsRehash_TrueForLegacy_TrueForWeaker_FalseForCurrent()
    {
        var hasher = Fast(); // configured for 1000 iterations

        Assert.True(hasher.NeedsRehash(LegacyPassword.Hash("x")));
        Assert.True(hasher.NeedsRehash(
            $"pbkdf2$sha256$500${Convert.ToBase64String(new byte[16])}${Convert.ToBase64String(new byte[32])}"));
        Assert.False(hasher.NeedsRehash(hasher.Hash("x")));
    }
}
