using System.Text.Json;
using FluentAssertions;
using HelseID.Standard.Models;
using HelseID.Standard.Services.Caching;
using HelseID.Standard.Tests.Mocks;

namespace HelseID.Standard.Tests.Services.Caching;

[TestFixture]
public class DistributedDiscoveryDocumentCacheTests
{
    private DistributedMemoryCacheMock _distributedMemoryCacheMock = null!;
    private DistributedDiscoveryDocumentCache _discoveryDocumentCache = null!;
    
    [SetUp]
    public void Setup()
    {
        _distributedMemoryCacheMock = new DistributedMemoryCacheMock();
        _discoveryDocumentCache = new DistributedDiscoveryDocumentCache(_distributedMemoryCacheMock);
    }

    [Test]
    public async Task GetDiscoveryDocument_returns_cached_document()
    {
        var expectedDiscoveryDocument = new DiscoveryDocument
        {
            AuthorizeEndpoint = "authorize",
            TokenEndpoint = "token"
        };

        _distributedMemoryCacheMock.SetCachedDataFromObject(expectedDiscoveryDocument);
        
        var discoveryDocument = await _discoveryDocumentCache.GetDiscoveryDocument();
        discoveryDocument.Should().BeEquivalentTo(expectedDiscoveryDocument);
    }
    
    [Test]
    public async Task GetDiscoveryDocument_caches_document()
    {
        var expectedDiscoveryDocument = new DiscoveryDocument
        {
            AuthorizeEndpoint = "authorize",
            TokenEndpoint = "token"
        };

        await _discoveryDocumentCache.AddDiscoveryDocumentToCache(expectedDiscoveryDocument);

        _distributedMemoryCacheMock.CachedObject.Should().NotBeEmpty();

        var cachedDocument = JsonSerializer.Deserialize<DiscoveryDocument>(_distributedMemoryCacheMock.CachedObject);
        cachedDocument.Should().BeEquivalentTo(expectedDiscoveryDocument);
    }
}
