using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// Data-access contract for JudoTech. Extracted from <see cref="DatabaseBase"/>
/// (MVP-002, ADR-0008) so the API and the auth service can depend on an
/// abstraction and tests can substitute an in-memory implementation.
///
/// Signatures match <see cref="DatabaseBase"/> exactly for now; the nullability
/// and async cleanup happen as the individual operations are rewritten in later
/// MVP-002 phases.
/// </summary>
public interface IJudoDatabase
{
    Task<bool> CreateUser(DbUser user);
    Task<DbUser> ReadUser(string username);
    Task<List<DbUser>> ReadAllUsers();
    Task<bool> UpdateUser(DbUser user);
    Task<bool> DeleteUser(string username);

    Task<DbLogin> LoginUser(DbUser user);
    Task<bool> LogoutUser(DbLogin login);
    Task<DbUser> GetUserFromToken(string token);

    Task<List<DbCompetition>> ReadAllCompetitions();
    Task<bool> CreateCompetition(DbCompetition competition);
    Task<DbCompetition> ReadCompetition(string name);
    Task<bool> UpdateCompetition(DbCompetition competition);
    Task<bool> DeleteCompetition(string name);
}
