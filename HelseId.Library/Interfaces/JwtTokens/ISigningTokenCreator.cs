namespace HelseId.Library.Interfaces.JwtTokens;

public interface ISigningTokenCreator
{
    string CreateSigningToken(IPayloadClaimsCreator payloadClaimsCreator, PayloadClaimParameters payloadClaimParameters);
}
