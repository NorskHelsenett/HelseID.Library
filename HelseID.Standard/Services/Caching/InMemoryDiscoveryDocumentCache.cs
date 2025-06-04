using HelseID.Standard.Interfaces.Caching;
using HelseID.Standard.Models;
using Microsoft.Extensions.Caching.Memory;

namespace HelseID.Standard.Services.Caching;

public class InMemoryDiscoveryDocumentCache : IDiscoveryDocumentCache
{
    private readonly IMemoryCache _cache;

    private const string DiscoveryDocumentKey = "DiscoveryDocument";

    public InMemoryDiscoveryDocumentCache(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task<DiscoveryDocument?> GetDiscoveryDocument()
    {
        if (_cache.TryGetValue(DiscoveryDocumentKey, out DiscoveryDocument? discoveryDocument))
        {
            return Task.FromResult(discoveryDocument);
        }

        return Task.FromResult<DiscoveryDocument?>(null);
    }

    public Task AddDiscoveryDocumentToCache(DiscoveryDocument discoveryDocument)
    {
        var expiration = DateTimeOffset.Now.AddHours(24);
        _cache.Set(DiscoveryDocumentKey, discoveryDocument, expiration);
        return Task.CompletedTask;
    }
}
