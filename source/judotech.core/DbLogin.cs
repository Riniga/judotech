using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Newtonsoft.Json;

public class DbLogin
{
    public static string ContainerName = "Logins";
    [JsonProperty("id")]
    public string Id { get; set; } = default!; 
    [JsonProperty("email")]
    public string Email { get; set; } = default!;
    [JsonProperty("token")]
    public Guid Token { get; set; } = default!;

    public async static Task<DbLogin> LoginUser(DbUser user)
    {
        var database = DatabaseBase.GetDefaultDatabase();
        DbUser userFromDb =  database.ReadUser(user.Email).Result;
        if(userFromDb.Password==user.Password)
        {
            Logger.Instance.Log("Correct password!");
            return(await database.LoginUser(user));
        }
        Logger.Instance.Log("Wrong password (db.pass <> user.pass): " + userFromDb.Password + " <> " + user.Password);
        return new DbLogin() { Email=user.Email};
    }
    public bool Logout()
    {
        var database = DatabaseBase.GetDefaultDatabase();
        return database.LogoutUser(this).Result;
    }

    public async static Task<DbUser> GetUserFromToken(string token)
    {
        var database = DatabaseBase.GetDefaultDatabase();
        return await database.GetUserFromToken(token);
    }

    public static string HashPassword(string password)
    {
        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password!,
            salt: Encoding.ASCII.GetBytes("AzureWebsite"),
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8));
        return hashed;
    }
}