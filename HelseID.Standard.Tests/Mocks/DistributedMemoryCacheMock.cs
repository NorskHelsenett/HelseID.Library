using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace HelseId.Standard.Tests.Mocks;

public class DistributedMemoryCacheMock : IDistributedCache
{
    public string LastKeyGet { get; set; } = "";
    public string LastKeySet { get; set; } = "";
    public byte[] CachedObject { get; set; } = [];

    public void SetCachedDataFromObject(object cachedObject)
    {
        CachedObject = JsonSerializer.SerializeToUtf8Bytes(cachedObject);
    }

    public void ResetCachedData()
    {
        CachedObject = [];
    }

    public DistributedCacheEntryOptions CacheEntryOptions { get; set; } = null!;
    
    public byte[]? Get(string key)
    {
        LastKeyGet = key;
        return CachedObject;
    }

    public Task<byte[]?> GetAsync(string key, CancellationToken token = new CancellationToken())
    {
        return Task.FromResult(Get(key));
    }

    public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
    {
        LastKeySet = key;
        CachedObject = value;
        CacheEntryOptions = options;
    }

    public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = new())
    {
        Set(key, value, options);
        return Task.CompletedTask;
    }

    public void Refresh(string key)
    {
        throw new NotImplementedException();
    }

    public Task RefreshAsync(string key, CancellationToken token = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public void Remove(string key)
    {
        throw new NotImplementedException();
    }

    public Task RemoveAsync(string key, CancellationToken token = new CancellationToken())
    {
        throw new NotImplementedException();
    }
}
