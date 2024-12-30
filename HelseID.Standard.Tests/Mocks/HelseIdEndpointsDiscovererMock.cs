using HelseID.Standard.Interfaces.Endpoints;

namespace HelseID.Standard.Tests.Mocks;

public class HelseIdEndpointsDiscovererMock : IHelseIdEndpointsDiscoverer
{
    public const string TokenEndpoint = "https://helseid-sts.nhn.no/connect/token";
    
    public bool GetTokenEndpointFromHelseIdCalled { get; set; }
    
    public Task<string> GetTokenEndpointFromHelseId()
    {
        GetTokenEndpointFromHelseIdCalled = true;
        return Task.FromResult(TokenEndpoint);
    }
}
