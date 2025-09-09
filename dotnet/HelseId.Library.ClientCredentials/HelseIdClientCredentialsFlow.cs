using HelseId.Library.Exceptions;

namespace HelseId.Library.ClientCredentials;

internal sealed class HelseIdClientCredentialsFlow : IHelseIdClientCredentialsFlow
{   
    private readonly IClientCredentialsTokenRequestBuilder _clientCredentialsTokenRequestBuilder;
    private readonly IPayloadClaimsCreator _payloadClaimsCreator;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ITokenCache _tokenCache;

    public HelseIdClientCredentialsFlow(
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

    public Task<TokenResponse> GetTokenResponseAsync(OrganizationNumbers organizationNumbers)
    {
        return GetTokenResponseAsync("", organizationNumbers);
    }

    public Task<TokenResponse> GetTokenResponseAsync()
    {
        return GetTokenResponseAsync(new OrganizationNumbers());
    }   
    
    public async Task<TokenResponse> GetTokenResponseAsync(string scope, OrganizationNumbers organizationNumbers)
    {
        var cachedResponse = await GetCachedToken(scope, organizationNumbers);
        if (cachedResponse != null)
        {
            return cachedResponse;
        }
        
        var result = await GetClientCredentialsToken(scope, organizationNumbers);

        if (result is AccessTokenResponse accessTokenResponse)
        {
            await AddTokenToCache(scope, organizationNumbers, accessTokenResponse);
        }
        
        return result;
    }

    public Task<TokenResponse> GetTokenResponseAsync(string scope)
    {
        return GetTokenResponseAsync (scope, new OrganizationNumbers());
    }

    private async Task<TokenResponse> GetClientCredentialsToken(string scope, OrganizationNumbers organizationNumbers)
    {
        var clientCredentialsTokenRequestParameters = new ClientCredentialsTokenRequestParameters
        {
            Scope = scope,
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

    private Task AddTokenToCache(string scope, OrganizationNumbers organizationNumbers, AccessTokenResponse tokenResponse)
    {
        var cacheKey = GetCacheKeyForOrganization(scope, organizationNumbers);
        return _tokenCache.AddTokenToCache(cacheKey, tokenResponse);
    }
    
    private Task<AccessTokenResponse?> GetCachedToken(string scope, OrganizationNumbers organizationNumbers)
    {
        var cacheKey = GetCacheKeyForOrganization(scope, organizationNumbers);
        return _tokenCache.GetAccessToken(cacheKey);
    }

    private static string GetCacheKeyForOrganization(string scope, OrganizationNumbers organizationNumbers)
    {
        return $"{HelseIdConstants.TokenResponseCacheKey}_{scope}_{organizationNumbers.ParentOrganization}_{organizationNumbers.ChildOrganization}";
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
                        ErrorDescription = jsonException.Message,
                        RawResponse = await response.Content.ReadAsStringAsync(),
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

            try
            {
                var tokenErrorResponse = await response.Content.ReadFromJsonAsync<TokenErrorResponse>();
                if (tokenErrorResponse != null)
                {
                    tokenErrorResponse.RawResponse = await response.Content.ReadAsStringAsync();
                    return tokenErrorResponse;
                }

                return new TokenErrorResponse
                {
                    Error = "Invalid response",
                    ErrorDescription = $"Expected error response, but received invalid json",
                    RawResponse = await response.Content.ReadAsStringAsync(),
                };
            }
            catch (JsonException jsonException)
            {
                return new TokenErrorResponse
                {
                    Error = "Invalid response",
                    ErrorDescription = jsonException.Message,
                    RawResponse = await response.Content.ReadAsStringAsync(),
                };
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
    }
}
