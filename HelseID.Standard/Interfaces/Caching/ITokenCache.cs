using HelseId.Standard.Models;

namespace HelseId.Standard.Interfaces.Caching;

public interface ITokenCache
{
    Task<AccessTokenResponse?> GetAccessToken(string cacheKey);
    Task AddTokenToCache(string cacheKey, AccessTokenResponse tokenResponse);
}
