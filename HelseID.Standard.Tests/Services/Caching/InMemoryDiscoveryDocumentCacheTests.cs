using FluentAssertions;
using HelseID.Standard.Models;
using HelseID.Standard.Services.Caching;
using HelseID.Standard.Tests.Mocks;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Time.Testing;

namespace HelseID.Standard.Tests.Services.Caching;

[TestFixture]
public class InMemoryDiscoveryDocumentCacheTests
{
    private InMemoryDiscoveryDocumentCache _discoveryDocumentCache = null!;
    private FakeTimeProvider _fakeTimeProvider
        ;

    [SetUp]
    public void Setup()
    {
        _fakeTimeProvider = new FakeTimeProvider();
        
        _discoveryDocumentCache = new InMemoryDiscoveryDocumentCache(_fakeTimeProvider);
    }

    [Test]
    public async Task GetAccessToken_returns_cached_document()
    {
        var cachedDiscoveryDocument = new DiscoveryDocument
        {
            TokenEndpoint = "token endpoint",
            AuthorizeEndpoint = "authorize endpoint"
        };

        await _discoveryDocumentCache.AddDiscoveryDocumentToCache(cachedDiscoveryDocument);
        
        var discoveryDocumentResponse = await _discoveryDocumentCache.GetDiscoveryDocument();
        discoveryDocumentResponse.Should().BeEquivalentTo(cachedDiscoveryDocument);
    }

    [Test]
    public async Task GetAccessToken_returns_null_if_no_document_is_found_in_cache()
    {
        (await _discoveryDocumentCache.GetDiscoveryDocument()).Should().BeNull();
    }
    
    [Test]
    public async Task GetAccessToken_returns_null_if_document_is_expired()
    {
        var cachedDiscoveryDocument = new DiscoveryDocument
        {
            TokenEndpoint = "token endpoint",
            AuthorizeEndpoint = "authorize endpoint"
        };

        await _discoveryDocumentCache.AddDiscoveryDocumentToCache(cachedDiscoveryDocument);

        _fakeTimeProvider.Advance(TimeSpan.FromDays(2));
        
        (await _discoveryDocumentCache.GetDiscoveryDocument()).Should().BeNull();
    }
}
