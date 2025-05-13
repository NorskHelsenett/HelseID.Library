using System.Net.Http.Json;
using HelseID.Standard.Interfaces.PayloadClaimCreators;
using HelseID.Standard.Interfaces.TokenRequests;
using HelseID.Standard.Models.Payloads;
using HelseID.Standard.Models.TokenRequests;
using HelseID.Standard.Services.TokenRequests;
using IdentityModel.Client;
using TokenResponse = HelseID.Standard.Models.TokenResponse;

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

    public HelseIdTokenRetriever(IClientCredentialsTokenRequestBuilder clientCredentialsTokenRequestBuilder, IPayloadClaimsCreator payloadClaimsCreator, IHttpClientFactory httpClientFactory)
    {
        _clientCredentialsTokenRequestBuilder = clientCredentialsTokenRequestBuilder;
        _payloadClaimsCreator = payloadClaimsCreator;
        _httpClientFactory = httpClientFactory;
    }
    public async Task<TokenResponse> GetTokenAsync()
    {
        var clientCredentialsTokenRequestParameters = new ClientCredentialsTokenRequestParameters
        {
            PayloadClaimParameters = new PayloadClaimParameters { UseOrganizationNumbers = false }
        };
        var request = await _clientCredentialsTokenRequestBuilder.CreateTokenRequest(_payloadClaimsCreator, clientCredentialsTokenRequestParameters);
        
        var result = await GetClientCredentialsTokenResponse(request);
        
        request = await _clientCredentialsTokenRequestBuilder.CreateTokenRequest(_payloadClaimsCreator, clientCredentialsTokenRequestParameters, result.DPoPNonce);
        
        result = await GetClientCredentialsTokenResponse(request);
        
        return result;
    }

    private async Task<TokenResponse> GetClientCredentialsTokenResponse(ClientCredentialsTokenRequest request)
    {
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            {"client_id", request.ClientId},
            {"scope", request.Scope},
            {"client_assertion", request.ClientAssertion.Value},
            {"client_assertion_type", request.ClientAssertion.Type},
            {"grant_type", request.GrantType},
            
        });
        var httpClient = _httpClientFactory.CreateClient();
        var url = request.Address;
        httpClient.DefaultRequestHeaders.Add("DPoP", request.DPoPProofToken);
        var response = await httpClient.PostAsync(url, content);
        var responseContent = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<TokenResponse>();
        }

        if (response.Headers.TryGetValues("DPoP-Nonce", out var values))
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
