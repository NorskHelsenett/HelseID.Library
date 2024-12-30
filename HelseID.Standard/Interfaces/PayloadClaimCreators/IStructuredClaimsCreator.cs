using HelseID.Standard.Models.Payloads;

namespace HelseID.Standard.Interfaces.PayloadClaimCreators;

public interface IStructuredClaimsCreator
{
    bool CreateStructuredClaims(PayloadClaimParameters payloadClaimParameters, out Dictionary<string, object> structuredClaims);
}
