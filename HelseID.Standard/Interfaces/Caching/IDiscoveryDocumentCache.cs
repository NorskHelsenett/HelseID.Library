using HelseId.Standard.Models;

namespace HelseId.Standard.Interfaces.Caching;

public interface IDiscoveryDocumentCache
{
    Task<DiscoveryDocument?> GetDiscoveryDocument();
    Task AddDiscoveryDocumentToCache(DiscoveryDocument discoveryDocument);
}
