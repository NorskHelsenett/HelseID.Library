using System.Text.Json.Serialization;

namespace HelseID.Standard.Models;

public class TokenResponse
{
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; init; }
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; init; }
    public string? DPoPNonce { get; init; }
    
}
