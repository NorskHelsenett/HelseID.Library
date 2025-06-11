using HelseId.Library.Models;

namespace HelseId.Library.Interfaces.Caching;

public interface ITokenCache
{
    Task<AccessTokenResponse?> GetAccessToken(string cacheKey);
    Task AddTokenToCache(string cacheKey, AccessTokenResponse tokenResponse);
}
