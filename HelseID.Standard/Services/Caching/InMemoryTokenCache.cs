using HelseId.Standard.Interfaces.Caching;
using HelseId.Standard.Models;
using HelseId.Standard.Models.Constants;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Internal;

namespace HelseId.Standard.Services.Caching;

public class InMemoryTokenCache : ITokenCache
{
    private readonly Dictionary<string, TokenResponseContainer?> _cache = new();
    private readonly TimeProvider _timeProvider;

    public InMemoryTokenCache(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public Task<AccessTokenResponse?> GetAccessToken(string cacheKey)
    {
        if (_cache.TryGetValue(cacheKey, out TokenResponseContainer? cachedTokenResponse))
        {
            if (cachedTokenResponse == null)
            {
                return Task.FromResult<AccessTokenResponse?>(null);
            }

            if (cachedTokenResponse.Expires > _timeProvider.GetUtcNow())
            {
                return Task.FromResult<AccessTokenResponse?>(cachedTokenResponse.TokenResponse);
            }

            _cache.Remove(cacheKey);
        }

        return Task.FromResult<AccessTokenResponse?>(null);
    }

    public Task AddTokenToCache(string cacheKey, AccessTokenResponse tokenResponse)
    {
        _cache[cacheKey] = new TokenResponseContainer(tokenResponse, _timeProvider);
        return Task.CompletedTask;
    }

    private sealed class TokenResponseContainer
    {
        public AccessTokenResponse TokenResponse { get; private set; }
        public DateTimeOffset Expires { get; }

        public TokenResponseContainer(AccessTokenResponse accessTokenResponse, TimeProvider timeProvider)
        {
            TokenResponse = accessTokenResponse;
            Expires = timeProvider.GetUtcNow().AddSeconds(accessTokenResponse.ExpiresIn - HelseIdConstants.TokenResponseLeewayInSeconds);
        }
    }
}
