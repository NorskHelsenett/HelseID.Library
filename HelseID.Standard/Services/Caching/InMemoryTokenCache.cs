using HelseID.Standard.Interfaces.Caching;
using HelseID.Standard.Models;
using HelseID.Standard.Models.Constants;
using Microsoft.Extensions.Caching.Memory;

namespace HelseID.Standard.Services.Caching;

public class InMemoryTokenCache : ITokenCache
{
    private readonly IMemoryCache _memoryCache;

    public InMemoryTokenCache(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public Task<AccessTokenResponse?> GetAccessToken(string cacheKey)
    {
        if (_memoryCache.TryGetValue(cacheKey, out AccessTokenResponse? cachedTokenRespone))
        {
            return Task.FromResult(cachedTokenRespone);
        }

        return Task.FromResult<AccessTokenResponse?>(null);
    }

    public Task AddTokenToCache(string cacheKey, AccessTokenResponse tokenResponse)
    {
        var expiration = DateTimeOffset.Now.AddSeconds(tokenResponse.ExpiresIn - HelseIdConstants.TokenResponseLeewayInSeconds);
        _memoryCache.Set(cacheKey, tokenResponse, expiration);
     
        return Task.CompletedTask;
    }
}
