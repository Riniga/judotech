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

        // The former TestAuthenticationApi HTTP endpoint (an exception-swallowing
        // self-test) was removed in MVP-001 Phase 6. Its flow now lives in
        // judotech.api.tests/AuthenticationIntegrationTests.cs.

        // The former HashPassword endpoint (a hashing oracle) was removed in
        // MVP-002 Phase 2 — clients send the plaintext password over TLS and the
        // server hashes it (ADR-0008).

        [Function("Login")]
        public static async Task<IActionResult> Login(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {
            string body = await new StreamReader(req.Body).ReadToEndAsync();
            var creds = JsonConvert.DeserializeObject<LoginRequest>(body);
            if (creds is null || string.IsNullOrWhiteSpace(creds.Email) || string.IsNullOrEmpty(creds.Password))
                return new BadRequestObjectResult("email and password are required");

            var result = await AuthService.CreateDefault().LoginAsync(creds.Email, creds.Password);
            if (!result.Succeeded)
                return new UnauthorizedResult();

            return new OkObjectResult(new { token = result.Token, expiresUtc = result.ExpiresUtc });
        }

        [Function("Logout")]
        public static async Task<IActionResult> Logout([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            string loginJson = await new StreamReader(req.Body).ReadToEndAsync();
            var loginObject = JObject.Parse(loginJson);
            DbLogin login = loginObject.ToObject<DbLogin>();
            return new OkObjectResult(login.Logout());
        }
    }
}
