using Microsoft.Extensions.Time.Testing;

namespace HelseId.Standard.Tests.Services.Caching;

[TestFixture]
public class InMemoryTokenCacheTests
{
    private InMemoryTokenCache _tokenCache = null!;
    private FakeTimeProvider _fakeTimeProvider = null!;
    
    [SetUp]
    public void Setup()
    {
        _fakeTimeProvider = new FakeTimeProvider();
        _tokenCache = new InMemoryTokenCache(_fakeTimeProvider);
    }

    [Test]
    public async Task GetAccessToken_returns_cached_document()
    {
        var expectedTokenRepsonse = new AccessTokenResponse
        {
            AccessToken = "access token",
            ExpiresIn = 1234
        };

        await _tokenCache.AddTokenToCache("cachekey", expectedTokenRepsonse);

        var accessTokenResponse = await _tokenCache.GetAccessToken("cachekey");
        accessTokenResponse.Should().BeSameAs(expectedTokenRepsonse);
    }
    
    [Test]
    public async Task GetAccessToken_returns_null_if_no_document_found_in_cache()
    {
        (await _tokenCache.GetAccessToken("cachekey")).Should().BeNull();
    }
    
    [Test]
    public async Task GetAccessToken_returns_null_document_found_in_cache_has_expired()
    {
        var expectedTokenRepsonse = new AccessTokenResponse
        {
            AccessToken = "access token",
            ExpiresIn = 60
        };

        await _tokenCache.AddTokenToCache("cachekey", expectedTokenRepsonse);

        _fakeTimeProvider.Advance(TimeSpan.FromMinutes(2));
        
        (await _tokenCache.GetAccessToken("cachekey")).Should().BeNull();
    }
}
