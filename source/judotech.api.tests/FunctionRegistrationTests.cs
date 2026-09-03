using System.Linq;
using System.Reflection;
using Microsoft.Azure.Functions.Worker;

namespace judotech.api.tests;

/// <summary>
/// Reflection checks over the function assembly. No Cosmos DB needed.
/// </summary>
public class FunctionRegistrationTests
{
    private static IEnumerable<string> FunctionNames()
    {
        return typeof(AuthenticatorApi).Assembly
            .GetTypes()
            .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance))
            .Select(m => m.GetCustomAttribute<FunctionAttribute>()?.Name)
            .Where(name => name is not null)
            .Select(name => name!);
    }

    [Fact]
    public void CoreEndpoints_AreRegistered()
    {
        var names = FunctionNames().ToHashSet();

        Assert.Contains("Login", names);
        Assert.Contains("Logout", names);
        Assert.Contains("CreateUser", names);
        Assert.Contains("ReadAllUser", names);
    }

    [Theory]
    [InlineData("TestAuthenticationApi")] // removed MVP-001 Phase 6
    [InlineData("HashPassword")]          // removed MVP-002 Phase 2 (hashing oracle)
    public void RemovedEndpoints_AreNotExposed(string name)
    {
        Assert.DoesNotContain(name, FunctionNames());
    }
}
