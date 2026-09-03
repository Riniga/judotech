using System.Threading.Tasks;
using judotech.testsupport;

namespace judotech.core.tests.TestSupport;

/// <summary>
/// Sanity checks for the shared in-memory database fake — later MVP-002 phases
/// build the auth tests on top of it.
/// </summary>
public class InMemoryJudoDatabaseTests
{
    [Fact]
    public async Task Create_Read_Delete_RoundTrips()
    {
        var db = new InMemoryJudoDatabase();
        var user = TestData.User("a@test.nu", roles: "judoka");

        Assert.True(await db.CreateUser(user));
        Assert.False(await db.CreateUser(user)); // duplicate email

        var read = await db.ReadUser("A@TEST.NU"); // case-insensitive
        Assert.NotNull(read);
        Assert.Equal("a@test.nu", read!.Email);

        Assert.True(await db.DeleteUser("a@test.nu"));
        Assert.Null(await db.ReadUser("a@test.nu"));
    }

    [Fact]
    public async Task Login_Then_GetUserFromToken_Resolves_The_User()
    {
        var db = new InMemoryJudoDatabase().Seed(TestData.User("b@test.nu"));

        var login = await db.LoginUser(await db.ReadUser("b@test.nu"));
        var resolved = await db.GetUserFromToken(login.Token.ToString());

        Assert.Equal("b@test.nu", resolved!.Email);
        Assert.Null(await db.GetUserFromToken(System.Guid.NewGuid().ToString()));
    }
}
