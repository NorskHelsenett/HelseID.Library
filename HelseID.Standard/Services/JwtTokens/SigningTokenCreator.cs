using HelseId.Standard.Configuration;
using HelseId.Standard.Interfaces.JwtTokens;
using HelseId.Standard.Interfaces.PayloadClaimCreators;
using HelseId.Standard.Models.Payloads;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace HelseId.Standard.Services.JwtTokens;

public class SigningTokenCreator : ISigningTokenCreator
{
    private readonly HelseIdConfiguration _helseIdConfiguration;

    public SigningTokenCreator(HelseIdConfiguration helseIdConfiguration)
    {
        _helseIdConfiguration = helseIdConfiguration;
    }
    
    public string CreateSigningToken(IPayloadClaimsCreator payloadClaimsCreator, PayloadClaimParameters payloadClaimParameters)
    {
        var claims = payloadClaimsCreator.CreatePayloadClaims(_helseIdConfiguration, payloadClaimParameters);

        var securityTokenDescriptor = new SecurityTokenDescriptor
        {
            Claims = claims,
            SigningCredentials = _helseIdConfiguration.SigningCredentials,
            TokenType = payloadClaimParameters.TokenType
        };

        // This creates a (signed) jwt token which is used for the client assertion.
        var tokenHandler = new JsonWebTokenHandler
        {
            SetDefaultTimesOnTokenCreation = false
        };

        return tokenHandler.CreateToken(securityTokenDescriptor);
    }
}
