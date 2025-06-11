using HelseId.Library.Configuration;
using HelseId.Library.Models.Payloads;

namespace HelseId.Library.Interfaces.PayloadClaimCreators;

public interface IPayloadClaimsCreator
{
    Dictionary<string, object> CreatePayloadClaims(
        HelseIdConfiguration configuration,
        PayloadClaimParameters payloadClaimParameters);
}
