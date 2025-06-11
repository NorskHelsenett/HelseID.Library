using HelseId.Library.Models.Payloads;

namespace HelseId.Library.Interfaces.PayloadClaimCreators;

public interface IStructuredClaimsCreator
{
    bool CreateStructuredClaims(PayloadClaimParameters payloadClaimParameters, out Dictionary<string, object> structuredClaims);
}
