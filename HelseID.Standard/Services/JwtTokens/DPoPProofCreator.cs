using System.Security.Cryptography;
using System.Text;
using HelseID.Standard.Configuration;
using HelseID.Standard.Exceptions;
using HelseID.Standard.Interfaces.JwtTokens;
using IdentityModel;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace HelseID.Standard.Services.JwtTokens;

public class DPoPProofCreator : IDPoPProofCreator
{
    private readonly TimeProvider _timeProvider;
    private readonly SigningCredentials _signingCredentials;
    
    public DPoPProofCreator(
        HelseIdConfiguration helseIdConfiguration,
        TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
        
        _signingCredentials = helseIdConfiguration.SigningCredentials;
    }
    
    public string CreateDPoPProof(string url, string httpMethod, string? dPoPNonce = null, string? accessToken = null)
    {
        var headers = SetHeaders();
        var claims = SetClaims(url, httpMethod, dPoPNonce, accessToken);

        var tokenHandler = new JsonWebTokenHandler
        {
            SetDefaultTimesOnTokenCreation = false
        };
        
        var securityTokenDescriptor = new SecurityTokenDescriptor
        {
            AdditionalHeaderClaims = headers,
            Claims = claims,
            SigningCredentials = _signingCredentials,
            // TODO: check this!
            IssuedAt = _timeProvider.GetLocalNow().DateTime,
        };

        return tokenHandler.CreateToken(securityTokenDescriptor);    
    }

    private static Dictionary<string, object> SetClaims(string url, string httpMethod, string? dPoPNonce, string? accessToken)
    {
        var claims = SetGeneralClaims(url, httpMethod);

        SetNonceClaim(dPoPNonce, claims);

        SetAccessTokenHash(accessToken, claims);
        
        return claims;
    }

    private static Dictionary<string, object> SetGeneralClaims(string url, string httpMethod)
    {
        
        return new Dictionary<string, object>()
        {
            [JwtRegisteredClaimNames.Jti] = Guid.NewGuid().ToString(),
            ["htm"] = httpMethod,
            ["htu"] = url,
        };
    }
    
    private static void SetNonceClaim(string? dPoPNonce, Dictionary<string, object> claims)
    {
        // Used when accessing the authentication server (HelseID):
        if (!string.IsNullOrEmpty(dPoPNonce))
        {
            // nonce: A recent nonce provided via the DPoP-Nonce HTTP header.
            claims[JwtClaimTypes.Nonce] = dPoPNonce;
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
            var ath = Base64Url.Encode(hash);

            claims[JwtClaimTypes.DPoPAccessTokenHash] = ath;
        }
    }
    
    private Dictionary<string, object> SetHeaders()
    {
        return new Dictionary<string, object>()
        {
            [JwtClaimTypes.TokenType] = "dpop+jwt",
            [JwtClaimTypes.JsonWebKey] = SetJwkForHeader(),
        };
    }
    
    private Dictionary<string, string> SetJwkForHeader()
    {
        var securityKey = _signingCredentials.Key as JsonWebKey;

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
                [JsonWebKeyParameterNames.Alg] = _signingCredentials.Algorithm,
            },
            _ => throw new InvalidKeyTypeForDPoPProofException()
        };
    }
}
