namespace HelseId.Library.Interfaces.JwtTokens;

public interface ISigningTokenCreator
{
    Task<string> CreateSigningToken(IPayloadClaimsCreator payloadClaimsCreator, PayloadClaimParameters payloadClaimParameters);
}
