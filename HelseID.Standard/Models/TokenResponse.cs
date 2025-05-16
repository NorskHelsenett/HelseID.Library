using System.Text.Json.Serialization;
using HelseID.Standard.Models.Constants;

namespace HelseID.Standard.Models;

public class TokenResponse
{
    [JsonPropertyName(JsonProperties.AccessToken)]
    public string? AccessToken { get; init; }
    [JsonPropertyName(JsonProperties.ExpiresIn)]
    public int ExpiresIn { get; init; }
    public string? DPoPNonce { get; init; }
    
}
