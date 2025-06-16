using HelseId.Library.Tests.Mocks;

namespace HelseId.Library.Tests.Services.Endpoints;

[TestFixture]
public class HelseIdEndpointsDiscovererTests
{
    private HelseIdEndpointsDiscoverer _helseIdEndpointsDiscoverer = null!;

    [SetUp]
    public void Setup()
    {
        _helseIdEndpointsDiscoverer = new HelseIdEndpointsDiscoverer(new DiscoveryDocumentGetterMock());
    }

    [Test]
    public async Task GetTokenEndpointFromHelseId_returns_endpoint()
    {
        var tokenEndpoint = await _helseIdEndpointsDiscoverer.GetTokenEndpointFromHelseId();

        tokenEndpoint.Should().NotBeNullOrEmpty();
        tokenEndpoint.Should().Be("https://helseid-sts.nhn.no/connect/token");
    }
}
