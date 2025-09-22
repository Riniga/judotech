using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
public class CosmosDatabase : DatabaseBase
{
    public CosmosDatabase() {}

    public async override Task<bool> CreateUser(DbUser user)
    {
        var container = await GetContainer(DbUser.ContainerName);
        Logger.Instance.Log("Retrieved Cosmos Db Container: " + container.Id);
        try
        {
            await container.CreateItemAsync<DbUser>(user, new PartitionKey(user.Email));
        }
        catch (Exception ex) 
        { 
            Logger.Instance.Log("Create user failed: " + ex.ToString());
            return false; 
        }
        return true;
    }

    public async override Task<DbUser> ReadUser(string email)
    {
        var container = await GetContainer(DbUser.ContainerName);
        ItemResponse<DbUser> response = null;
        try {
            response = await container.ReadItemAsync<DbUser>(email, new PartitionKey(email));
        } catch (Exception ex) 
        { 
            Logger.Instance.Log("Get user <"+email+"> failed: " + ex.ToString());
            return null; 
        }
        return response.Resource;
    }
    public async override Task<List<DbUser>> ReadAllUsers()
    {
        var container = await GetContainer(DbUser.ContainerName);
        var queryDefinition = new QueryDefinition("SELECT * FROM Users");
        var queryResultSetIterator = container.GetItemQueryIterator<DbUser>(queryDefinition);

        List<DbUser> users = new List<DbUser>();
        while (queryResultSetIterator.HasMoreResults)
        {
            var currentResultSet = await queryResultSetIterator.ReadNextAsync();
            users.AddRange(currentResultSet);
        }
        return users;
    }
    public async override Task<bool> UpdateUser(DbUser user)
    {
        Console.WriteLine("Update user: " + user.Email);
        Console.WriteLine("Update user: " + user.FullName);

        var container = await GetContainer(DbUser.ContainerName);
        try
        {
            await container.ReplaceItemAsync<DbUser>(user, user.Email, new PartitionKey(user.Email));
        }
        catch (Exception ex) 
        { 
            Logger.Instance.Log("Update user failed: " + ex.ToString());
            return false; 
        }
        return true;
    }
    public async override Task<bool> DeleteUser(string email)
    {
        var container = await GetContainer(DbUser.ContainerName);
        try
        {
            await container.DeleteItemAsync<DbUser>(email, new PartitionKey(email));
        }
        catch (Exception ex) 
        { 
            Logger.Instance.Log("Delete user failed: " + ex.ToString());
            return false; 
        }
        return true;
    }


    public async override Task<bool> CreateCompetition(DbCompetition competition)
    {
        var container = await GetContainer(DbCompetition.ContainerName);
        Logger.Instance.Log("Retrieved Cosmos Db Container: " + container.Id);
        try
        {
            await container.CreateItemAsync<DbCompetition>(competition, new PartitionKey(competition.Name));
        }
        catch (Exception ex)
        {
            Logger.Instance.Log("Create competition failed: " + ex.ToString());
            return false;
        }
        return true;
    }


    public async override Task<DbCompetition> ReadCompetition(string name)
    {
        var container = await GetContainer(DbCompetition.ContainerName);
        ItemResponse<DbCompetition> response = null;
        try
        {
            response = await container.ReadItemAsync<DbCompetition>(name, new PartitionKey(name));
        }
        catch (Exception ex)
        {
            Logger.Instance.Log("Get competition <" + name + "> failed: " + ex.ToString());
            return null;
        }
        return response.Resource;
    }
    public async override Task<List<DbCompetition>> ReadAllCompetitions()
    {
        var container = await GetContainer(DbCompetition.ContainerName);
        var queryDefinition = new QueryDefinition("SELECT * FROM Competitions");
        var queryResultSetIterator = container.GetItemQueryIterator<DbCompetition>(queryDefinition);

        List<DbCompetition> competitions = new List<DbCompetition>();
        while (queryResultSetIterator.HasMoreResults)
        {
            var currentResultSet = await queryResultSetIterator.ReadNextAsync();
            competitions.AddRange(currentResultSet);
        }
        return competitions;
    }
    public async override Task<bool> UpdateCompetition(DbCompetition competition)
    {
        var container = await GetContainer(DbCompetition.ContainerName);
        try
        {
            await container.ReplaceItemAsync<DbCompetition>(competition, competition.Name, new PartitionKey(competition.Name));
        }
        catch (Exception ex)
        {
            Logger.Instance.Log("Update competitions failed: " + ex.ToString());
            return false;
        }
        return true;
    }
    public async override Task<bool> DeleteCompetition(string name)
    {
        var container = await GetContainer(DbCompetition.ContainerName);
        try
        {
            await container.DeleteItemAsync<DbCompetition>(name, new PartitionKey(name));
        }
        catch (Exception ex)
        {
            Logger.Instance.Log("Delete competition failed: " + ex.ToString());
            return false;
        }
        return true;
    }

