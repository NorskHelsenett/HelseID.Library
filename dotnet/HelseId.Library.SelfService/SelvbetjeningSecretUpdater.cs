using System.Security.Cryptography;
using HelseId.Library.ClientCredentials.Interfaces;
using HelseId.Library.Interfaces.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HelseId.Library.SelfService;

public class SelvbetjeningSecretUpdater : ISelvbetjeningSecretUpdater
{
    private readonly IHelseIdClientCredentialsFlow _clientCredentialsFlow;
    private readonly ISigningCredentialReference _signingCredentialReference;

    public SelvbetjeningSecretUpdater(
        IHelseIdClientCredentialsFlow clientCredentialsFlow, 
        ISigningCredentialReference signingCredentialReference)
    {
        _clientCredentialsFlow = clientCredentialsFlow;
        _signingCredentialReference = signingCredentialReference;
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
        
        // blabla kalle api

        await _signingCredentialReference.UpdateSigningCredential(jwkWithPrivateKey.ToString());
         
        throw new NotImplementedException();
    }
}
