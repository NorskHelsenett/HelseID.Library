using HelseID.Standard.Interfaces.Caching;
using HelseID.Standard.Models;

namespace HelseID.Standard.Tests.Services.Caching;

public class DiscoveryDocumentCacheMock : IDiscoveryDocumentCache
{
    public Task<DiscoveryDocument?> GetDiscoveryDocument()
    {
        return Task.FromResult(CachedData);
    }

    public Task AddDiscoveryDocumentToCache(DiscoveryDocument discoveryDocument)
    {
        CachedData = discoveryDocument;
        return Task.CompletedTask;
    }

    public DiscoveryDocument? CachedData { get; private set; }
}
