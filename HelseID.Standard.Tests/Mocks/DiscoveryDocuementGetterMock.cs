using HelseID.Standard.Interfaces.Endpoints;
using IdentityModel.Client;

namespace HelseID.Standard.Tests.Mocks;

public class DiscoveryDocumentGetterMock : IDiscoveryDocumentGetter
{
    public async Task<DiscoveryDocumentResponse> GetDiscoveryDocument()
    {
        var discoveryDocumentGetter = new DiscoveryDocumentGetterWithMockHttpClient("https://helseid-sts.nhn.no", new MemoryCacheMock());
        return await discoveryDocumentGetter.GetDiscoveryDocument();
    }
}
