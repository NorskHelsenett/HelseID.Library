namespace HelseId.Standard.Interfaces.Endpoints;

public interface IHelseIdEndpointsDiscoverer
{
    Task<string> GetTokenEndpointFromHelseId();
}
