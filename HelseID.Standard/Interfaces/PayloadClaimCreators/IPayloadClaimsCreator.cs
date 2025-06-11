using HelseId.Standard.Configuration;
using HelseId.Standard.Models.Payloads;

namespace HelseId.Standard.Interfaces.PayloadClaimCreators;

public interface IPayloadClaimsCreator
{
    Dictionary<string, object> CreatePayloadClaims(
        HelseIdConfiguration configuration,
        PayloadClaimParameters payloadClaimParameters);
}
