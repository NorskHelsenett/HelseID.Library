namespace HelseId.Library.Interfaces.Endpoints;

public interface IHelseIdEndpointsDiscoverer
{
    Task<string> GetTokenEndpointFromHelseId();
}
