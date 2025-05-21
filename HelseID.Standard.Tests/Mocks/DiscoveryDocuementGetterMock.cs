using HelseID.Standard.Interfaces.Endpoints;
using HelseID.Standard.Models;

namespace HelseID.Standard.Tests.Mocks;

public class DiscoveryDocumentGetterMock : IDiscoveryDocumentGetter
{
    public async Task<DiscoveryDocument> GetDiscoveryDocument()
    {
        var discoveryDocumentGetter = new DiscoveryDocumentGetterWithMockHttpClient();
        return await discoveryDocumentGetter.GetDiscoveryDocument();
    }
}
