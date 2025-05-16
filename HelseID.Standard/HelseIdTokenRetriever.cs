using System.Net.Http.Json;
using HelseID.Standard.Interfaces.PayloadClaimCreators;
using HelseID.Standard.Interfaces.TokenRequests;
using HelseID.Standard.Models;
using HelseID.Standard.Models.Constants;
using HelseID.Standard.Models.Payloads;
using HelseID.Standard.Models.TokenRequests;
using Microsoft.Extensions.Caching.Memory;

namespace HelseID.Standard;

public interface IHelseIdTokenRetriever
{
    Task<TokenResponse> GetTokenAsync();
}

public class HelseIdTokenRetriever : IHelseIdTokenRetriever
{   
    private readonly IClientCredentialsTokenRequestBuilder _clientCredentialsTokenRequestBuilder;
    private readonly IPayloadClaimsCreator _payloadClaimsCreator;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMemoryCache _memoryCache;

    public HelseIdTokenRetriever(IClientCredentialsTokenRequestBuilder clientCredentialsTokenRequestBuilder, IPayloadClaimsCreator payloadClaimsCreator, IHttpClientFactory httpClientFactory, IMemoryCache memoryCache)
    {
        _clientCredentialsTokenRequestBuilder = clientCredentialsTokenRequestBuilder;
        _payloadClaimsCreator = payloadClaimsCreator;
        _httpClientFactory = httpClientFactory;
        _memoryCache = memoryCache;
    }
    public async Task<TokenResponse> GetTokenAsync()
    {
        var cachedResponse = GetCachedToken();
        if (cachedResponse != null)
        {
            return cachedResponse;
        }
        
        var result = await GetClientCredentialsToken();

        AddTokenToCache(result);
        
        return result;
    }

    private async Task<TokenResponse> GetClientCredentialsToken()
    {
        var clientCredentialsTokenRequestParameters = new ClientCredentialsTokenRequestParameters
        {
            PayloadClaimParameters = new PayloadClaimParameters { UseOrganizationNumbers = false }
        };
        var request = await _clientCredentialsTokenRequestBuilder.CreateTokenRequest(_payloadClaimsCreator, clientCredentialsTokenRequestParameters);
        
        var result = await GetClientCredentialsTokenResponse(request);
        
        request = await _clientCredentialsTokenRequestBuilder.CreateTokenRequest(_payloadClaimsCreator, clientCredentialsTokenRequestParameters, result.DPoPNonce);
        
        return await GetClientCredentialsTokenResponse(request);
    }

    private void AddTokenToCache(TokenResponse tokenResponse)
    {
        _memoryCache.Set(HelseIdConstants.TokenResponseCacheKey, tokenResponse,
            DateTimeOffset.Now.AddSeconds(tokenResponse.ExpiresIn - HelseIdConstants.TokenResponseLeewayInSeconds));
    }

    private TokenResponse? GetCachedToken()
    {
        if(_memoryCache.TryGetValue(HelseIdConstants.TokenResponseCacheKey, out TokenResponse? tokenResponse))
        {
            return tokenResponse;
        }
        return null;
    }
    

    private async Task<TokenResponse> GetClientCredentialsTokenResponse(HelseIdTokenRequest request)
    {
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            {ParameterNames.ClientId, request.ClientId},
            {ParameterNames.Scope, request.Scope},
            {ParameterNames.ClientAssertion, request.ClientAssertion},
            {ParameterNames.ClientAssertionType, HelseIdConstants.ClientAssertionType},
            {ParameterNames.GrantType, request.GrantType},
            
        });
        var httpClient = _httpClientFactory.CreateClient();
        var url = request.Address;
        httpClient.DefaultRequestHeaders.Add(HeaderNames.DPoP, request.DPoPProofToken);
        var response = await httpClient.PostAsync(url, content);
        var responseContent = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<TokenResponse>();
        }

        if (response.Headers.TryGetValues(HeaderNames.DPoPNonce, out var values))
        {
            var dpopNonce = values.FirstOrDefault();

            return new TokenResponse
            {
                DPoPNonce = dpopNonce,
            };
        }

        return new TokenResponse();
    }
}
