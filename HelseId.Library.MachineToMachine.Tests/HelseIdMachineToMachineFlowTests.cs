namespace HelseId.Library.MachineToMachine.Tests;

[TestFixture]
public class HelseIdMachineToMachineFlowTests : IDisposable
{
    private HelseIdMachineToMachineFlow _machineToMachineFlow = null!;
    private MockHttpMessageHandlerWithCount _httpMessageHandler = null!;
    private TokenCacheMock _cacheMock = null!;
    private ClientCredentialsTokenRequestBuilderMock _clientCredentialsTokenRequestBuilder = null!;

    [SetUp]
    public void SetUp()
    {
        _clientCredentialsTokenRequestBuilder = new ClientCredentialsTokenRequestBuilderMock();
        var payloadClaimsCreatorMock = new PayloadClaimsCreatorMock();

        _httpMessageHandler = new MockHttpMessageHandlerWithCount();
        SetupNormalTokenEndpointResponse();

        var httpClientFactoryMock = new HttpClientFactoryMock(_httpMessageHandler);
        
        _cacheMock = new TokenCacheMock();

        _machineToMachineFlow = new HelseIdMachineToMachineFlow(_clientCredentialsTokenRequestBuilder,
            payloadClaimsCreatorMock,
            httpClientFactoryMock,
            _cacheMock);        
    }

    private void SetupNormalTokenEndpointResponse()
    {
        _httpMessageHandler.ResetExpectations();
        
        var requestCounter = 0;
        Func<HttpRequestMessage, bool> isFirstRequest = _ =>
        {
            requestCounter++;
            return requestCounter == 1;
        };
        
        Func<HttpRequestMessage, bool> isSecondRequest = _ =>     {
            requestCounter++;
            return requestCounter == 2;
        };
        
        _httpMessageHandler
            .Expect("https://helseid-sts.nhn.no/connect/token")
            .With(isFirstRequest)
            .Respond(HttpStatusCode.BadRequest,
                new Dictionary<string, string> { { "DPoP-Nonce", "dpop-nonce" } },
                JsonContent.Create(new
                {
                    error = "error here",
                    error_description = "error description here"
                }));

        _httpMessageHandler
            .Expect("https://helseid-sts.nhn.no/connect/token")
            .With(isSecondRequest)
            .Respond(JsonContent.Create(new
            {
                access_token = "access token from endpoint",
                expires_in = 60
            }));
    }

    private void SetupErrorTokenResponse()
    {
        _httpMessageHandler.ResetExpectations();

        _httpMessageHandler.Expect("https://helseid-sts.nhn.no/connect/token")
            .Respond(HttpStatusCode.BadRequest,
            JsonContent.Create(new
            {
                error = "error",
                error_description = "error description",
            }));
    }
    
    private void SetupInvalidTokenResponse(HttpStatusCode statusCode = HttpStatusCode.OK, string content = "")
    {
        _httpMessageHandler.ResetExpectations();

        _httpMessageHandler
            .Expect("https://helseid-sts.nhn.no/connect/token")
            .Respond(statusCode, new StringContent(content, Encoding.UTF8, "application/json"));
    }

    [Test]
    public async Task GetTokenAsync_returns_AccessTokenResponse_for_normal_response()
    {
        var tokenResponse = await _machineToMachineFlow.GetTokenAsync();

        tokenResponse.Should().BeOfType<AccessTokenResponse>();

        var accessTokenResponse = (AccessTokenResponse)tokenResponse;
        
        accessTokenResponse.AccessToken.Should().Be("access token from endpoint");
        accessTokenResponse.ExpiresIn.Should().Be(60);
    }

    [Test]
    public async Task GetTokenAsync_builds_token_request_with_specified_organization_numbers()
    {
        await _machineToMachineFlow.GetTokenAsync(new OrganizationNumbers("parent", "child"));

        var payloadParameters = _clientCredentialsTokenRequestBuilder.TokenRequestParameters!.PayloadClaimParameters;
        payloadParameters.ParentOrganizationNumber.Should().Be("parent");
        payloadParameters.ChildOrganizationNumber.Should().Be("child");
    }
    
