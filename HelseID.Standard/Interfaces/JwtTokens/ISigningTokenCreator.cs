using HelseId.Standard.Interfaces.PayloadClaimCreators;
using HelseId.Standard.Models.Payloads;

namespace HelseId.Standard.Interfaces.JwtTokens;

public interface ISigningTokenCreator
{
    string CreateSigningToken(IPayloadClaimsCreator payloadClaimsCreator, PayloadClaimParameters payloadClaimParameters);
}
