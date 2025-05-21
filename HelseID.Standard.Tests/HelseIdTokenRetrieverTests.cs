using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using HelseID.Standard.Services.TokenRequests;
using HelseID.Standard.Tests.Mocks;
using RichardSzalay.MockHttp;

namespace HelseID.Standard.Tests;

[TestFixture]
public class HelseIdTokenRetrieverTests
{
    [Test]
    public async Task GetTokenAsync_returns_TokenResponse()
    {
        var clientCredentialsTokenRequestBuilder = new ClientCredentialsTokenRequestBuilderMock();
        var payloadClaimsCreatorMock = new PayloadClaimsCreatorMock();

        var httpMessageHandler = new MockHttpMessageHandlerWithCount();
        httpMessageHandler
            .When("https://helseid-sts.nhn.no/connect/token")
            .Respond(HttpStatusCode.BadRequest,
                new Dictionary<string, string> { { "DPoP-Nonce", "dpop-nonce" } },
                JsonContent.Create(new
                {
                    error = "error here",
                    error_description = "error description here"
                }));
        
        httpMessageHandler
            .When("https://helseid-sts.nhn.no/connect/token")
            .Respond(JsonContent.Create(new
            {
                access_token = "access token",
                expires_in = 60
            }));
        
        var httpClientFactoryMock = new HttpClientFactoryMock(httpMessageHandler);
        
        var cacheMock = new DistributedMemoryCacheMock();

        var tokenRetriever = new HelseIdTokenRetriever(clientCredentialsTokenRequestBuilder,
            payloadClaimsCreatorMock,
            httpClientFactoryMock,
            cacheMock);

        var tokenResponse = await tokenRetriever.GetTokenAsync();

        tokenResponse.Should().NotBeNull();
        tokenResponse.AccessToken.Should().Be("access token");
        tokenResponse.ExpiresIn.Should().Be(60);
    }
}
