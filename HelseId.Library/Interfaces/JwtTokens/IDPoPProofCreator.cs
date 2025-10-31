namespace HelseId.Library.Interfaces.JwtTokens;

internal interface IDPoPProofCreator
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

