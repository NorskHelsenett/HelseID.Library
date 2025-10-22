using HelseId.Library.Mocks;
using HelseId.Library.Tests.Configuration;
using HelseId.Library.Tests.Mocks;

namespace HelseId.Library.Tests.Services.TokenRequests;

public abstract class TokenRequestBuilderTests : ConfigurationTests
{
    public const string DPoPProof = "dpop proof";
    
    protected SigningTokenCreatorMock SigningTokenCreatorMock { get; set; } = null!;
    protected IdPoPProofCreatorMock DpoPProofCreatorMock { get; set; } = null!;
    protected HelseIdEndpointsDiscovererMock HelseIdEndpointsDiscovererMock { get; set; } = null!;
    protected PayloadClaimsCreatorMock PayloadClaimsCreatorMock { get; set; } = null!;

    [SetUp]
    public void SetupForTokenRequestBuilderTest()
    {
        SigningTokenCreatorMock = new SigningTokenCreatorMock();
        DpoPProofCreatorMock = new IdPoPProofCreatorMock(DPoPProof);
        HelseIdEndpointsDiscovererMock = new HelseIdEndpointsDiscovererMock();
        PayloadClaimsCreatorMock = new PayloadClaimsCreatorMock();
    }
}
