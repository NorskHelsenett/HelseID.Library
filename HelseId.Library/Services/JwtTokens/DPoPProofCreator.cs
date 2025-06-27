using HelseId.Library.Interfaces.Configuration;

namespace HelseId.Library.Services.JwtTokens;

public class DPoPProofCreator : IDPoPProofCreator
{
    private readonly TimeProvider _timeProvider;
    private readonly IHelseIdConfigurationGetter _helseIdConfigurationGetter;
    
    public DPoPProofCreator(
        IHelseIdConfigurationGetter helseIdConfigurationGetter,
        TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
        _helseIdConfigurationGetter = helseIdConfigurationGetter;
    }
    
    public async Task<string> CreateDPoPProof(string url, string httpMethod, string? dPoPNonce = null, string? accessToken = null)
    {
        var helseIdConfiguration = await _helseIdConfigurationGetter.GetConfiguration();
     
        var headers = SetHeaders(helseIdConfiguration);
        var claims = SetClaims(url, httpMethod, dPoPNonce, accessToken);

        var tokenHandler = new JsonWebTokenHandler
        {
            SetDefaultTimesOnTokenCreation = false
        };

        var securityTokenDescriptor = new SecurityTokenDescriptor
        {
            AdditionalHeaderClaims = headers,
            Claims = claims,
            SigningCredentials = helseIdConfiguration.SigningCredentials,
        };

        return tokenHandler.CreateToken(securityTokenDescriptor);    
    }

    private Dictionary<string, object> SetClaims(string url, string httpMethod, string? dPoPNonce, string? accessToken)
    {
        var claims = SetGeneralClaims(url, httpMethod);

        SetNonceClaim(dPoPNonce, claims);

        SetAccessTokenHash(accessToken, claims);
        
        return claims;
    }

    private Dictionary<string, object> SetGeneralClaims(string url, string httpMethod)
    {
        
        return new Dictionary<string, object>()
        {
            [JwtRegisteredClaimNames.Jti] = Guid.NewGuid().ToString(),
            ["htm"] = httpMethod,
            ["htu"] = url,
            ["iat"] = _timeProvider.GetUtcNow().ToUnixTimeSeconds(),
        };
    }
    
    private static void SetNonceClaim(string? dPoPNonce, Dictionary<string, object> claims)
    {
        // Used when accessing the authentication server (HelseID):
        if (!string.IsNullOrEmpty(dPoPNonce))
        {
            // nonce: A recent nonce provided via the DPoP-Nonce HTTP header.
            claims[ClaimTypes.Nonce] = dPoPNonce;
        }
    }
    
    private static void SetAccessTokenHash(string? accessToken, Dictionary<string, object> claims)
    {
        // Used when accessing an API that requires a DPoP token:
        if (!string.IsNullOrEmpty(accessToken))
        {
            // ath: hash of the access token. The value MUST be the result of a base64url encoding
            // the SHA-256 [SHS] hash of the ASCII encoding of the associated access token's value.
            var hash = SHA256.HashData(Encoding.ASCII.GetBytes(accessToken));
            var ath = Base64UrlEncoder.Encode(hash);

            claims[ClaimTypes.AccessTokenHash] = ath;
        }
    }
    
    private static Dictionary<string, object> SetHeaders(HelseIdConfiguration configuration)
    {
        return new Dictionary<string, object>()
        {
            [JwtRegisteredClaimNames.Typ] = "dpop+jwt",
            [ClaimTypes.JsonWebKey] = SetJwkForHeader(configuration),
        };
    }
    
    private static Dictionary<string, string> SetJwkForHeader(HelseIdConfiguration configuration)
    {
        var securityKey = configuration.SigningCredentials.Key as JsonWebKey;

        return securityKey!.Kty switch
        {
            JsonWebAlgorithmsKeyTypes.EllipticCurve => new Dictionary<string, string>
            {
                [JsonWebKeyParameterNames.Kty] = securityKey.Kty,
                [JsonWebKeyParameterNames.X] = securityKey.X,
                [JsonWebKeyParameterNames.Y] = securityKey.Y,
                [JsonWebKeyParameterNames.Crv] = securityKey.Crv,
            },
            JsonWebAlgorithmsKeyTypes.RSA => new Dictionary<string, string>
            {
                [JsonWebKeyParameterNames.Kty] = securityKey.Kty,
                [JsonWebKeyParameterNames.N] = securityKey.N,
                [JsonWebKeyParameterNames.E] = securityKey.E,
                [JsonWebKeyParameterNames.Alg] = configuration.SigningCredentials.Algorithm,
            },
            _ => throw new InvalidKeyTypeForDPoPProofException()
        };
    }
}
