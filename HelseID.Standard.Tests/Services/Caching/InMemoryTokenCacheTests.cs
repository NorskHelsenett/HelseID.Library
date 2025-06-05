using System.Text.Json;
using FluentAssertions;
using HelseID.Standard.Models;
using HelseID.Standard.Services.Caching;
using HelseID.Standard.Tests.Mocks;

namespace HelseID.Standard.Tests.Services.Caching;

[TestFixture]
public class InMemoryTokenCacheTests : IDisposable
{
    private MemoryCacheMock _memoryCacheMock = null!;
    private InMemoryTokenCache _tokenCache = null!;
    
    [SetUp]
    public void Setup()
    {
        _memoryCacheMock = new MemoryCacheMock();
        _tokenCache = new InMemoryTokenCache(_memoryCacheMock);
    }

    [Test]
    public async Task GetAccessToken_returns_cached_document()
    {
        var expectedTokenRepsonse = new AccessTokenResponse
        {
            AccessToken = "access token",
            ExpiresIn = 123
        };

        _memoryCacheMock.SetCachedObject("cachekey", expectedTokenRepsonse);

        var accessTokenResponse = await _tokenCache.GetAccessToken("cachekey");
        accessTokenResponse.Should().BeEquivalentTo(expectedTokenRepsonse);
    }
    
    [Test]
    public async Task GetAccessToken_caches_document()
    {
        var cachedTokenResponse = new AccessTokenResponse
        {
            AccessToken = "access token",
            ExpiresIn = 123
        };

        await _tokenCache.AddTokenToCache("cachekey", cachedTokenResponse);
        
        (await _tokenCache.GetAccessToken("cachekey")).Should().BeSameAs(cachedTokenResponse);
        
        var cacheEntry =  _memoryCacheMock.Entries.Single();
        cacheEntry.Key.Should().Be("cachekey");
        cacheEntry.Value.Value.Should().BeSameAs(cachedTokenResponse);
    }

    public void Dispose()
    {
        _memoryCacheMock.Dispose();
    }
}
