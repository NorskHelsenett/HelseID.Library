using FluentAssertions;
using HelseID.Standard.Models;
using HelseID.Standard.Services.Caching;
using HelseID.Standard.Tests.Mocks;

namespace HelseID.Standard.Tests.Services.Caching;

[TestFixture]
public class InMemoryDiscoveryDocumentCacheTests : IDisposable
{
    private MemoryCacheMock _memoryCacheMock = null!;
    private InMemoryDiscoveryDocumentCache _discoveryDocumentCache = null!;
    
    [SetUp]
    public void Setup()
    {
        _memoryCacheMock = new MemoryCacheMock();
        _discoveryDocumentCache = new InMemoryDiscoveryDocumentCache(_memoryCacheMock);
    }

    [Test]
    public async Task GetAccessToken_returns_cached_document()
    {
        var cachedDiscoveryDocument = new DiscoveryDocument
        {
            TokenEndpoint = "token endpoint",
            AuthorizeEndpoint = "authorize endpoint"
        };

        _memoryCacheMock.SetCachedObject("DiscoveryDocument", cachedDiscoveryDocument);

        var accessTokenResponse = await _discoveryDocumentCache.GetDiscoveryDocument();
        accessTokenResponse.Should().BeEquivalentTo(cachedDiscoveryDocument);
    }
    
    [Test]
    public async Task GetAccessToken_caches_document()
    {
        var cachedDiscoveryDocument = new DiscoveryDocument
        {
            TokenEndpoint = "token endpoint",
            AuthorizeEndpoint = "authorize endpoint"
        };

        await _discoveryDocumentCache.AddDiscoveryDocumentToCache(cachedDiscoveryDocument);
        
        (await _discoveryDocumentCache.GetDiscoveryDocument()).Should().BeSameAs(cachedDiscoveryDocument);
        
        var cacheEntry =  _memoryCacheMock.Entries.Single();
        cacheEntry.Value.Value.Should().BeSameAs(cachedDiscoveryDocument);
    }

    public void Dispose()
    {
        _memoryCacheMock.Dispose();
    }
}
