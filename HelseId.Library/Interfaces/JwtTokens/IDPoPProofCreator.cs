namespace HelseId.Library.Interfaces.JwtTokens;

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
    
    /// <summary>
    /// Creates a DPoP proof using the registered key pair with the given parameters.
    /// </summary>
    /// <param name="url">The url for the request</param>
    /// <param name="httpMethod">The HttpMethod for the request</param>
    /// <param name="accessToken">The Access Token the DPoP proof should be bound to</param>
    /// <returns></returns>
    Task<string> CreateDPoPProofForApiCall(
        string url,
        string httpMethod,
        string accessToken);
}

