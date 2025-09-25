using System.Text;
using HelseId.Library.SelfService.Interfaces;

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

public class HelseIdClientCredentialsFlowMock : IHelseIdClientCredentialsFlow
{
    public OrganizationNumbers? OrganizationNumbers { get; private set; }

    public string Scope { get; set; } = string.Empty;
    
    public bool SetTokenErrorResponse { get; set; }

    public string ErrorResponse { get; set; } = "ErrorResponse";

    private readonly string _accessToken;

    public HelseIdClientCredentialsFlowMock(string accessToken)
    {
        _accessToken = accessToken;
    }
    
    public Task<TokenResponse> GetTokenResponseAsync()
    {
        OrganizationNumbers = null;
        return Task.FromResult(AccessTokenResponse());
    }
    
    public Task<TokenResponse> GetTokenResponseAsync(OrganizationNumbers organizationNumbers)
    {
        OrganizationNumbers = organizationNumbers;
        return Task.FromResult(AccessTokenResponse());
    }

    public Task<TokenResponse> GetTokenResponseAsync(string scope)
    {
        Scope = scope;
        OrganizationNumbers = null;
        return Task.FromResult(AccessTokenResponse());
    }

    public Task<TokenResponse> GetTokenResponseAsync(string scope, OrganizationNumbers organizationNumbers)
    {
        OrganizationNumbers = organizationNumbers;
        return Task.FromResult(AccessTokenResponse());
    }

    private TokenResponse AccessTokenResponse()
    {
        if (SetTokenErrorResponse)
        {
            return new TokenErrorResponse
            {
                Error = "Error",
                ErrorDescription = ErrorResponse,
            };
        }
        return new AccessTokenResponse
        {
            AccessToken = _accessToken,
            ExpiresIn = 60
        };
    }

    public Task<string> GetAccessTokenAsync()
    {
        OrganizationNumbers = null;
        return Task.FromResult(_accessToken);
    }

    public Task<string> GetAccessTokenAsync(OrganizationNumbers organizationNumbers)
    {
        OrganizationNumbers = organizationNumbers;
        return Task.FromResult(_accessToken);
    }
}
