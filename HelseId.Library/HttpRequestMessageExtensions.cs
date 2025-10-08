using System.Net.Http.Headers;

namespace HelseId.Library;

public static class HttpRequestMessageExtensions
{
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
