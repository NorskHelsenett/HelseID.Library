namespace HelseId.Library.ExtensionMethods;

public static class TokenResponseExtensions
{
    public static bool IsSuccessful(this TokenResponse tokenResponse, out AccessTokenResponse accessTokenResponse)
    {
        if (tokenResponse is AccessTokenResponse)
        {
            accessTokenResponse = ((AccessTokenResponse)tokenResponse);
            return true;
        }

        accessTokenResponse = new AccessTokenResponse()
        {
            AccessToken = string.Empty,
            ExpiresIn = 0,
        };
        
        return false;
    }
    
    public static TokenErrorResponse AsError(this TokenResponse tokenResponse)
    {
        if (tokenResponse is TokenErrorResponse tokenErrorResponse)
        {
            return tokenErrorResponse;
        }

        return new TokenErrorResponse()
        {
            Error = String.Empty,
        };
    }
}
