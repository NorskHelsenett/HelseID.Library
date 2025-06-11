using System.Text.Json;
using HelseId.Library.Interfaces.Caching;
using HelseId.Library.Models;
using HelseId.Library.Models.Constants;
using Microsoft.Extensions.Caching.Distributed;

namespace HelseId.Library.Services.Caching;

public class DistributedTokenCache : ITokenCache
{
    private readonly IDistributedCache _cache;

    public DistributedTokenCache(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<AccessTokenResponse?> GetAccessToken(string cacheKey)
    {
        var cachedTokenResponse = await _cache.GetAsync(cacheKey);
        if (cachedTokenResponse == null || cachedTokenResponse.Length == 0)
        {
            return null;
        }

        return JsonSerializer.Deserialize<AccessTokenResponse>(cachedTokenResponse);
    }
    
    public async Task AddTokenToCache(string cacheKey, AccessTokenResponse tokenResponse)
    {
        var serializedTokenResponse = JsonSerializer.SerializeToUtf8Bytes(tokenResponse);
        await _cache.SetAsync(cacheKey,
            serializedTokenResponse,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(tokenResponse.ExpiresIn - HelseIdConstants.TokenResponseLeewayInSeconds)
            });
    }
}
