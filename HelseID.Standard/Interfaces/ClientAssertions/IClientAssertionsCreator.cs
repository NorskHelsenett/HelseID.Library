using HelseId.Standard.Interfaces.PayloadClaimCreators;
using HelseId.Standard.Models.Payloads;

namespace HelseId.Standard.Interfaces.ClientAssertions;

public interface IClientAssertionsCreator
{
    string CreateClientAssertion(IPayloadClaimsCreator payloadClaimsCreator, PayloadClaimParameters payloadClaimParameters);
}
