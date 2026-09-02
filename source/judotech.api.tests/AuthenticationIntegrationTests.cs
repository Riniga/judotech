namespace judotech.api.tests;

/// <summary>
/// End-to-end authentication flow against a real Cosmos DB. Ported from the
/// former <c>AuthenticatorApi.TestAuthenticationApi</c> HTTP endpoint (removed in
/// MVP-001 Phase 6).
///
/// Skipped by default: it needs the <c>EndpointUrl</c> / <c>PrimaryKey</c> /
/// <c>DatabaseId</c> environment variables and a reachable Cosmos DB (emulator or
/// real). Remove the <c>Skip</c> to run it locally. CI excludes it via
/// <c>--filter Category!=Integration</c>.
/// </summary>
[Trait("Category", "Integration")]
public class AuthenticationIntegrationTests
{
    [Fact(Skip = "Integration test — requires a reachable Cosmos DB and connection settings.")]
    public async Task FullAuthenticationFlow_CreatesLogsInVerifiesAndDeletesAUser()
    {
        var user = new DbUser(
            "test@test.nu",
            "Test Testsson",
            "0202020202-0202",
            "adress",
            "12345",
            "city",
            "123",
            "456",
            "license",
            "club",
            "zone",
            "QuLWdRplKNXLjEz3IQyoJ8aGrY/OlPTOMWw2YidkzIk=");

        Assert.True(user.Create());

        try
        {
            var login = await DbLogin.LoginUser(user);
            Assert.Equal(user.Email, login.Email);

            var fromToken = await DbLogin.GetUserFromToken(login.Token.ToString());
            Assert.NotNull(fromToken);
            Assert.Equal(login.Email, fromToken!.Email);

            var wrongToken = await DbLogin.GetUserFromToken(Guid.NewGuid().ToString());
            Assert.Null(wrongToken);

            Assert.True(login.Logout());
        }
        finally
        {
            user.Delete();
        }
    }
}
