namespace judotech.testsupport;

/// <summary>Builders for the persisted models, so tests read cleanly.</summary>
public static class TestData
{
    /// <summary>
    /// A <see cref="DbUser"/> with just the fields the auth path cares about.
    /// <paramref name="password"/> is the stored value (a hash in real use).
    /// </summary>
    public static DbUser User(
        string email = "athlete@test.nu",
        string password = "stored-hash",
        params string[] roles)
        => new()
        {
            Id = email,
            Email = email,
            Password = password,
            Roles = roles.ToList(),
            Active = true,
        };

    public static DbCompetition Competition(
        string name = "Test Cup",
        string responsibleEmail = "manager@test.nu")
        => new()
        {
            Id = name,
            Name = name,
            ResponsibleEmail = responsibleEmail,
        };
}
