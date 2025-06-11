namespace HelseId.Library.Models.Constants;

public static class HelseIdConstants {
    // A HelseID-specific parameter in the token response for the refresh token expiration time:
    public const string RefreshTokenExpiresIn = "rt_expires_in";
    
    // The name of the claim for the client id in use for request objects
    public const string ClientIdClaimName = "client_id";

    public const string ClientAssertionType = "urn:ietf:params:oauth:client-assertion-type:jwt-bearer";

    public const string TokenResponseCacheKey = "TokenResponseCacheKey";
    
    public const int TokenResponseLeewayInSeconds = 2;
}

public static class ClaimTypes
{
    public const string Nonce = "nonce";

    public const string AccessTokenHash = "ath";
    
    public const string JsonWebKey = "jwk";
    
}

public static class ParameterNames
{
    public const string ClientId = "client_id";
    
    public const string Scope = "scope";
    public const string ClientAssertionType = "client_assertion_type";
    public const string ClientAssertion = "client_assertion";
    public const string GrantType = "grant_type";
}

public static class HeaderNames
{
    public const string DPoPNonce = "DPoP-Nonce";
    public const string DPoP = "DPoP";
}

public static class JsonProperties
{
    public const string TokenEndpoint = "token_endpoint";
    public const string AuthorizationEndpoint = "authorization_endpoint";
    public const string AccessToken = "access_token";
    public const string ExpiresIn = "expires_in";
}

public static class GrantTypes
{
    public const string ClientCredentials = "client_credentials";
}
