using System.Net.Http.Json;
using System.Text;
using HelseId.Library.ClientCredentials.Interfaces;
using HelseId.Library.Exceptions;
using HelseId.Library.Interfaces.Configuration;
using HelseId.Library.Interfaces.JwtTokens;
using HelseId.Library.Models;
using HelseId.Library.SelfService.Interfaces;
using HelseId.Library.SelfService.Models;

namespace HelseId.Library.SelfService;

public class SelvbetjeningSecretUpdater : ISelvbetjeningSecretUpdater
{
    private readonly IHelseIdClientCredentialsFlow _clientCredentialsFlow;
    private readonly ISigningCredentialReference _signingCredentialReference;
    private readonly IDPoPProofCreator _dPoPProofCreator;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IKeyManagementService _keyManagementService;

    public SelvbetjeningSecretUpdater(
        IHelseIdClientCredentialsFlow clientCredentialsFlow, 
        ISigningCredentialReference signingCredentialReference,
        IDPoPProofCreator  dPoPProofCreator, IHttpClientFactory httpClientFactory, IKeyManagementService  keyManagementService)
    {
        _clientCredentialsFlow = clientCredentialsFlow;
        _signingCredentialReference = signingCredentialReference;
        _dPoPProofCreator = dPoPProofCreator;
        _httpClientFactory = httpClientFactory;
        _keyManagementService = keyManagementService;
    }

    public async Task<DateTime> UpdateClientSecret()
    {
        var (jsonPublicKey, jsonPrivateKey) = await _keyManagementService.GenerateNewKeyPair();
        
        var tokenResponse = await _clientCredentialsFlow.GetTokenResponseAsync("nhn:selvbetjening/client");
        if(tokenResponse is TokenErrorResponse tokenErrorResponse)
        {
            throw new HelseIdException(tokenErrorResponse);
        }

        var accessTokenResponse = (AccessTokenResponse)tokenResponse;
        var dPopProof = await _dPoPProofCreator.CreateDPoPProofForApiCall("https://api.selvbetjening.test.nhn.no/v1/client-secret", "POST", accessTokenResponse.AccessToken);
        var httpClient = _httpClientFactory.CreateClient();
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://api.selvbetjening.test.nhn.no/v1/client-secret");
        httpRequest.Content = new StringContent(jsonPublicKey, Encoding.UTF8, mediaType: "application/json");
        httpRequest.Headers.Add("Authorization", $"DPoP {accessTokenResponse.AccessToken}");
        httpRequest.Headers.Add("DPoP", dPopProof);
        
        var response = await httpClient.SendAsync(httpRequest);
        var content = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ClientSecretUpdateResponse>();
        await _signingCredentialReference.UpdateSigningCredential(jsonPrivateKey);
        return result!.Expiration;
    }
}
