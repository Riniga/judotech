using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.Extensions.Primitives;

namespace judotech.api
{
    public class CompetitionApi
    {
        private readonly ILogger<CompetitionApi> _logger;
        public CompetitionApi(ILogger<CompetitionApi> logger)
        {
            _logger = logger;
        }
        

        [Function("CreateCompetition")]
        public static async Task<IActionResult> CreateCompetition([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            string competitionJson = await new StreamReader(req.Body).ReadToEndAsync();
            var competitionObject = JObject.Parse(competitionJson);
            var competition = competitionObject.ToObject<DbCompetition>();
            var result = competition.Create();

            return new OkObjectResult(result);
        }

        [Function("CreateCompetitions")]
        public static async Task<IActionResult> CreateCompetitions([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, ILogger log)
        {
            string body = await new StreamReader(req.Body).ReadToEndAsync();
            var bodyJson = JObject.Parse(body);
            var competitionsArray = (JArray)bodyJson["competitions"];
            var competitions = competitionsArray.ToObject<List<DbCompetition>>();

            bool result = true;
            foreach (var competition in competitions)
            {
                result = competition.Create();
            }
            
            return new OkObjectResult(result);
        }


        [Function("ReadCompetition")]
        public static IActionResult ReadCompetition([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            req.Query["name"].ToString();
            if (string.IsNullOrEmpty(req.Query["name"])) return new BadRequestObjectResult("Please pass a competition name on the query string");
            var competition = new DbCompetition();
            return new OkObjectResult(competition);
        }
        [Function("ReadAllCompetitions")]
        public static IActionResult ReadAllCompetitions([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            return new OkObjectResult(Competitions.Instance.AllCompetitions);
        }


        [Function("UpdateCompetition")]
        public static async Task<IActionResult> UpdateCompetition([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            string body = await new StreamReader(req.Body).ReadToEndAsync();
            var bodyJson = JObject.Parse(body);
            var request = bodyJson.ToObject<AuthoraizedRequest>();

            if(!IsAuthoraized(request.Token, "manager")) return new UnauthorizedResult();
            
            var result = request.User.Update();

            return new OkObjectResult(result);
        }


        private static bool IsAuthoraized(string token, string role)
        {
            if (string.IsNullOrEmpty(token)) return false;
            DbUser loggedInUser = DbLogin.GetUserFromToken(token).Result;
            if (loggedInUser == null) return false; 
            if (loggedInUser.Roles.Contains(role)) return true;
            return false;
        }
    }
}
