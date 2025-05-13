using HelseID.Standard.Interfaces.PayloadClaimCreators;
using HelseID.Standard.Models.Payloads;
using IdentityModel.Client;

namespace HelseID.Standard.Interfaces.ClientAssertions;

public interface IClientAssertionsCreator
{
    string CreateClientAssertion(IPayloadClaimsCreator payloadClaimsCreator, PayloadClaimParameters payloadClaimParameters);
}
