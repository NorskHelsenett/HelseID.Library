using HelseId.Library.ClientCredentials.Services;
using HelseId.Library.Mocks;

namespace HelseId.Library.ClientCredentials.Tests.Services;

[TestFixture]
public class HelseIdDPoPDelegatingHandlerTests
{
    [Test]
    public async Task SendAsync_requests_AccessToken_and_DPoP_proof()
    {
        var clientCredentialsFlowMock = new HelseIdClientCredentialsFlowMock("access token", "scope");
        var dpopProofCreatorMock = new DPoPProofCreatorMock("DPoP proof");
     
        var handler = new HelseIdDPoPDelegatingHandler(clientCredentialsFlowMock,
            dpopProofCreatorMock,
            "scope");

        var mockHttpMessageHandler = new MockHttpMessageHandler();
        mockHttpMessageHandler
            .Expect(HttpMethod.Get, "https://helseid.no")
            .WithHeaders("Authorization", "DPoP access token")
            .WithHeaders("DPoP", "DPoP proof")
            .Respond(JsonContent.Create(new
            {
                success = true
            }));
        handler.InnerHandler = mockHttpMessageHandler;

        var httpClient = new HttpClient(handler); 
        await httpClient.GetAsync("https://helseid.no");

        clientCredentialsFlowMock.CallCount.Should().Be(1);
        clientCredentialsFlowMock.Scope.Should().Be("scope");
        dpopProofCreatorMock.AccessToken.Should().Be("access token");
        dpopProofCreatorMock.Url.Should().StartWith("https://helseid.no");
        dpopProofCreatorMock.HttpMethod.Should().Be(HttpMethod.Get);
    }
}
