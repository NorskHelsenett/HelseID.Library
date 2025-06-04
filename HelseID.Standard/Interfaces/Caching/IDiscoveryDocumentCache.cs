using HelseID.Standard.Models;

namespace HelseID.Standard.Interfaces.Caching;

public interface IDiscoveryDocumentCache
{
    Task<DiscoveryDocument?> GetDiscoveryDocument();
    Task AddDiscoveryDocumentToCache(DiscoveryDocument discoveryDocument);
}
