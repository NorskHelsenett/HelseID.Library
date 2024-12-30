using HelseID.Standard.Interfaces.PayloadClaimCreators;
using HelseID.Standard.Models.Payloads;

namespace HelseID.Standard.Interfaces.JwtTokens;

public interface ISigningTokenCreator
{
    string CreateSigningToken(IPayloadClaimsCreator payloadClaimsCreator, PayloadClaimParameters payloadClaimParameters);
}
