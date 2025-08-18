using HelseId.Library.Tests.Mocks;

namespace HelseId.Library.Tests.Services.Caching;

[TestFixture]
public class DistributedTokenCacheTests
{
    private DistributedMemoryCacheMock _distributedMemoryCacheMock = null!;
    private DistributedTokenCache _tokenCache = null!;
    
    [SetUp]
    public void Setup()
    {
        _distributedMemoryCacheMock = new DistributedMemoryCacheMock();
        _tokenCache = new DistributedTokenCache(_distributedMemoryCacheMock);
    }

    [Test]
    public async Task GetAccessToken_returns_cached_document()
    {
        var expectedTokenRepsonse = new AccessTokenResponse
        {
            AccessToken = "access token",
            ExpiresIn = 123
        };

        _distributedMemoryCacheMock.SetCachedDataFromObject(expectedTokenRepsonse);

        var accessTokenResponse = await _tokenCache.GetAccessToken("cachekey");
        accessTokenResponse.Should().BeEquivalentTo(expectedTokenRepsonse);

        _distributedMemoryCacheMock.LastKeyGet.Should().Be("cachekey");
    }
    
    [Test]
    public async Task GetAccessToken_caches_document()
    {
        var expectedTokenRepsonse = new AccessTokenResponse
        {
            AccessToken = "access token",
            ExpiresIn = 123
        };

        await _tokenCache.AddTokenToCache("cachekey", expectedTokenRepsonse);

        _distributedMemoryCacheMock.LastKeySet.Should().Be("cachekey");
        _distributedMemoryCacheMock.CachedObject.Should().NotBeEmpty();

        var cachedTokenResponse = JsonSerializer.Deserialize<AccessTokenResponse>(_distributedMemoryCacheMock.CachedObject);
        cachedTokenResponse.Should().BeEquivalentTo(expectedTokenRepsonse);
    }
}
