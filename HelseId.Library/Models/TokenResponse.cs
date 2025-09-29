namespace HelseId.Library.Models;

public abstract class TokenResponse
{
    public string RawResponse { get; set; } = "";
}

public class AccessTokenResponse : TokenResponse
{

    [JsonPropertyName(JsonProperties.AccessToken)]
    public required string AccessToken { get; init; }

    [JsonPropertyName(JsonProperties.ExpiresIn)]
    public required int ExpiresIn { get; init; }
}

public class TokenErrorResponse : TokenResponse
{
    [JsonPropertyName("error")]
    public required string Error { get; set; }

    [JsonPropertyName("error_description")]
    public string ErrorDescription { get; set; } = "";
}
