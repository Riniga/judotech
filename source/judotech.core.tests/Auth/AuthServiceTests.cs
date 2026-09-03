using System.Threading.Tasks;
using judotech.testsupport;

namespace judotech.core.tests.Auth;

public class AuthServiceTests
{
    private static (AuthService svc, InMemoryJudoDatabase db, PasswordHasher hasher) Build()
    {
        var hasher = new PasswordHasher(new AuthOptions { Pbkdf2Iterations = 1_000 });
        var db = new InMemoryJudoDatabase();
        return (new AuthService(db, hasher), db, hasher);
    }

    [Fact]
    public async Task LoginAsync_Succeeds_WithCorrectPassword()
    {
        var (svc, db, hasher) = Build();
        db.Seed(TestData.User("a@test.nu", hasher.Hash("pw"), "judoka"));

        var result = await svc.LoginAsync("a@test.nu", "pw");

        Assert.True(result.Succeeded);
        Assert.False(string.IsNullOrWhiteSpace(result.Token));
        Assert.DoesNotContain('$', result.Token!); // a token, not a password hash
    }

    [Fact]
    public async Task LoginAsync_Fails_WithWrongPassword()
    {
        var (svc, db, hasher) = Build();
        db.Seed(TestData.User("a@test.nu", hasher.Hash("pw")));

        Assert.False((await svc.LoginAsync("a@test.nu", "WRONG")).Succeeded);
    }

    [Fact]
    public async Task LoginAsync_Fails_ForUnknownEmail()
    {
        var (svc, _, _) = Build();

        Assert.False((await svc.LoginAsync("ghost@test.nu", "pw")).Succeeded);
    }

    [Fact]
    public async Task LoginAsync_MigratesLegacyHash_OnSuccess()
    {
        var (svc, db, _) = Build();
        var legacy = LegacyPassword.Hash("pw");
        db.Seed(TestData.User("a@test.nu", legacy));

        var result = await svc.LoginAsync("a@test.nu", "pw");

        Assert.True(result.Succeeded);
        var stored = (await db.ReadUser("a@test.nu"))!.Password;
        Assert.StartsWith("pbkdf2$sha256$", stored);
        Assert.NotEqual(legacy, stored);
    }

    [Fact]
    public async Task LoginAsync_DoesNotRehash_AnAlreadyCurrentHash()
    {
        var (svc, db, hasher) = Build();
        var current = hasher.Hash("pw");
        db.Seed(TestData.User("a@test.nu", current));

        await svc.LoginAsync("a@test.nu", "pw");

        Assert.Equal(current, (await db.ReadUser("a@test.nu"))!.Password);
    }
}
