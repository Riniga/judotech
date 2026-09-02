namespace judotech.core.tests;

public class SettingsTests
{
    [Theory]
    [InlineData("Users", "/email")]
    [InlineData("Competitions", "/name")]
    [InlineData("Logins", "/email")]
    public void Containers_MapEachContainerToItsPartitionKeyPath(string container, string partitionKeyPath)
    {
        Assert.True(Settings.Instance.Containers.TryGetValue(container, out var actual));
        Assert.Equal(partitionKeyPath, actual);
    }
}
