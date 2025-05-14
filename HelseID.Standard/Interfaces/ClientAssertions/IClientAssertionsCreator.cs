using HelseID.Standard.Interfaces.PayloadClaimCreators;
using HelseID.Standard.Models.Payloads;
namespace HelseID.Standard.Interfaces.ClientAssertions;

public interface IClientAssertionsCreator
{
    string CreateClientAssertion(IPayloadClaimsCreator payloadClaimsCreator, PayloadClaimParameters payloadClaimParameters);
}
