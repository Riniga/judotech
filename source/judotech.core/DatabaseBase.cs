using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public abstract class DatabaseBase
{

    public abstract Task<bool> CreateUser(DbUser user);
    public abstract Task<DbUser> ReadUser(string username);
    public abstract Task<List<DbUser>> ReadAllUsers();
    public abstract Task<bool> UpdateUser(DbUser user);
    public abstract Task<bool> DeleteUser(string username);
    
    public abstract Task<DbLogin> LoginUser(DbUser user);
    public abstract Task<bool> LogoutUser(DbLogin login);
    public abstract Task<DbUser> GetUserFromToken(string token);

    public abstract Task<List<DbCompetition>> ReadAllCompetitions();
    public abstract Task<bool> CreateCompetition(DbCompetition competition);
    public abstract Task<DbCompetition> ReadCompetition(string name);
    public abstract Task<bool> UpdateCompetition(DbCompetition competition);
    public abstract Task<bool> DeleteCompetition(string name);

    internal static DatabaseBase GetDatabase<T>() where T : DatabaseBase, new()
    {
        return new T();
    }
    internal static DatabaseBase GetDefaultDatabase()
    {
        return GetDatabase<CosmosDatabase>();
    }
}

