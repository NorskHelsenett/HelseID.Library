using System.Net.Http.Json;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using HelseId.Library.ClientCredentials.Interfaces;
using HelseId.Library.Exceptions;
using HelseId.Library.Interfaces.Configuration;
using HelseId.Library.Interfaces.JwtTokens;
using HelseId.Library.Models;
using HelseId.Library.SelfService.Configuration;
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
    private readonly SelvbetjeningConfiguration  _configuration;

    public SelvbetjeningSecretUpdater(
        IHelseIdClientCredentialsFlow clientCredentialsFlow, 
        ISigningCredentialReference signingCredentialReference,
        IDPoPProofCreator  dPoPProofCreator, IHttpClientFactory httpClientFactory, IKeyManagementService  keyManagementService,
        SelvbetjeningConfiguration configuration)
    {
        _clientCredentialsFlow = clientCredentialsFlow;
        _signingCredentialReference = signingCredentialReference;
        _dPoPProofCreator = dPoPProofCreator;
        _httpClientFactory = httpClientFactory;
        _keyManagementService = keyManagementService;
        _configuration = configuration;
    }

    public async Task<DateTime> UpdateClientSecret()
    {
        var publicPrivateKeyPair = _keyManagementService.GenerateNewKeyPair();
        
        var httpRequest = await GetClientSecretRequest(publicPrivateKeyPair.PublicKey);

        var httpClient = _httpClientFactory.CreateClient();
        var response = await httpClient.SendAsync(httpRequest);
        
        
        if(!response.IsSuccessStatusCode){
            var content = await response.Content.ReadAsStringAsync();
            throw new HelseIdException(content, "Error from Selvbetjening");
        }
        var result = await response.Content.ReadFromJsonAsync<ClientSecretUpdateResponse>();
        await _signingCredentialReference.UpdateSigningCredential(publicPrivateKeyPair.PrivateKey);
        return result!.Expiration;
    }

    private async Task<HttpRequestMessage> GetClientSecretRequest(string  publicKey)
    {
        var tokenResponse = await _clientCredentialsFlow.GetTokenResponseAsync(_configuration.SelvbetjeningScope);
        if(tokenResponse is TokenErrorResponse tokenErrorResponse)
        {
            throw new HelseIdException(tokenErrorResponse);
        }

        var accessTokenResponse = (AccessTokenResponse)tokenResponse;
        var dPopProof = await _dPoPProofCreator.CreateDPoPProofForApiCall(_configuration.UpdateClientSecretEndpoint, "POST", accessTokenResponse.AccessToken);
        
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, _configuration.UpdateClientSecretEndpoint);
        httpRequest.Content = new StringContent(publicKey, Encoding.UTF8, mediaType: "application/json");
        httpRequest.Headers.Add("Authorization", $"DPoP {accessTokenResponse.AccessToken}");
        httpRequest.Headers.Add("DPoP", dPopProof);
        return httpRequest;
    }
}
