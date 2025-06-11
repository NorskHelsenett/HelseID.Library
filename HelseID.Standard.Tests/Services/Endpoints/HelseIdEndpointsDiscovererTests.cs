using FluentAssertions;
using HelseId.Standard.Services.Endpoints;
using HelseId.Standard.Tests.Mocks;

namespace HelseId.Standard.Tests.Services.Endpoints;

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
