using HelseId.Library.Interfaces.Configuration;

namespace HelseId.Library.Services.JwtTokens;

public class SigningTokenCreator : ISigningTokenCreator
{
    private readonly IHelseIdConfigurationGetter _helseIdConfigurationGetter;
    private readonly ISigningCredentialReference _signingCredentialReference;

    public SigningTokenCreator(IHelseIdConfigurationGetter helseIdConfiguration, ISigningCredentialReference signingCredentialReference)
    {
        _helseIdConfigurationGetter = helseIdConfiguration;
        _signingCredentialReference = signingCredentialReference;
    }
    
    public async Task<string> CreateSigningToken(IPayloadClaimsCreator payloadClaimsCreator, PayloadClaimParameters payloadClaimParameters)
    {
        var helseIdConfiguration = await _helseIdConfigurationGetter.GetConfiguration();
        
        var claims = payloadClaimsCreator.CreatePayloadClaims(helseIdConfiguration, payloadClaimParameters);

        var securityTokenDescriptor = new SecurityTokenDescriptor
        {
            Claims = claims,
            SigningCredentials = await _signingCredentialReference.GetSigningCredentialReference(),
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
