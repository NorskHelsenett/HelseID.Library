using System.Text;
using HelseId.Library.Selvbetjening.Interfaces;

namespace HelseId.Library.Mocks;

public class ClientSecretEndpointMock : IClientSecretEndpoint
{
    private readonly string _uri;
    public string PublicKey { get; set; } = string.Empty;

    public ClientSecretEndpointMock(string uri)
    {
        _uri = uri;
    }
    
    public Task<HttpRequestMessage> GetClientSecretRequest(string publicKey)
    {
        PublicKey = publicKey;
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, _uri);
        httpRequest.Content = new StringContent(publicKey, Encoding.UTF8, mediaType: "application/json");
        httpRequest.Headers.Add("Authorization", $"DPoP eyFoobar");
        httpRequest.Headers.Add("DPoP", "eyFoobarFooobar");
        return Task.FromResult(httpRequest);    
    }
}
