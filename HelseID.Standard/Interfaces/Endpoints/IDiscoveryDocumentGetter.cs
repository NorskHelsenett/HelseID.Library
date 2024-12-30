using IdentityModel.Client;

namespace HelseID.Standard.Interfaces.Endpoints;

public interface IDiscoveryDocumentGetter
{
    Task<DiscoveryDocumentResponse> GetDiscoveryDocument();
}
