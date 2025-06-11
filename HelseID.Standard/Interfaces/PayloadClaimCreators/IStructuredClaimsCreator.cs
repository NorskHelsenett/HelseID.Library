using HelseId.Standard.Models.Payloads;

namespace HelseId.Standard.Interfaces.PayloadClaimCreators;

public interface IStructuredClaimsCreator
{
    bool CreateStructuredClaims(PayloadClaimParameters payloadClaimParameters, out Dictionary<string, object> structuredClaims);
}