    public async override Task<DbLogin> LoginUser(DbUser user)
    {
        user.Password = string.Empty;
        DbLogin login = new DbLogin() {Email= user.Email, Token= Guid.NewGuid() };
        
        var container = await GetContainer(DbLogin.ContainerName);
        try
        {
            await container.DeleteItemAsync<DbLogin>(login.Email, new PartitionKey(login.Email));
        }
        catch (Exception) {  }

        try
        {
            await container.CreateItemAsync<DbLogin>(login, new PartitionKey(user.Email));
        }
        catch (Exception ex) 
        { 
            Logger.Instance.Log("Create öogin failed: " + ex.ToString());
            return new DbLogin(); 
        }

        
        return login;
    }
    public async override Task<bool> LogoutUser(DbLogin login)
    {
        string token = login.Token.ToString();
        var user = await GetUserFromToken(token);
        if (user.Email==login.Email) 
        {
            try
            {
                var container = await GetContainer(DbLogin.ContainerName);
                await container.DeleteItemAsync<DbLogin>(login.Email, new PartitionKey(login.Email));
            }
            catch (Exception ex) 
            { 
                Logger.Instance.Log("Logout user failed: " + ex.ToString());
                return false; 
            }
            return true;
        }
        return false;
    }
    public async override Task<DbUser> GetUserFromToken(string token)
    {
        try
        {
            var container = await GetContainer(DbLogin.ContainerName);

            string query = "SELECT * FROM Logins WHERE Logins.token='" + token + "'";
            Console.WriteLine(query);

            var queryDefinition = new QueryDefinition(query);
            var queryResultSetIterator = container.GetItemQueryIterator<DbLogin>(queryDefinition);

            
            while (queryResultSetIterator.HasMoreResults)
            {
                var currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (var item in currentResultSet)
                { 
                    Console.WriteLine(item.Email);
                    var user = await ReadUser(item.Email);
                    return user;
                }
            }
            
            //var container = await GetContainer(ContainerType.Logins);
            //var queryDefinition = new QueryDefinition("SELECT * FROM Logins WHERE token='"+token+"'");
            
            //using (FeedIterator<DbLogin> feedIterator = container.GetItemQueryIterator<DbLogin>(queryDefinition))
            //{
            //    while (feedIterator.HasMoreResults)
            //    {
            //        FeedResponse<DbLogin> response = await feedIterator.ReadNextAsync();
            //        foreach (var item in response)
            //        {
            //            Console.WriteLine(item);
            //            //var user = await ReadUser(login.Email);
            //            //return user;
            //        }

            //    }
            //}

        }
        catch (Exception ex) 
        { 
            Logger.Instance.Log("Retrieve login with token  "+token +"  failed: " + ex.ToString());
            return null; 
        }
        return null;
    }

    private async Task<Container> GetContainer(string container)
    {
        var primaryKey = Environment.GetEnvironmentVariable("PrimaryKey");
        var cosmosClient = new CosmosClient(Environment.GetEnvironmentVariable("EndpointUrl"), primaryKey);
        Database database = await cosmosClient.CreateDatabaseIfNotExistsAsync(Environment.GetEnvironmentVariable("DatabaseId"));
        
        return await database.CreateContainerIfNotExistsAsync(container, Settings.Instance.Containers[container]);
    }
}