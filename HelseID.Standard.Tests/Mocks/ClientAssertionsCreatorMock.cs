using HelseID.Standard.Interfaces.ClientAssertions;
using HelseID.Standard.Interfaces.PayloadClaimCreators;
using HelseID.Standard.Models.Payloads;
using IdentityModel.Client;

namespace HelseID.Standard.Tests.Mocks;

public class ClientAssertionsCreatorMock : IClientAssertionsCreator
{
    public const string Value = "Client assertion value";

    public IPayloadClaimsCreator PayloadClaimsCreator { get; set; } = null!;
    public PayloadClaimParameters PayloadClaimParameters { get; set; } = null!;
    
    public string CreateClientAssertion(IPayloadClaimsCreator payloadClaimsCreator, PayloadClaimParameters payloadClaimParameters)
    {
        PayloadClaimsCreator = payloadClaimsCreator;
        PayloadClaimParameters = payloadClaimParameters;

        return Value;
    }
}
