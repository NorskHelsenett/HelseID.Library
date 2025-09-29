using HelseId.Library.Exceptions;
using HelseId.Library.Interfaces.Configuration;

namespace HelseId.Library.ClientCredentials;

internal sealed class HelseIdClientCredentialsFlow : IHelseIdClientCredentialsFlow
{   
    private readonly IClientCredentialsTokenRequestBuilder _clientCredentialsTokenRequestBuilder;
    private readonly IPayloadClaimsCreator _payloadClaimsCreator;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ITokenCache _tokenCache;
    private readonly ISigningCredentialReference _signingCredentialReference;

    public HelseIdClientCredentialsFlow(
        IClientCredentialsTokenRequestBuilder clientCredentialsTokenRequestBuilder, 
        IPayloadClaimsCreator payloadClaimsCreator, 
        IHttpClientFactory httpClientFactory, 
        ITokenCache tokenCache,
        ISigningCredentialReference signingCredentialReference)
    {
        _clientCredentialsTokenRequestBuilder = clientCredentialsTokenRequestBuilder;
        _payloadClaimsCreator = payloadClaimsCreator;
        _httpClientFactory = httpClientFactory;
        _tokenCache = tokenCache;
        _signingCredentialReference = signingCredentialReference;
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
                UseRequestObjects = false,
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

    private async Task AddTokenToCache(string scope, OrganizationNumbers organizationNumbers, AccessTokenResponse tokenResponse)
    {
        var cacheKey = await GetCacheKeyForOrganization(scope, organizationNumbers); 
        await _tokenCache.AddTokenToCache(cacheKey, tokenResponse);
    }

    private async Task<AccessTokenResponse?> GetCachedToken(string scope, OrganizationNumbers organizationNumbers)
    {
        var cacheKey = await GetCacheKeyForOrganization(scope, organizationNumbers);
        return await _tokenCache.GetAccessToken(cacheKey);
    }

    private async Task<string> GetCacheKeyForOrganization(string scope, OrganizationNumbers organizationNumbers)
    {
        var signingCredential = await _signingCredentialReference.GetSigningCredential();
        return CacheKeyFromParts(HelseIdConstants.TokenResponseCacheKey,
            signingCredential.Kid ?? "",
            scope,
            organizationNumbers.ParentOrganization,
            organizationNumbers.ChildOrganization);
    }

    private static string CacheKeyFromParts(params string[] parts)
    {
        return string.Join('_', parts);
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
