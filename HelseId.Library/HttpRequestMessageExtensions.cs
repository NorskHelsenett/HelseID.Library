using System.Net.Http.Headers;

namespace HelseId.Library;

public static class HttpRequestMessageExtensions
{
    /// <summary>
    /// Sets the Authorization- and DPoP-headers of the given HttpRequestMessage
    /// </summary>
    /// <param name="httpRequest">the HttpRequestMessage instance</param>
    /// <param name="accessTokenResponse">The AccessTokenResponse instance that the DPoP Proof is bound to</param>
    /// <param name="dpopProof">The DPoP Proof</param>
    /// <exception cref="InvalidOperationException">Throws an InvalidOperationException if the Authorization header is already set</exception>
    public static void SetDPoPTokenAndProof(this HttpRequestMessage httpRequest, AccessTokenResponse accessTokenResponse, string dpopProof)
    {
        if (httpRequest.Headers.Authorization != null)
        {
            throw new InvalidOperationException("Authorization header is already set");
        }
        
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("DPoP", accessTokenResponse.AccessToken);
        httpRequest.Headers.Add("DPoP", dpopProof);
    }
}
