using System.Collections.Concurrent;

namespace judotech.testsupport;

/// <summary>
/// In-memory <see cref="IJudoDatabase"/> for unit tests — no Cosmos DB.
/// Mirrors the current data-layer contract (see <c>IJudoDatabase</c>); it is
/// updated alongside the real implementation as MVP-002 phases land.
/// </summary>
public sealed class InMemoryJudoDatabase : IJudoDatabase
{
    private readonly ConcurrentDictionary<string, DbUser> _users = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, DbLogin> _logins = new(StringComparer.OrdinalIgnoreCase); // keyed by email
    private readonly ConcurrentDictionary<string, DbCompetition> _competitions = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Add users directly, bypassing <see cref="CreateUser"/>.</summary>
    public InMemoryJudoDatabase Seed(params DbUser[] users)
    {
        foreach (var u in users) _users[u.Email] = u;
        return this;
    }

    // --- Users ---

    public Task<bool> CreateUser(DbUser user)
        => Task.FromResult(_users.TryAdd(user.Email, user));

    public Task<DbUser> ReadUser(string username)
        => Task.FromResult(_users.TryGetValue(username, out var u) ? u : null!);

    public Task<List<DbUser>> ReadAllUsers()
        => Task.FromResult(_users.Values.ToList());

    public Task<bool> UpdateUser(DbUser user)
    {
        _users[user.Email] = user;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteUser(string username)
        => Task.FromResult(_users.TryRemove(username, out _));

    // --- Logins ---

    public Task<DbLogin> LoginUser(DbUser user)
    {
        var login = new DbLogin { Id = user.Email, Email = user.Email, Token = Guid.NewGuid() };
        _logins[user.Email] = login;
        return Task.FromResult(login);
    }

    public Task<bool> LogoutUser(DbLogin login)
        => Task.FromResult(_logins.TryRemove(login.Email, out _));

    public Task<DbUser> GetUserFromToken(string token)
    {
        var match = _logins.Values.FirstOrDefault(l => l.Token.ToString() == token);
        return match is null ? Task.FromResult<DbUser>(null!) : ReadUser(match.Email);
    }

    // --- Competitions ---

    public Task<List<DbCompetition>> ReadAllCompetitions()
        => Task.FromResult(_competitions.Values.ToList());

    public Task<bool> CreateCompetition(DbCompetition competition)
        => Task.FromResult(_competitions.TryAdd(competition.Name, competition));

    public Task<DbCompetition> ReadCompetition(string name)
        => Task.FromResult(_competitions.TryGetValue(name, out var c) ? c : null!);

    public Task<bool> UpdateCompetition(DbCompetition competition)
    {
        _competitions[competition.Name] = competition;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteCompetition(string name)
        => Task.FromResult(_competitions.TryRemove(name, out _));
}
