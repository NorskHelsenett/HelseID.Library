using HelseID.Standard.Configuration;
using HelseID.Standard.Interfaces.JwtTokens;
using HelseID.Standard.Interfaces.PayloadClaimCreators;
using HelseID.Standard.Models.Payloads;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace HelseID.Standard.Services.JwtTokens;

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
