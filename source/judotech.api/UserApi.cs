using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Configuration;

namespace judotech.api;

public class UserApi
{
    private readonly ILogger<UserApi> _logger;

    public UserApi(ILogger<UserApi> logger)
    {
        _logger = logger;
    }

    [Function("CreateUser")]
    public async Task<IActionResult> CreateUserAsync([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        string userJson = await new StreamReader(req.Body).ReadToEndAsync();
        var userObject = JObject.Parse(userJson);
        var user = userObject.ToObject<DbUser>();
        user.Password = DbLogin.HashPassword(user.Password); //TODO: Should and Could the password be hashed before sending it to service?
        _logger.LogInformation("Try to create user: " + userJson);
        var result = user.Create();
        Users.Instance.Refresh();
        return new OkObjectResult(result);
    }

    [Function("CreateUsers")]
    public async Task<IActionResult> CreateUsersAsync([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        string body = await new StreamReader(req.Body).ReadToEndAsync();
        var bodyJson = JObject.Parse(body);
        var usersArray = (JArray)bodyJson["users"];
        var users = usersArray.ToObject<List<DbUser>>();

        bool result = true;
        foreach (var user in users)
        {
            user.Password = DbLogin.HashPassword(user.Password); //TODO: Should and Could the password be hashed before sending it to service?
            result = user.Create();
        }
        Users.Instance.Refresh();
        return new OkObjectResult(result);
    }

    [Function("ReadUser")]
    public IActionResult ReadUser([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        if (!req.Query.TryGetValue("email", out var sv) || string.IsNullOrWhiteSpace(sv.ToString()))
            return new BadRequestObjectResult("Please pass an email on the query string");

        var user = new DbUser(sv.ToString());
        Console.WriteLine("User full name from db: " + user.FullName);
        user.Password = String.Empty;
        return new OkObjectResult(user);
        
    }
    
    [Function("ReadAllUser")]
    public IActionResult ReadAllUserAsync([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        var shouldclear = req.Query["clearcache"];
        if (!string.IsNullOrEmpty(shouldclear) && shouldclear.ToString().ToLower()=="true") Users.Instance.Refresh();
        return new OkObjectResult(Users.Instance.AllUsers);
    }

    [Function("UpdateUser")]
    public async Task<IActionResult> UpdateUserAsync([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        string body = await new StreamReader(req.Body).ReadToEndAsync();
        var bodyJson = JObject.Parse(body);
        var user = bodyJson.ToObject<DbUser>();

        // om lösnord har ett värde 
        if (!string.IsNullOrEmpty(user.Password))
        {
            user.Password = DbLogin.HashPassword(user.Password);
        }
        var result = user.Update();
        Users.Instance.Refresh();
        return new OkObjectResult(result);
    }
    [Function("DeleteUser")]
    public IActionResult DeleteUserAsync([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        if (!req.Query.TryGetValue("email", out var sv) || string.IsNullOrWhiteSpace(sv.ToString()))
            return new BadRequestObjectResult("Please pass an email on the query string");

        var user = new DbUser(sv.ToString());

        if (string.IsNullOrEmpty(user.Email)) return new BadRequestObjectResult("Please pass an email on the query string");
        user.Delete();
        Users.Instance.Refresh();
        return new OkObjectResult(true);
        
    }
}
