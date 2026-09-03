using Newtonsoft.Json;

namespace judotech.api;

/// <summary>Inbound body for <c>POST /api/Login</c>.</summary>
public sealed class LoginRequest
{
    [JsonProperty("email")]
    public string Email { get; set; } = "";

    [JsonProperty("password")]
    public string Password { get; set; } = "";
}
