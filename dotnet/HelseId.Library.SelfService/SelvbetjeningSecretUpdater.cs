using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
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
    private readonly IHttpClientFactory _httpClientFactory;

    public SelvbetjeningSecretUpdater(
        IHelseIdClientCredentialsFlow clientCredentialsFlow, 
        ISigningCredentialReference signingCredentialReference,
        IDPoPProofCreator  dPoPProofCreator, IHttpClientFactory httpClientFactory)
    {
        _clientCredentialsFlow = clientCredentialsFlow;
        _signingCredentialReference = signingCredentialReference;
        _dPoPProofCreator = dPoPProofCreator;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<DateTime> UpdateClientSecret()
    {
        var newKeyPair = RSA.Create(4096);
        var kid = RandomNumberGenerator.GetHexString(32);
        var rsaWithPrivateKey = new RsaSecurityKey(newKeyPair.ExportParameters(true));
        rsaWithPrivateKey.KeyId = kid;
        var rsaWithoutPrivateKey = new RsaSecurityKey(newKeyPair.ExportParameters(false));
        rsaWithoutPrivateKey.KeyId = kid;
        var jwkWithPrivateKey = JsonWebKeyConverter.ConvertFromRSASecurityKey(rsaWithPrivateKey);
        var jwkWithoutPrivateKey = JsonWebKeyConverter.ConvertFromRSASecurityKey(rsaWithoutPrivateKey);
        
        Dictionary<string, string> jwkPublicValues = new Dictionary<string, string>();
        jwkPublicValues.Add("kty", "RSA");
        jwkPublicValues.Add("e", jwkWithoutPrivateKey.E);
        jwkPublicValues.Add("use", "sig");
        jwkPublicValues.Add("kid", jwkWithoutPrivateKey.Kid);
        jwkPublicValues.Add("alg", "PS256");
        jwkPublicValues.Add("n",  jwkWithoutPrivateKey.N);
        string jsonPublicKey = JsonSerializer.Serialize(jwkPublicValues);
        
        Dictionary<string, string> jwkPrivateValues = new Dictionary<string, string>();
        jwkPrivateValues.Add("kty", "RSA");
        jwkPrivateValues.Add("e", jwkWithPrivateKey.E);
        jwkPrivateValues.Add("use", "sig");
        jwkPrivateValues.Add("kid", jwkWithPrivateKey.Kid);
        jwkPrivateValues.Add("alg", "PS256");
        jwkPrivateValues.Add("n",  jwkWithPrivateKey.N);
        jwkPrivateValues.Add("p",  jwkWithPrivateKey.P);
        jwkPrivateValues.Add("q",  jwkWithPrivateKey.Q);
        jwkPrivateValues.Add("d",  jwkWithPrivateKey.D);
        jwkPrivateValues.Add("qi",  jwkWithPrivateKey.QI);
        jwkPrivateValues.Add("dq",  jwkWithPrivateKey.DQ);
        jwkPrivateValues.Add("dp",  jwkWithPrivateKey.DP);
        string jsonPrivateKey = JsonSerializer.Serialize(jwkPrivateValues);
        
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
        
        return result.Expiration;
    }
}
