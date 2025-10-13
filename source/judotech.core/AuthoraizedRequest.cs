
using Newtonsoft.Json;

public class AuthoraizedRequest
{
    [JsonProperty("token")]
    public string Token { get; set; } = default!;
    [JsonProperty("user")]
    public DbUser User { get; set; } = default!;
}