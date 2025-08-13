namespace HelseId.Library.Services.Endpoints;

public class HelseIdEndpointsDiscoverer : IHelseIdEndpointsDiscoverer
{
    private readonly IDiscoveryDocumentGetter _discoveryDocumentGetter;

    public HelseIdEndpointsDiscoverer(IDiscoveryDocumentGetter discoveryDocumentGetter)
    {
        _discoveryDocumentGetter = discoveryDocumentGetter;
    }
    
    public async Task<string> GetTokenEndpointFromHelseId()
    {
        var disco = await _discoveryDocumentGetter.GetDiscoveryDocument();
        return disco.TokenEndpoint!;
    }
}
