using System.Text.Json;
using HelseID.Standard.Interfaces.Caching;
using HelseID.Standard.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace HelseID.Standard.Services.Caching;

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
