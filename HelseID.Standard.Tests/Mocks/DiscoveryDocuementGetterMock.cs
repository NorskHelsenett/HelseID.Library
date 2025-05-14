using HelseID.Standard.Interfaces.Endpoints;
using HelseID.Standard.Models;

namespace HelseID.Standard.Tests.Mocks;

public class DiscoveryDocumentGetterMock : IDiscoveryDocumentGetter
{
    public async Task<DiscoveryDocument> GetDiscoveryDocument()
    {
        var discoveryDocumentGetter = new DiscoveryDocumentGetterWithMockHttpClient("https://helseid-sts.nhn.no", new MemoryCacheMock());
        return await discoveryDocumentGetter.GetDiscoveryDocument();
    }
}
