using System.Text.Json.Serialization;
using HelseId.Standard.Models.Constants;

namespace HelseId.Standard.Models;

public abstract class TokenResponse
{
}

public class AccessTokenResponse : TokenResponse
{

    [JsonPropertyName(JsonProperties.AccessToken)]
    public string? AccessToken { get; init; }

    [JsonPropertyName(JsonProperties.ExpiresIn)]
    public int ExpiresIn { get; init; }
}

public class DPoPNonceResponse : TokenResponse
{
    public string? DPoPNonce { get; init; }
}

public class TokenErrorResponse : TokenResponse
{
    [JsonPropertyName("error")]
    public required string Error { get; set; }
    
    [JsonPropertyName("error_description")]
    public required string ErrorDescription { get; set; }
}
