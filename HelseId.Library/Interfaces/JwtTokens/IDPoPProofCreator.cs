namespace HelseId.Library.Interfaces.JwtTokens;

public interface IDPoPProofCreatorForApiRequests
{
    /// <summary>
    /// Creates a DPoP proof using the registered key pair with the given parameters.
    /// </summary>
    /// <param name="url">The url for the request</param>
    /// <param name="httpMethod">The HttpMethod for the request</param>
    /// <param name="accessToken">The Access Token the DPoP proof should be bound to</param>
    /// <returns></returns>
    Task<string> CreateDPoPProofForApiRequest(
        string url,
        string httpMethod,
        string accessToken);
    
    /// <summary>
    /// Creates a DPoP proof using the registered key pair with the given parameters.
    /// </summary>
    /// <param name="url">The url for the request</param>
    /// <param name="httpMethod">The HttpMethod for the request</param>
    /// <param name="accessTokenResponse">The Access Token Response the DPoP proof should be bound to</param>
    /// <returns></returns>
    Task<string> CreateDPoPProofForApiRequest(
        string url,
        string httpMethod,
        AccessTokenResponse accessTokenResponse);
}

public interface IDPoPProofCreator
{
    /// <summary>
    /// Creates a DPoP proof using the registered key pair with the given parameters.
    /// </summary>
    /// <param name="url">The url for the request</param>
    /// <param name="httpMethod">The HttpMethod for the request</param>
    /// <param name="dPoPNonce">The server supplied nonce value</param>
    /// <returns></returns>
    Task<string> CreateDPoPProofForTokenRequest(
        string url,
        string httpMethod,
        string? dPoPNonce = null);
}

