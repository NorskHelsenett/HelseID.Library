using HelseId.Library.Mocks;
using HelseId.Library.Tests.Configuration;
using HelseId.Library.Tests.Mocks;

namespace HelseId.Library.Tests.Services.TokenRequests;

public abstract class TokenRequestBuilderTests : ConfigurationTests
{
    public const string DPoPProof = "dpop proof";
    
    protected SigningTokenCreatorMock SigningTokenCreatorMock { get; set; } = null!;
    protected DPoPProofCreatorForTokenRequestMock DpoPProofCreatorMock { get; set; } = null!;
    protected HelseIdEndpointsDiscovererMock HelseIdEndpointsDiscovererMock { get; set; } = null!;
    protected PayloadClaimsCreatorMock PayloadClaimsCreatorMock { get; set; } = null!;

    [SetUp]
    public void SetupForTokenRequestBuilderTest()
    {
        SigningTokenCreatorMock = new SigningTokenCreatorMock();
        DpoPProofCreatorMock = new DPoPProofCreatorForTokenRequestMock(DPoPProof);
        HelseIdEndpointsDiscovererMock = new HelseIdEndpointsDiscovererMock();
        PayloadClaimsCreatorMock = new PayloadClaimsCreatorMock();
    }
}


public class DPoPProofCreatorForTokenRequestMock : IDPoPProofCreator
{
    public string? Url { get; private set; }
    public string? HttpMethod { get; private set; }
    public string? DPoPNonce { get; private set; }
    public string? AccessToken { get; private set; }

    private readonly string _dPoPProof;
    
    public DPoPProofCreatorForTokenRequestMock(string dPoPProof)
    {
        _dPoPProof = dPoPProof;
    }
    
    public Task<string> CreateDPoPProofForTokenRequest(string url, string httpMethod, string? dPoPNonce = null)
    {
        Url = url;
        HttpMethod = httpMethod;
        DPoPNonce = dPoPNonce;
        return Task.FromResult(_dPoPProof);
    }
}
