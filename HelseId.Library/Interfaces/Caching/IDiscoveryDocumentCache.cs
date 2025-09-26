namespace HelseId.Library.Interfaces.Caching;

public interface IDiscoveryDocumentCache
{
    Task<DiscoveryDocument?> GetDiscoveryDocument();
    Task AddDiscoveryDocumentToCache(DiscoveryDocument discoveryDocument);
}
