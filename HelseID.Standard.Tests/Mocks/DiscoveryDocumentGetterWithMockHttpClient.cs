using HelseID.Standard.Interfaces.Endpoints;
using HelseID.Standard.Models;

namespace HelseID.Standard.Tests.Mocks;

public class DiscoveryDocumentGetterWithMockHttpClient : IDiscoveryDocumentGetter
{
    public Task<DiscoveryDocument> GetDiscoveryDocument()
    {
        return Task.FromResult(new DiscoveryDocument
        {
            AuthorizeEndpoint = "https://helseid-sts.nhn.no/connect/authorize",
        });
    }
}
