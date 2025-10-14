using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace judotech.api
{
    public class AuthenticatorApi
    {
        private readonly ILogger<AuthenticatorApi> _logger;
        public AuthenticatorApi(ILogger<AuthenticatorApi> logger)
        {
            _logger = logger;
        }

        [Function("Login")]
        public static async Task<IActionResult> Login([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            //TODO: Could this be simplified are we reaching over the river for water?
            string userJson = await new StreamReader(req.Body).ReadToEndAsync();
            
            // var userObject = JObject.Parse(userJson);
            // var user = userObject.ToObject<DbUser>();
            
            var user = JsonConvert.DeserializeObject<DbUser>(userJson);
            Logger.Instance.Log("With password: " + user.Password);

            user.Password = DbLogin.HashPassword(user.Password); //TODO: Should and Could the password be hashed before sending it to service?
            

            Logger.Instance.Log("Try to login user: " +user.Email);
            Logger.Instance.Log("With password: " + user.Password);
            
            var login = await DbLogin.LoginUser(user);

            Logger.Instance.Log("Return user on login:" + login ); 
            return new OkObjectResult(login);
        }
        [Function("Logout")]
        public static async Task<IActionResult> Logout([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            string loginJson = await new StreamReader(req.Body).ReadToEndAsync();
            var loginObject = JObject.Parse(loginJson);
            DbLogin login= loginObject.ToObject<DbLogin>();
            return new OkObjectResult(login.Logout());
        }
        [Function("HashPassword")]
        public static IActionResult HashPassword([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            if (!req.Query.TryGetValue("password", out var password) || string.IsNullOrWhiteSpace(password.ToString()))
                return new BadRequestObjectResult("Please pass an password on the query string");
            string hash = DbLogin.HashPassword(password.ToString());
            return new OkObjectResult(hash);
        }
    }
}
