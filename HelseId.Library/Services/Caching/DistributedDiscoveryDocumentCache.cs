using System.Text.Json;
using HelseId.Library.Interfaces.Caching;
using HelseId.Library.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace HelseId.Library.Services.Caching;

public class DistributedDiscoveryDocumentCache : IDiscoveryDocumentCache
{
    private IDistributedCache _cache;

    private const string DiscoveryDocumentKey = "DiscoveryDocument";

    public DistributedDiscoveryDocumentCache(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<DiscoveryDocument?> GetDiscoveryDocument()
    {
        var discoveryDocumentBytes = await _cache.GetAsync(DiscoveryDocumentKey);
        if (discoveryDocumentBytes == null || discoveryDocumentBytes.Length == 0)
        {
            return null;
        }
        
        return JsonSerializer.Deserialize<DiscoveryDocument>(discoveryDocumentBytes);
    }

    public async Task AddDiscoveryDocumentToCache(DiscoveryDocument discoveryDocument)
    {
        var discoveryDocumentBytes = JsonSerializer.SerializeToUtf8Bytes(discoveryDocument);
        await _cache.SetAsync(DiscoveryDocumentKey, discoveryDocumentBytes, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
        });

    }
}
