using System.Net.Http.Json;
using System.Security.Cryptography;
using HelseId.Library.ClientCredentials.Interfaces;
using HelseId.Library.Exceptions;
using HelseId.Library.Interfaces.Configuration;
using HelseId.Library.Interfaces.JwtTokens;
using HelseId.Library.Models;
using HelseId.Library.SelfService.Models;
using Microsoft.IdentityModel.Tokens;

namespace HelseId.Library.SelfService;

public class SelvbetjeningSecretUpdater : ISelvbetjeningSecretUpdater
{
    private readonly IHelseIdClientCredentialsFlow _clientCredentialsFlow;
    private readonly ISigningCredentialReference _signingCredentialReference;
    private readonly IDPoPProofCreator _dPoPProofCreator;

    public SelvbetjeningSecretUpdater(
        IHelseIdClientCredentialsFlow clientCredentialsFlow, 
        ISigningCredentialReference signingCredentialReference,
        IDPoPProofCreator  dPoPProofCreator)
    {
        _clientCredentialsFlow = clientCredentialsFlow;
        _signingCredentialReference = signingCredentialReference;
        _dPoPProofCreator = dPoPProofCreator;
    }

    public async Task UpdateClientSecret()
    {
        // 1. Create new json web key pair
        // 2. Get access token for SB-api
        // 3. Call SB-api to update new json web key public key
        // 4. Store key locally

        var newKeyPair = RSA.Create(4096);
        var rsaWithPrivateKey = new RsaSecurityKey(newKeyPair.ExportParameters(true));
        var rsaWithoutPrivateKey = new RsaSecurityKey(newKeyPair.ExportParameters(false));
        var jwkWithPrivateKey = JsonWebKeyConverter.ConvertFromRSASecurityKey(rsaWithPrivateKey);
        var jwkWithoutPrivateKey = JsonWebKeyConverter.ConvertFromRSASecurityKey(rsaWithoutPrivateKey);
        
        var tokenResponse = await _clientCredentialsFlow.GetTokenResponseAsync();
        if(tokenResponse is TokenErrorResponse tokenErrorResponse)
        {
            throw new HelseIdException(tokenErrorResponse);
        }

        var accessTokenResponse = (AccessTokenResponse)tokenResponse;
        var dPopProof = await _dPoPProofCreator.CreateDPoPProofForApiCall("https://api.selvbetjening.test.nhn.no/v1/client-secret", "POST", accessTokenResponse.AccessToken);
        
        // blabla kalle api
        
        
        var httpClient = new HttpClient();
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://api.selvbetjening.test.nhn.no/v1/client-secret");
        httpRequest.Content = JsonContent.Create(jwkWithoutPrivateKey);
        httpRequest.Headers.Add("Authorization", $"DPoP {accessTokenResponse.AccessToken}");
        httpRequest.Headers.Add("DPoP", dPopProof);
        
        var response = await httpClient.SendAsync(httpRequest);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ClientSecretUpdateResponse>();
        await _signingCredentialReference.UpdateSigningCredential(jwkWithPrivateKey.ToString());
        
    }
}
