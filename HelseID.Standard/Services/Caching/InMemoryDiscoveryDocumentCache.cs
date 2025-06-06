using HelseID.Standard.Interfaces.Caching;
using HelseID.Standard.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Internal;

namespace HelseID.Standard.Services.Caching;

public class InMemoryDiscoveryDocumentCache : IDiscoveryDocumentCache
{
    private const string DiscoveryDocumentKey = "DiscoveryDocument";

    private readonly Dictionary<string, DiscoveryDocumentContainer?> _cache = new();
    private readonly TimeProvider _timeProvider;

    public InMemoryDiscoveryDocumentCache(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public Task<DiscoveryDocument?> GetDiscoveryDocument()
    {
        if (_cache.TryGetValue(DiscoveryDocumentKey, out DiscoveryDocumentContainer? discoveryDocumentContainer))
        {
            if (discoveryDocumentContainer == null)
            {
                return Task.FromResult<DiscoveryDocument?>(null);
            }

            if (discoveryDocumentContainer.Expires > _timeProvider.GetUtcNow())
            {
                return Task.FromResult<DiscoveryDocument?>(discoveryDocumentContainer.DiscoveryDocument);
            }
        }

        return Task.FromResult<DiscoveryDocument?>(null);
    }

    public Task AddDiscoveryDocumentToCache(DiscoveryDocument discoveryDocument)
    {
        _cache[DiscoveryDocumentKey] = new DiscoveryDocumentContainer(discoveryDocument, _timeProvider);
        return Task.CompletedTask;
    }

    private sealed class DiscoveryDocumentContainer
    {
        public DiscoveryDocument DiscoveryDocument { get; private set; }
        public DateTimeOffset Expires { get; }

        public DiscoveryDocumentContainer(DiscoveryDocument discoveryDocument, TimeProvider timeProvider)
        {
            DiscoveryDocument = discoveryDocument;
            Expires = timeProvider.GetUtcNow().AddHours(24);
        }
    }
}
