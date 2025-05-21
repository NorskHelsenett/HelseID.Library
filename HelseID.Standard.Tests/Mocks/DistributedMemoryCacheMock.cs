using Microsoft.Extensions.Caching.Distributed;

namespace HelseID.Standard.Tests.Mocks;

public class DistributedMemoryCacheMock : IDistributedCache
{
    public string LastKeyGet { get; set; } = "";
    public string LastKeySet { get; set; } = "";
    public byte[] CachedData { get; set; } = [];

    public DistributedCacheEntryOptions CacheEntryOptions { get; set; } = null!;
    
    public byte[]? Get(string key)
    {
        LastKeyGet = key;
        return CachedData;
    }

    public Task<byte[]?> GetAsync(string key, CancellationToken token = new CancellationToken())
    {
        return Task.FromResult(Get(key));
    }

    public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
    {
        LastKeySet = key;
        CachedData = value;
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
