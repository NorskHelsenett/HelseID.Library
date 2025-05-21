using System.Net.Http.Json;
using System.Text.Json;
using HelseID.Standard.Interfaces.PayloadClaimCreators;
using HelseID.Standard.Interfaces.TokenRequests;
using HelseID.Standard.Models;
using HelseID.Standard.Models.Constants;
using HelseID.Standard.Models.Payloads;
using HelseID.Standard.Models.TokenRequests;
using Microsoft.Extensions.Caching.Distributed;

namespace HelseID.Standard;

public class HelseIdTokenRetriever : IHelseIdTokenRetriever
{   
    private readonly IClientCredentialsTokenRequestBuilder _clientCredentialsTokenRequestBuilder;
    private readonly IPayloadClaimsCreator _payloadClaimsCreator;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IDistributedCache _cache;

    public HelseIdTokenRetriever(
        IClientCredentialsTokenRequestBuilder clientCredentialsTokenRequestBuilder, 
        IPayloadClaimsCreator payloadClaimsCreator, 
        IHttpClientFactory httpClientFactory, 
        IDistributedCache cache)
    {
        _clientCredentialsTokenRequestBuilder = clientCredentialsTokenRequestBuilder;
        _payloadClaimsCreator = payloadClaimsCreator;
        _httpClientFactory = httpClientFactory;
        _cache = cache;
    }
    public async Task<TokenResponse> GetTokenAsync()
    {
        var cachedResponse = await GetCachedToken();
        if (cachedResponse != null)
        {
            return cachedResponse;
        }
        
        var result = await GetClientCredentialsToken();

        await AddTokenToCache(result);
        
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

    private async Task AddTokenToCache(TokenResponse tokenResponse)
    {
        var serializedTokenResponse = JsonSerializer.SerializeToUtf8Bytes(tokenResponse);
        await _cache.SetAsync(HelseIdConstants.TokenResponseCacheKey,
            serializedTokenResponse,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(tokenResponse.ExpiresIn - HelseIdConstants.TokenResponseLeewayInSeconds)
            });
    }

    private async Task<TokenResponse?> GetCachedToken()
    {
        var cachedTokenResponse = await _cache.GetAsync(HelseIdConstants.TokenResponseCacheKey);

        if (cachedTokenResponse == null || cachedTokenResponse.Length == 0)
        {
            return null;
        }

        return JsonSerializer.Deserialize<TokenResponse>(cachedTokenResponse);
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
        
        httpClient.DefaultRequestHeaders.Add(HeaderNames.DPoP, request.DPoPProofToken);
        var response = await httpClient.PostAsync(request.Address, content);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<TokenResponse>() ?? new TokenResponse();
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
