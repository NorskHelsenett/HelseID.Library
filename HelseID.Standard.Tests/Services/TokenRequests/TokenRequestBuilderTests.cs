using HelseID.Standard.Tests.Configuration;
using HelseID.Standard.Tests.Mocks;

namespace HelseID.Standard.Tests.Services.TokenRequests;

public abstract class TokenRequestBuilderTests : ConfigurationTests
{
    protected ClientAssertionsCreatorMock ClientAssertionsCreatorMock { get; set; } = null!;
    protected DPoPProofCreatorMock DpoPProofCreatorMock { get; set; } = null!;
    protected HelseIdEndpointsDiscovererMock HelseIdEndpointsDiscovererMock { get; set; } = null!;
    protected PayloadClaimsCreatorMock PayloadClaimsCreatorMock { get; set; } = null!;

    [SetUp]
    public void SetupForTokenRequestBuilderTest()
    {
        ClientAssertionsCreatorMock = new ClientAssertionsCreatorMock();
        DpoPProofCreatorMock = new DPoPProofCreatorMock();
        HelseIdEndpointsDiscovererMock = new HelseIdEndpointsDiscovererMock();
        PayloadClaimsCreatorMock = new PayloadClaimsCreatorMock();
    }
}
