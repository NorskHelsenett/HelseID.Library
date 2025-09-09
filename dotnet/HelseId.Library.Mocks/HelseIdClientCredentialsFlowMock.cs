namespace HelseId.Library.Mocks;

public class HelseIdClientCredentialsFlowMock : IHelseIdClientCredentialsFlow
{
    public OrganizationNumbers? OrganizationNumbers { get; private set; }

    private readonly string _accessToken;

    public HelseIdClientCredentialsFlowMock(string accessToken)
    {
        _accessToken = accessToken;
    }
    
    public Task<TokenResponse> GetTokenResponseAsync()
    {
        OrganizationNumbers = null;
        return Task.FromResult<TokenResponse>(new AccessTokenResponse
        {
            AccessToken = _accessToken,
            ExpiresIn = 60
        });
    }

    public Task<TokenResponse> GetTokenResponseAsync(OrganizationNumbers organizationNumbers)
    {
        OrganizationNumbers = organizationNumbers;
        return Task.FromResult<TokenResponse>(new AccessTokenResponse
        {
            AccessToken = _accessToken,
            ExpiresIn = 60
        });
    }

    public Task<TokenResponse> GetTokenResponseAsync(string scope)
    {
        throw new NotImplementedException();
    }

    public Task<TokenResponse> GetTokenResponseAsync(string scope, OrganizationNumbers organizationNumbers)
    {
        throw new NotImplementedException();
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
