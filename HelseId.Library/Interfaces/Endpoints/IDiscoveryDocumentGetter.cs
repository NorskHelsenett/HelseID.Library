namespace HelseId.Library.Interfaces.Endpoints;

public interface IDiscoveryDocumentGetter
{
    Task<DiscoveryDocument> GetDiscoveryDocument();
}
