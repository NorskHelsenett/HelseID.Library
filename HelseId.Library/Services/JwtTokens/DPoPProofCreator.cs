namespace HelseId.Library.Services.JwtTokens;

public class DPoPProofCreator : IDPoPProofCreator, IDPoPProofCreatorForApiRequests
{
    private readonly ISigningCredentialReference _signingCredentialReference;
    private readonly TimeProvider _timeProvider;
    
    public DPoPProofCreator(
        ISigningCredentialReference signingCredentialReference,
        TimeProvider timeProvider)
    {
        _signingCredentialReference = signingCredentialReference;
        _timeProvider = timeProvider;
    }

    public Task<string> CreateDPoPProofForTokenRequest(string url, string httpMethod, string? dPoPNonce = null)
    {
        return CreateDPoPProofInternal(url,
            httpMethod,
            dPoPNonce);
    }
    
    public Task<string> CreateDPoPProofForApiRequest(string url, string httpMethod, string accessToken)
    {
        return CreateDPoPProofInternal(url,
            httpMethod,
            null,
            accessToken);
    }

    public Task<string> CreateDPoPProofForApiRequest(string url, string httpMethod, AccessTokenResponse accessTokenResponse)
    {
        return CreateDPoPProofInternal(url,
            httpMethod,
            null,
            accessTokenResponse.AccessToken);
    }

    private async Task<string> CreateDPoPProofInternal(string url, string httpMethod, string? dPoPNonce = null, string? accessToken = null)
    {
        if (!string.IsNullOrEmpty(new Uri(url).Query))
        {
            throw new HelseIdException("Cannot create DPoP proof for url with query string", $"Invalid url: {url}");
        } 

        var headers = await SetHeaders();
        var claims = SetClaims(url, httpMethod, dPoPNonce, accessToken);

        var tokenHandler = new JsonWebTokenHandler
        {
            SetDefaultTimesOnTokenCreation = false
        };

        var securityTokenDescriptor = new SecurityTokenDescriptor
        {
            AdditionalHeaderClaims = headers,
            Claims = claims,
            SigningCredentials = await _signingCredentialReference.GetSigningCredential(),
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
    
    private async Task<Dictionary<string, object>> SetHeaders()
    {
        return new Dictionary<string, object>()
        {
            [JwtRegisteredClaimNames.Typ] = "dpop+jwt",
            [ClaimTypes.JsonWebKey] = await SetJwkForHeader(),
        };
    }
    
    private async Task<Dictionary<string, string>> SetJwkForHeader()
    {
        var signingCredential = await _signingCredentialReference.GetSigningCredential();
        var securityKey = signingCredential.Key as JsonWebKey;

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
                [JsonWebKeyParameterNames.Alg] = signingCredential.Algorithm,
            },
            _ => throw new InvalidKeyTypeForDPoPProofException()
        };
    }
}
