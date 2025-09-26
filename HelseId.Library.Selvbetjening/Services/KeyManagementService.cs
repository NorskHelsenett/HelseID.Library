using System.Security.Cryptography;
using System.Text.Json;
using HelseId.Library.SelfService.Interfaces;
using HelseId.Library.SelfService.Models;
using Microsoft.IdentityModel.Tokens;

namespace HelseId.Library.SelfService.Services;

public class KeyManagementService : IKeyManagementService
{
    
    
    public PublicPrivateKeyPair GenerateNewKeyPair()
    {
        var newKeyPair = RSA.Create(4096);
        var kid = RandomNumberGenerator.GetHexString(32);

        var rsaWithPrivateKey = new RsaSecurityKey(newKeyPair.ExportParameters(true)) { KeyId = kid };
        var rsaWithoutPrivateKey = new RsaSecurityKey(newKeyPair.ExportParameters(false)) { KeyId = kid };

        var jwkWithPrivateKey = JsonWebKeyConverter.ConvertFromRSASecurityKey(rsaWithPrivateKey);
        var jwkWithoutPrivateKey = JsonWebKeyConverter.ConvertFromRSASecurityKey(rsaWithoutPrivateKey);

        var jwkPublicValues = new Dictionary<string, string>
        {
            { "kty", "RSA" },
            { "e", jwkWithoutPrivateKey.E },
            { "use", "sig" },
            { "kid", jwkWithoutPrivateKey.Kid },
            { "alg", "PS256" },
            { "n", jwkWithoutPrivateKey.N }
        };
        var jsonPublicKey = JsonSerializer.Serialize(jwkPublicValues);

        var jwkPrivateValues = new Dictionary<string, string>
        {
            { "kty", "RSA" },
            { "e", jwkWithPrivateKey.E },
            { "use", "sig" },
            { "kid", jwkWithPrivateKey.Kid },
            { "alg", "PS256" },
            { "n", jwkWithPrivateKey.N },
            { "p", jwkWithPrivateKey.P },
            { "q", jwkWithPrivateKey.Q },
            { "d", jwkWithPrivateKey.D },
            { "qi", jwkWithPrivateKey.QI },
            { "dq", jwkWithPrivateKey.DQ },
            { "dp", jwkWithPrivateKey.DP }
        };
        var jsonPrivateKey = JsonSerializer.Serialize(jwkPrivateValues);
        return new PublicPrivateKeyPair
        {
            PrivateKey = jsonPrivateKey,
            PublicKey = jsonPublicKey
        };
    }
}
