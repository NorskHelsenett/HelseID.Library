using HelseId.Standard.Models;

namespace HelseId.Standard.Interfaces.Endpoints;

public interface IDiscoveryDocumentGetter
{
    Task<DiscoveryDocument> GetDiscoveryDocument();
}