    [Test]
    public async Task GetTokenAsync_caches_response_from_token_endpoint()
    {
        await _machineToMachineFlow.GetTokenAsync();

        _cacheMock.CachedData.Should().NotBeNullOrEmpty();
        _cacheMock.LastKeySet.Should().Be(HelseIdConstants.TokenResponseCacheKey + "__"); 
    }
    
    [Test]
    public async Task GetTokenAsync_caches_response_from_token_endpoint_keyed_on_organization_numbers()
    {
        await _machineToMachineFlow.GetTokenAsync(new OrganizationNumbers
        {
            ParentOrganization = "123",
            ChildOrganization = "456"
        });

        _cacheMock.CachedData.Should().NotBeNullOrEmpty();
        _cacheMock.LastKeySet.Should().Be(HelseIdConstants.TokenResponseCacheKey + "_123_456"); 
    }
    
    [Test]
    public async Task GetTokenAsync_calls_token_endpoint_when_cached_token_expires()
    {
        _cacheMock.SetCachedDataFromObject(new AccessTokenResponse
        {
            AccessToken = "cached access token",
            ExpiresIn = 123
        });
        
        var firstTokenResponse = await _machineToMachineFlow.GetTokenAsync() as AccessTokenResponse;
        firstTokenResponse!.AccessToken.Should().Be("cached access token");

        _cacheMock.ResetCachedData();
        
        var secondTokenResponse = await _machineToMachineFlow.GetTokenAsync() as AccessTokenResponse;
        secondTokenResponse!.AccessToken.Should().Be("access token from endpoint");
    }
    
    [Test]
    public async Task GetTokenAsync_returns_cached_TokenResponse_if_available()
    {
        _cacheMock.SetCachedDataFromObject(new AccessTokenResponse()
        {
            AccessToken = "cached access token",
            ExpiresIn = 123
        });
        
        var tokenResponse = await _machineToMachineFlow.GetTokenAsync() as AccessTokenResponse;

        tokenResponse.Should().NotBeNull();
        tokenResponse.AccessToken.Should().Be("cached access token");
        tokenResponse.ExpiresIn.Should().Be(123);
    }

    [Test]
    public async Task GetTokenAsync_returns_error_response_when_request_fails()
    {
        SetupErrorTokenResponse();

        var tokenResponse = await _machineToMachineFlow.GetTokenAsync();

        tokenResponse.Should().BeOfType<TokenErrorResponse>();

        var tokenErrorResponse = (TokenErrorResponse)tokenResponse;
        tokenErrorResponse.Error.Should().Be("error");
        tokenErrorResponse.ErrorDescription.Should().Be("error description");
    }
    
    [Test]
    public async Task GetTokenAsync_does_not_cache_error_response()
    {
        SetupErrorTokenResponse();

        await _machineToMachineFlow.GetTokenAsync();

        _cacheMock.CachedData.Should().BeEmpty();
        _cacheMock.LastKeySet.Should().BeNullOrEmpty();
    }
    
    [Test]
    public async Task GetTokenAsync_returns_error_response_when_token_response_is_invalid()
    {
        SetupInvalidTokenResponse();

        var tokenResponse = await _machineToMachineFlow.GetTokenAsync();

        tokenResponse.Should().BeOfType<TokenErrorResponse>();
    }

    [Test]
    public async Task GetTokenAsync_returns_error_response_when_token_response_is_invalid_with_invalid_formatting()
    {
        SetupInvalidTokenResponse(HttpStatusCode.InternalServerError, "not json");

        var tokenResponse = await _machineToMachineFlow.GetTokenAsync();

        tokenResponse.Should().BeOfType<TokenErrorResponse>();
        ((TokenErrorResponse)tokenResponse).Error.Should().Be("Invalid response");
    }

    public void Dispose()
    {
        _httpMessageHandler.Dispose();
    }
}
