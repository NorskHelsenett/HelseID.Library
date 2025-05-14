using HelseID.Standard.Models;

namespace HelseID.Standard.Interfaces.Endpoints;

public interface IDiscoveryDocumentGetter
{
    Task<DiscoveryDocument> GetDiscoveryDocument();
}
