using FluentAssertions;
using HelseID.Standard.Tests.Mocks;
using IdentityModel.Client;

namespace HelseID.Standard.Tests.Services.Endpoints;

[TestFixture]
public class DiscoveryDocumentGetterTests
{
    private const string StsUrl = "https://helseid-sts.nhn.no";
    private MemoryCacheMock _memoryCacheMock = null!;
    private DiscoveryDocumentGetterWithMockHttpClient _discoveryDocumentGetter = null!;
    
    [SetUp]
    public void Setup()
    {
        _memoryCacheMock = new MemoryCacheMock();
        _discoveryDocumentGetter = new DiscoveryDocumentGetterWithMockHttpClient(StsUrl, _memoryCacheMock);
    }

    [TearDown]
    public void TearDown()
    {
        _memoryCacheMock.Dispose();
    }

    [Test]
    public async Task GetDiscoveryDocument_calls_the_endpoint()
    {
        var result = await _discoveryDocumentGetter.GetDiscoveryDocument();

        result.Should().NotBeNull();
        result.AuthorizeEndpoint.Should().Be("https://helseid-sts.nhn.no/connect/authorize");
        _discoveryDocumentGetter.SetupClient.Should().BeTrue();
    }
    
    [Test]
    public async Task GetDiscoveryDocument_caches_the_response()
    {
        await _discoveryDocumentGetter.GetDiscoveryDocument();

        _memoryCacheMock.Entries.Should().HaveCount(1);
        MockCacheEntry cacheEntry = _memoryCacheMock.Entries["DiscoveryDocument"];
        cacheEntry.Should().NotBeNull();
        DiscoveryDocumentResponse? response = cacheEntry.Value as DiscoveryDocumentResponse;
        response!.AuthorizeEndpoint.Should().Be("https://helseid-sts.nhn.no/connect/authorize");
    }
    
    [Test]
    public async Task GetDiscoveryDocument_does_not_call_the_endpoint_when_the_cache_is_set()
    {
        var result = await _discoveryDocumentGetter.GetDiscoveryDocument();

        result.Should().NotBeNull();
        result.AuthorizeEndpoint.Should().Be("https://helseid-sts.nhn.no/connect/authorize");
        _discoveryDocumentGetter.SetupClient.Should().BeTrue();
        _discoveryDocumentGetter.SetupClient = false;
        
        result = await _discoveryDocumentGetter.GetDiscoveryDocument();
        result.Should().NotBeNull();
        result.AuthorizeEndpoint.Should().Be("https://helseid-sts.nhn.no/connect/authorize");
        _discoveryDocumentGetter.SetupClient.Should().BeFalse();
    }
}
