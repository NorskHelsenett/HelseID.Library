namespace HelseId.Library.Interfaces.Endpoints;

internal interface IHelseIdEndpointsDiscoverer
{
    Task<string> GetTokenEndpointFromHelseId();
}
