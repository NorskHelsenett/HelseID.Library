namespace HelseId.Library.Services.Caching;

public class InMemoryDiscoveryDocumentCache : IDiscoveryDocumentCache
{
    private DiscoveryDocumentContainer? _discoveryDocumentContainer;
    private readonly TimeProvider _timeProvider;

    public InMemoryDiscoveryDocumentCache(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public Task<DiscoveryDocument?> GetDiscoveryDocument()
    {
        if (_discoveryDocumentContainer == null)
        {
            return Task.FromResult<DiscoveryDocument?>(null);
        }

        if (_discoveryDocumentContainer.Expires > _timeProvider.GetUtcNow())
        {
            return Task.FromResult<DiscoveryDocument?>(_discoveryDocumentContainer.DiscoveryDocument);
        }

        return Task.FromResult<DiscoveryDocument?>(null);
    }

    public Task AddDiscoveryDocumentToCache(DiscoveryDocument discoveryDocument)
    {
        _discoveryDocumentContainer = new DiscoveryDocumentContainer(discoveryDocument, _timeProvider);
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
