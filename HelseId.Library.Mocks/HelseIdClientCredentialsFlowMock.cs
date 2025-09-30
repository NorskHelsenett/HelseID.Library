namespace HelseId.Library.Mocks;



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
