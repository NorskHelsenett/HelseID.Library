using HelseId.Standard.Interfaces.Endpoints;
using HelseId.Standard.Models;

namespace HelseId.Standard.Tests.Mocks;

public class DiscoveryDocumentGetterMock : IDiscoveryDocumentGetter
{
    public Task<DiscoveryDocument> GetDiscoveryDocument()
    {
        return Task.FromResult(new DiscoveryDocument
        {
            AuthorizeEndpoint = "https://helseid-sts.nhn.no/connect/authorize",
            TokenEndpoint = "https://helseid-sts.nhn.no/connect/token",
        });
    }
}
