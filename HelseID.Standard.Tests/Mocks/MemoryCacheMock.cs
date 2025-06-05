using Microsoft.Extensions.Caching.Memory;

namespace HelseID.Standard.Tests.Mocks;

public class MemoryCacheMock : IMemoryCache
{
    public bool ReturnedValue { get; private set; }

    public Dictionary<object, MockCacheEntry> Entries { get; private set; } = new();

    public void SetCachedObject(string cacheKey, object cachedObject)
    {
        Entries.Add(cacheKey, new MockCacheEntry { Value = cachedObject});
    }
    
    public ICacheEntry CreateEntry(object key)
    {
        var value = new MockCacheEntry() { Key = key };
        Entries[key] = value;
        return value;
    }

    public void Remove(object key)
    {
        Entries.Remove(key);
    }

    public bool TryGetValue(object key, out object? value)
    {
        if (Entries.TryGetValue(key, out var entry))
        {
            value = entry.Value;
            ReturnedValue = true;
            return true;
        }

        value = null;
        return false;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
