using HelseId.Library.Interfaces.Configuration;

namespace HelseId.Library.Services.JwtTokens;

public class SigningTokenCreator : ISigningTokenCreator
{
    private readonly IHelseIdConfigurationGetter _helseIdConfigurationGetter;

    public SigningTokenCreator(IHelseIdConfigurationGetter helseIdConfiguration)
    {
        _helseIdConfigurationGetter = helseIdConfiguration;
    }
    
    public async Task<string> CreateSigningToken(IPayloadClaimsCreator payloadClaimsCreator, PayloadClaimParameters payloadClaimParameters)
    {
        var helseIdConfiguration = await _helseIdConfigurationGetter.GetConfiguration();
        
        var claims = payloadClaimsCreator.CreatePayloadClaims(helseIdConfiguration, payloadClaimParameters);

        var securityTokenDescriptor = new SecurityTokenDescriptor
        {
            Claims = claims,
            SigningCredentials = helseIdConfiguration.SigningCredentials,
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
