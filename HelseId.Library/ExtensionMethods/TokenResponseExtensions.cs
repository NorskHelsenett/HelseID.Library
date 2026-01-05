namespace HelseId.Library.ExtensionMethods;

public static class TokenResponseExtensions
{
    /// <summary>
    /// Checks if the TokenResponse is successful
    /// </summary>
    /// <param name="tokenResponse">The TokenResponse object to be inspected</param>
    /// <param name="accessTokenResponse">The TokenResponse converted to an AccessTokenResponse object</param>
    /// <returns>Returns true if the TokenResponse is an AccessTokenResponse</returns>
    public static bool IsSuccessful(this TokenResponse tokenResponse, out AccessTokenResponse accessTokenResponse)
    {
        if (tokenResponse is AccessTokenResponse response)
        {
            accessTokenResponse = response;
            return true;
        }

        accessTokenResponse = new AccessTokenResponse()
        {
            AccessToken = string.Empty,
            ExpiresIn = 0,
            Scope = string.Empty
        };
        
        return false;
    }
    
    /// <summary>
    /// Converts the TokenResponse to a TokenErrorResponse
    /// </summary>
    /// <param name="tokenResponse">The TokenResponse object to be converted</param>
    /// <returns>Returns a TokenErrorResponse object. If the supplies TokenResponse object is a success,
    /// an empty TokenErrorResponse object is returned.</returns>
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
