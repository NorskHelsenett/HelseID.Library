using HelseId.Library.Tests.Mocks;

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
    public async Task GetTokenResponseAsync_returns_AccessTokenResponse_for_normal_response()
    {
        var tokenResponse = await _machineToMachineFlow.GetTokenResponseAsync();

        tokenResponse.Should().BeOfType<AccessTokenResponse>();

        var accessTokenResponse = (AccessTokenResponse)tokenResponse;
        
        accessTokenResponse.AccessToken.Should().Be("access token from endpoint");
        accessTokenResponse.ExpiresIn.Should().Be(60);
    }

    [Test]
    public async Task GetTokenResponseAsync_builds_token_request_with_specified_organization_numbers()
    {
        await _machineToMachineFlow.GetTokenResponseAsync(new OrganizationNumbers("parent", "child"));

        var payloadParameters = _clientCredentialsTokenRequestBuilder.TokenRequestParameters!.PayloadClaimParameters;
        payloadParameters.ParentOrganizationNumber.Should().Be("parent");
        payloadParameters.ChildOrganizationNumber.Should().Be("child");
    }
    
    [Test]
    public async Task GetTokenResponseAsync_caches_response_from_token_endpoint()
    {
        await _machineToMachineFlow.GetTokenResponseAsync();

        _cacheMock.CachedData.Should().NotBeNullOrEmpty();
        _cacheMock.LastKeySet.Should().Be(HelseIdConstants.TokenResponseCacheKey + "__"); 
    }
    
    [Test]
    public async Task GetTokenResponseAsync_caches_response_from_token_endpoint_keyed_on_organization_numbers()
    {
        await _machineToMachineFlow.GetTokenResponseAsync(new OrganizationNumbers
        {
            ParentOrganization = "123",
            ChildOrganization = "456"
        });

        _cacheMock.CachedData.Should().NotBeNullOrEmpty();
        _cacheMock.LastKeySet.Should().Be(HelseIdConstants.TokenResponseCacheKey + "_123_456"); 
    }
    
    [Test]
    public async Task GetTokenResponseAsync_calls_token_endpoint_when_cached_token_expires()
    {
        _cacheMock.SetCachedDataFromObject(new AccessTokenResponse
        {
            AccessToken = "cached access token",
            ExpiresIn = 123
        });
        
        var firstTokenResponse = await _machineToMachineFlow.GetTokenResponseAsync() as AccessTokenResponse;
        firstTokenResponse!.AccessToken.Should().Be("cached access token");

        _cacheMock.ResetCachedData();
        
        var secondTokenResponse = await _machineToMachineFlow.GetTokenResponseAsync() as AccessTokenResponse;
        secondTokenResponse!.AccessToken.Should().Be("access token from endpoint");
    }
    
    [Test]
    public async Task GetTokenResponseAsync_returns_cached_TokenResponse_if_available()
    {
        _cacheMock.SetCachedDataFromObject(new AccessTokenResponse()
        {
            AccessToken = "cached access token",
            ExpiresIn = 123
        });
        
        var tokenResponse = await _machineToMachineFlow.GetTokenResponseAsync() as AccessTokenResponse;

        tokenResponse.Should().NotBeNull();
        tokenResponse.AccessToken.Should().Be("cached access token");
        tokenResponse.ExpiresIn.Should().Be(123);
    }

    [Test]
    public async Task GetTokenResponseAsync_returns_error_response_when_request_fails()
    {
        SetupErrorTokenResponse();

        var tokenResponse = await _machineToMachineFlow.GetTokenResponseAsync();

        tokenResponse.Should().BeOfType<TokenErrorResponse>();

        var tokenErrorResponse = (TokenErrorResponse)tokenResponse;
        tokenErrorResponse.Error.Should().Be("error");
        tokenErrorResponse.ErrorDescription.Should().Be("error description");
    }
    
    [Test]
    public async Task GetTokenResponseAsync_does_not_cache_error_response()
    {
        SetupErrorTokenResponse();

        await _machineToMachineFlow.GetTokenResponseAsync();

        _cacheMock.CachedData.Should().BeEmpty();
        _cacheMock.LastKeySet.Should().BeNullOrEmpty();
    }
    
    [Test]
    public async Task GetTokenResponseAsync_returns_error_response_when_token_response_is_invalid()
    {
        SetupInvalidTokenResponse();

        var tokenResponse = await _machineToMachineFlow.GetTokenResponseAsync();

        tokenResponse.Should().BeOfType<TokenErrorResponse>();
    }

    [Test]
    public async Task GetTokenResponseAsync_returns_error_response_when_token_response_is_invalid_with_invalid_formatting()
    {
        SetupInvalidTokenResponse(HttpStatusCode.InternalServerError, "not json");

        var tokenResponse = await _machineToMachineFlow.GetTokenResponseAsync();

        tokenResponse.Should().BeOfType<TokenErrorResponse>();

        var tokenErrorResponse = (TokenErrorResponse)tokenResponse;
        
        tokenErrorResponse.Error.Should().Be("Invalid response");
        tokenErrorResponse.RawResponse.Should().Be("not json");
    }
    
    [Test]
    public async Task GetTokenResponseAsync_returns_error_response_when_token_response_is_json_but_missing_expected_elements()
    {
        SetupInvalidTokenResponse(HttpStatusCode.InternalServerError, """
                                                                      { "Error2" : "invalid_request" }
                                                                      """);

        var tokenResponse = await _machineToMachineFlow.GetTokenResponseAsync();

        tokenResponse.Should().BeOfType<TokenErrorResponse>();

        var tokenErrorResponse = (TokenErrorResponse)tokenResponse;
        
        tokenErrorResponse.Error.Should().Be("Invalid response");
        tokenErrorResponse.ErrorDescription.Should().NotBeEmpty();
        tokenErrorResponse.RawResponse.Should().Be("{ \"Error2\" : \"invalid_request\" }");
    }  
    
    [Test]
    public async Task GetTokenResponseAsync_returns_error_response_when_token_response_is_json_but_missing_error_description()
    {
        SetupInvalidTokenResponse(HttpStatusCode.InternalServerError, """
                                                                      { "Error" : "invalid_request" }
                                                                      """);

        var tokenResponse = await _machineToMachineFlow.GetTokenResponseAsync();

        tokenResponse.Should().BeOfType<TokenErrorResponse>();

        var tokenErrorResponse = (TokenErrorResponse)tokenResponse;
        
        tokenErrorResponse.Error.Should().Be("invalid_request");
        tokenErrorResponse.ErrorDescription.Should().BeEmpty();
        tokenErrorResponse.RawResponse.Should().Be("{ \"Error\" : \"invalid_request\" }");
    }
    
    [Test]
    public async Task GetAccessTokenAsync_returns_AccessTokenResponse_for_normal_response()
    {
        var accessToken = await _machineToMachineFlow.GetAccessTokenAsync();

        accessToken.Should().Be("access token from endpoint");
    }
    
    [Test]
    public async Task GetAccessTokenAsync_throws_HelseIdException_when_request_fails()
    {
        SetupErrorTokenResponse();

        Func<Task<string>> getAccesstoken = () => _machineToMachineFlow.GetAccessTokenAsync();
        var assertion = await getAccesstoken.Should().ThrowAsync<HelseIdException>().WithMessage("error description");
        assertion.Which.Error.Should().Be("error");
    }

    [Test]
    public async Task GetAccessTokenAsync_builds_token_request_with_specified_organization_numbers()
    {
        await _machineToMachineFlow.GetAccessTokenAsync(new OrganizationNumbers("parent", "child"));

        var payloadParameters = _clientCredentialsTokenRequestBuilder.TokenRequestParameters!.PayloadClaimParameters;
        payloadParameters.ParentOrganizationNumber.Should().Be("parent");
        payloadParameters.ChildOrganizationNumber.Should().Be("child");
    }

    [Test]
    public async Task GetAccessTokenAsync_caches_response_from_token_endpoint()
    {
        await _machineToMachineFlow.GetAccessTokenAsync();

        _cacheMock.CachedData.Should().NotBeNullOrEmpty();
        _cacheMock.LastKeySet.Should().Be(HelseIdConstants.TokenResponseCacheKey + "__"); 
    }
    
    [Test]
    public async Task GetAccessTokenAsync_calls_token_endpoint_when_cached_token_expires()
    {
        _cacheMock.SetCachedDataFromObject(new AccessTokenResponse
        {
            AccessToken = "cached access token",
            ExpiresIn = 123
        });
        
        var firstAccessToken = await _machineToMachineFlow.GetAccessTokenAsync();
        firstAccessToken.Should().Be("cached access token");

        _cacheMock.ResetCachedData();
        
        var secondAccessToken = await _machineToMachineFlow.GetAccessTokenAsync();
        secondAccessToken.Should().Be("access token from endpoint");
    }

    [Test]
    public async Task GetAccesTokenAsync_returns_cached_TokenResponse_if_available()
    {
        _cacheMock.SetCachedDataFromObject(new AccessTokenResponse()
        {
            AccessToken = "cached access token",
            ExpiresIn = 123
        });
        
        var accessToken = await _machineToMachineFlow.GetAccessTokenAsync();

        accessToken.Should().Be("cached access token");
    }


    public void Dispose()
    {
        _httpMessageHandler.Dispose();
    }
}
