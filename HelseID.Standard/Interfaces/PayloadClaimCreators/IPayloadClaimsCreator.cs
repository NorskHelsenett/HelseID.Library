using HelseID.Standard.Configuration;
using HelseID.Standard.Models.Payloads;

namespace HelseID.Standard.Interfaces.PayloadClaimCreators;

public interface IPayloadClaimsCreator
{
    Dictionary<string, object> CreatePayloadClaims(
        HelseIdConfiguration configuration,
        PayloadClaimParameters payloadClaimParameters);
}
