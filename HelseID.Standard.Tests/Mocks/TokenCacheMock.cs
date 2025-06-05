using System.Text.Json;
using HelseID.Standard.Interfaces.Caching;
using HelseID.Standard.Models;

namespace HelseID.Standard.Tests.Services.Caching;

public class TokenCacheMock : ITokenCache
{
    public string LastKeyGet { get; set; } = "";
    public string LastKeySet { get; set; } = "";
    public byte[] CachedData { get; set; } = [];

    public Task<AccessTokenResponse?> GetAccessToken(string cacheKey)
    {
        LastKeyGet = cacheKey;
        if (CachedData.Length == 0)
        {
            return Task.FromResult<AccessTokenResponse?>(null);
        }

        return Task.FromResult(JsonSerializer.Deserialize<AccessTokenResponse?>(CachedData));
    }

    public Task AddTokenToCache(string cacheKey, AccessTokenResponse tokenResponse)
    {
        LastKeySet = cacheKey;
        CachedData = JsonSerializer.SerializeToUtf8Bytes(tokenResponse);
        return Task.CompletedTask;
    }

    public void SetCachedDataFromObject(AccessTokenResponse tokenResponse)
    {
        CachedData = JsonSerializer.SerializeToUtf8Bytes(tokenResponse);
    }

    public void ResetCachedData()
    {
        CachedData = [];
    }
}
