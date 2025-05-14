namespace HelseID.Standard.Models.Constants;

public static class HelseIdConstants {
    // A HelseID-specific parameter in the token response for the refresh token expiration time:
    public const string RefreshTokenExpiresIn = "rt_expires_in";
    
    // The name of the claim for the client id in use for request objects
    public const string ClientIdClaimName = "client_id";
    
    
}

public static class ClaimTypes
{
    public const string Nonce = "nonce";

    public const string AccessTokenHash = "ath";
    
    public const string JsonWebKey = "jwk";
}
