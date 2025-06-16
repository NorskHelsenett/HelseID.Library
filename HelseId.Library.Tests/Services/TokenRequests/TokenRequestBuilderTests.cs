using HelseId.Library.Tests.Configuration;
using HelseId.Library.Tests.Mocks;

namespace HelseId.Library.Tests.Services.TokenRequests;

public abstract class TokenRequestBuilderTests : ConfigurationTests
{
    protected SigningTokenCreatorMock SigningTokenCreatorMock { get; set; } = null!;
    protected DPoPProofCreatorMock DpoPProofCreatorMock { get; set; } = null!;
    protected HelseIdEndpointsDiscovererMock HelseIdEndpointsDiscovererMock { get; set; } = null!;
    protected PayloadClaimsCreatorMock PayloadClaimsCreatorMock { get; set; } = null!;

    [SetUp]
    public void SetupForTokenRequestBuilderTest()
    {
        SigningTokenCreatorMock = new SigningTokenCreatorMock();
        DpoPProofCreatorMock = new DPoPProofCreatorMock();
        HelseIdEndpointsDiscovererMock = new HelseIdEndpointsDiscovererMock();
        PayloadClaimsCreatorMock = new PayloadClaimsCreatorMock();
    }
}
