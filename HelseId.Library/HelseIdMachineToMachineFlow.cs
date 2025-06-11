using HelseId.Library.Interfaces;
using HelseId.Library.Models.DetailsFromClient;

namespace HelseId.Library;

public class HelseIdMachineToMachineFlow : IHelseIdMachineToMachineFlow
{   
    private readonly IClientCredentialsTokenRequestBuilder _clientCredentialsTokenRequestBuilder;
    private readonly IPayloadClaimsCreator _payloadClaimsCreator;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ITokenCache _tokenCache;

    public HelseIdMachineToMachineFlow(
        IClientCredentialsTokenRequestBuilder clientCredentialsTokenRequestBuilder, 
        IPayloadClaimsCreator payloadClaimsCreator, 
        IHttpClientFactory httpClientFactory, 
        ITokenCache tokenCache)
    {
        _clientCredentialsTokenRequestBuilder = clientCredentialsTokenRequestBuilder;
        _payloadClaimsCreator = payloadClaimsCreator;
        _httpClientFactory = httpClientFactory;
        _tokenCache = tokenCache;
    }

    public Task<TokenResponse> GetTokenAsync()
    {
        return GetTokenAsync(new OrganizationNumbers());
    }
    
    public async Task<TokenResponse> GetTokenAsync(OrganizationNumbers organizationNumbers)
    {
        var cachedResponse = await GetCachedToken(organizationNumbers);
        if (cachedResponse != null)
        {
            return cachedResponse;
        }
        
        var result = await GetClientCredentialsToken(organizationNumbers);

        if (result is AccessTokenResponse accessTokenResponse)
        {
            await AddTokenToCache(organizationNumbers, accessTokenResponse);
        }
        
        return result;
    }

    private async Task<TokenResponse> GetClientCredentialsToken(OrganizationNumbers organizationNumbers)
    {
        var clientCredentialsTokenRequestParameters = new ClientCredentialsTokenRequestParameters
        {
            PayloadClaimParameters = new PayloadClaimParameters
            {
                UseOrganizationNumbers = organizationNumbers.HasOrganizationNumbers,
                ParentOrganizationNumber = organizationNumbers.ParentOrganization,
                ChildOrganizationNumber = organizationNumbers.ChildOrganization
            }
        };
        var request = await _clientCredentialsTokenRequestBuilder.CreateTokenRequest(_payloadClaimsCreator, clientCredentialsTokenRequestParameters);
        
        var response = await GetClientCredentialsTokenResponse(request);

        if (response is DPoPNonceResponse dPoPNonceResponse)
        {
            request = await _clientCredentialsTokenRequestBuilder.CreateTokenRequest(_payloadClaimsCreator, clientCredentialsTokenRequestParameters, dPoPNonceResponse.DPoPNonce);
        
            return await GetClientCredentialsTokenResponse(request);            
        }

        return response;
    }

    private Task AddTokenToCache(OrganizationNumbers organizationNumbers, AccessTokenResponse tokenResponse)
    {
        var cacheKey = GetCacheKeyForOrganization(organizationNumbers);
        return _tokenCache.AddTokenToCache(cacheKey, tokenResponse);
    }
    
    private Task<AccessTokenResponse?> GetCachedToken(OrganizationNumbers organizationNumbers)
    {
        var cacheKey = GetCacheKeyForOrganization(organizationNumbers);
        return _tokenCache.GetAccessToken(cacheKey);
    }

    private static string GetCacheKeyForOrganization(OrganizationNumbers organizationNumbers)
    {
        return $"{HelseIdConstants.TokenResponseCacheKey}_{organizationNumbers.ParentOrganization}_{organizationNumbers.ChildOrganization}";
    }
    
    private async Task<TokenResponse> GetClientCredentialsTokenResponse(HelseIdTokenRequest request)
    {
        try
        {
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { ParameterNames.ClientId, request.ClientId },
                { ParameterNames.Scope, request.Scope },
                { ParameterNames.ClientAssertion, request.ClientAssertion },
                { ParameterNames.ClientAssertionType, HelseIdConstants.ClientAssertionType },
                { ParameterNames.GrantType, request.GrantType },
            });

            var httpClient = _httpClientFactory.CreateClient();

            httpClient.DefaultRequestHeaders.Add(HeaderNames.DPoP, request.DPoPProofToken);
            var response = await httpClient.PostAsync(request.Address, content);
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var accessTokenResponse = await response.Content.ReadFromJsonAsync<AccessTokenResponse>();
                    if (accessTokenResponse != null)
                    {
                        return accessTokenResponse;
                    }
                }
                catch (JsonException jsonException)
                {
                    return new TokenErrorResponse
                    {
                        Error = "Invalid response",
                        ErrorDescription = jsonException.Message
                    };
                }
            }

            if (response.Headers.TryGetValues(HeaderNames.DPoPNonce, out var values))
            {
                var dpopNonce = values.FirstOrDefault();

                return new DPoPNonceResponse
                {
                    DPoPNonce = dpopNonce,
                };
            }

            var tokenErrorResponse = await response.Content.ReadFromJsonAsync<TokenErrorResponse>();
            if (tokenErrorResponse != null)
            {
                return tokenErrorResponse;
            }
        }
        catch (Exception exception)
        {
            return new TokenErrorResponse
            {
                Error = "Invalid response",
                ErrorDescription = exception.Message
            };
        }

        return new TokenErrorResponse
        {
            Error = "Invalid operation",
            ErrorDescription = "Token request failed in an unexpected way"
        };
    }
}
