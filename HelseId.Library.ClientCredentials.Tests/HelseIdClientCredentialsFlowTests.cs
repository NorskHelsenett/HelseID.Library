using HelseId.Library.Mocks;
using HelseId.Library.Tests.Mocks;
using Microsoft.IdentityModel.Tokens;

namespace HelseId.Library.ClientCredentials.Tests;

[TestFixture]
public class HelseIdClientCredentialsFlowTests : IDisposable
{
    private HelseIdClientCredentialsFlow _clientCredentialsFlow = null!;
    private MockHttpMessageHandlerWithCount _httpMessageHandler = null!;
    private TokenCacheMock _cacheMock = null!;
    private ClientCredentialsTokenRequestBuilderMock _clientCredentialsTokenRequestBuilder = null!;
    private SigningCredentialsReferenceMock _signingCredentialsReferenceMock;

    private const string Jwk1 = """
                                {"alg":"RS256", "d":"XdL6YAB9o0qpXzHsxJwXT1cL7fLArydZAFu5Dqu0a0r_DoXgc8W_VAlOboeGwhoUa2XsJLOiXD_xKwWeKozy54PaHdXQxmXGmUElhEkBdJEt82O_bu4MgQffqPIYLTE3GmmYAoz4GKsFqYQQjvDfIU-vSx69l-f0xBYuU7Po_YYmIL83q5xkuBAva3t9Aj9WDAk82YpT7B5bloHffvLkm30cmkHMHhCXgEJTstkwHI9wMFUrWxJrlh8vpwVbQyiDDDNGyHDmfRMr63gjx3ivwwQspiHdUrNtf1-RNundl362L7qypTmAWPi21bd3D8oLoBHbEFG7CfNeBLnjffKAsh8fNCksv_Hn-jE_HtE-VHpywKkcAvZHa-cb8GvkQg54_x-qgFJ1Zi1YQ7id1e0jtpCKB_SRN-kzzgNL8shfwWm9xHs-P4og6xvpp7IrRtBrhcyKGCjaaUX0KBF9RtgApmT-mPq4ZPM6Zdov2ZrLnzUC2prNAsV-pAHcl2PXLqQM74tlzB1fLpqUQeWKamW5quV64O5tBh4R54-FqoYNacL076QHkP96b0YuY4k-Hbu_nZa1IQlfop8AmZPsCJc4R2CmKVtHdvboSd1vtovwrhTkJHNILC5h8HXIKsKlM9ZeZwIJ7STAKWJlSYgMM4hvPoPQ6MKT7NRLh-R4iQHbPDk","dp":"Ukd8lLTFkEBmgL_XGi6JbMBbQMqKnbzeadtvUuYtB91cEwTUnuGRwQ6sEVthP7pGZhVVih57VVk5VAZ1LD56cXA2zdAJQRrCfpJo_5dYr5FcIowx5ZCD2ICWcNLOv77lgTilUyxNsNvgM40niT1aSzVlDNuKOUtjcvJ8SVFjIBnrnL-IZ5NpQDwNTim1m0Po1o1s5EP5RtQ-0yTF8-G-Cgk5H0ClWWVtdD45Im5yUShvNbY-QEAXxPTZHGOsOep4Lm9bg17fbfDtcvtMVMShEjHTwJbUi2Tv2pOzgCGlvyJZ_SKIQmgMuzGcq18R6bHr_qtc3-XLVNYnzXsY7AgELQ","dq":"jBzrMG7LhJQrE1ROjEdnIbRqgnNisMJhOSo4Hi_hAbY9tGL3Posp8_2M_3AOAU9HALPrE2T1nBVjwRaIcOROuM_xzZT9FgWBnlixNcgyIRuQuF7doLfQ5Baz9T872PO6uSajGFKyGyc07ivfyfGsif7QIyVG-ALWfso1-GjzQT-iZc60jKyeeU8NGNPW5yss3xIDI35Md7P7PFIhH-L6gB_lsGD8jxrC8XcXzcHW1UqcSGkw7E7e7heohHXmB_fKa5ySZnczPgR3Sbg5CXG7AVpNxY0OSiJn5AVGMpMu5airOcJ6JVdQbfknW6Ct08lvCTJfxFzNPFNCeeH5i8l7RQ","e":"AQAB","kty":"RSA","n":"4qXP535J5q0tN1hgr9IQhs6HgtPrtiwzLaiPYAJx3WIQDj0Fzy4FQ2XzlKLg24c0Yk7X4kFm3ua0tZ0Va-Iod1XdVDm8SfcGumqKTui_Q8G-7zNz_oq-CIij9_B1wVHtMys4mUnVcKOkoFqHt_9gvXb034CoZt4X2s_hS0OgEVqtWUJDjRHMrBWVtY7ioe0Ed33Kd5qFCJu60rOuZ6FfAXgo1VCpslRCg2egVF3cKNXAoBYAfjIS6jqYgeMlsUNzaNs_FjWf2K-rvfxoPuyJHzHp3a0vIQOukw-522EjszGEctOAzSQd_gJzmH1Hw3gAjGJ3Z8kAKrYdjflSQw1xamml8DE5Y4cAGG9g-xNvWtiRWRperRGXsBn3qfbtWK9vN07pNOgWxqa23lTsZQqWzPPTWltyiux_7TRGGkqxrpGI3XEXlcV7O4xBmgEN3-T9kZHD40Uel9axE9OIv6CrgiuTAFxGLIIxd-fAZ7_jRQ8_Q1AdO78OwVd8LkziKI5H4c0ewWMVpxT4Afwfb7smyKwN4Cjb_TwAFpH767F0Z95i5nhpce_ZelGH3klM44OSSL_HmWjNB1ndQOJbwUjEmvsJy_PwGmtCsqZisk76mV8-5--ptftgfjvxmAIVQHiJtUkbkIQyq-EiZhoo1IZFZPqJ5gs3-GRUGE6SXlG5JaU","p":"8Z0cVaG87pB4Coiud64BAFGUJN0xB0Q0qIA1Y0iP3T-pMfQFRe09z4Hh8ozXrqUzUsqclv-sK9LtrzLri2jBzj-MuJ3qfoscQXv96ZFKEH7E7PCQ0xYER1oT4hleM92mBrk1v7zWmsde2t-2HHUKTpe8Tbs2Q-yZr7pvCGSIeItu0DQubDAvk2li4PMJHmoUuJw5AVFMT7cFM1xOAeCvOMVUc6S_QrjBLvTva8uoLUV-LYgnt3yva3x6tA0JoWv2SIjX3IO208AruYiPhTrpNLqgHGKFrjV9dZ0xhyjJtRNObggXXWWYqhzEP-k2FLykLGgaAbpvBPMLrECVeXApWw","q":"8CSTicEe_kVG0L5BUcjiMDNI0dTJCScEkBj3m3gdXHOhbpByaalsu6sdekT-eCRebMWNTFXqcwnk3K34hk4PYQbwpu-rPF2DGUg1Vbe4nTwQylgeT9Ag9xD3GG5W5rQr2BuBHHBGyn1bJ1uqnKTYZ3IHUu7BMmy9_FX-mSVC-isBeBnHHaqRNk5R3VOeHR-WPEYES3mDKq0matwjaosi3935p7LaqAmnhYYG9-kAD6YjV7Mmvy2xwZ7qG8KH1r6pB3ON4q_fk_gOIKQfxZXZN4lOh8PTOBFd5Gy9feMPoa3fbOkrcuGMzXbKPr61JAASjJNmz94N2kVTEJD0Uo4c_w","qi":"sNycboiTb6exCAaJk-_CVTXNqmjhvDwxProOEdJuD1DPJKqOlM-VU4m9M5zsiUv7EHuvw_A6fX6oxvLrHnsoPmaDfN05GrjxyoCRNtBva4L2OuO8KHtqgqTSJSjmLEM7P7jqriY_pss5ZgylOg7DEeZZGWBuQGqdnTb3RtOeehM0aUTJO6xXE34809tVBLyqkrOd5CBOO8ve2kcgxKrRErcz5-l-TJiE0jFPZej9UmMgz4JGpLBGVS0-jrGJqUdP4HWjEzU14CXxhg5sFZNx6NFCS-4o-DtUkTEHjDKb8u8zBOR6pvsYJ5HF837zomWf39JMJGiFL1XCZWXniVbW9Q","kid":"C71331FC403783934D2E335FD1F75B8B"}
                                """;
    
    private const string Jwk2 = """
                                {"p": "3BomenA-ddDgoPBTo2yRYCBIA35urT448RJelI9gCHSfPq49cnD15V6tX4ItFGAf0Kdm5exUaftfadOzfyXRBSqbgthNTj4k086YtMgdF9TFSE10_EkBct6QodF0qbdxrGZOaInXNou1v1m7o9P-X-yP8F39hJ9CmNSbioymO2E","kty": "RSA","q": "o1I3Pb9KQtrJsxSVgmsvZIDIgKnZCSvvsF2dkdUVp4HUNAFPvEffQzBAxqwjwvdNN_TwdzKTEvxedw-JtppEOaP99R1uy6qsHflg_iQf8sP6gnzTID-rdaceYuMQS-pwsf_oPhVMe0ly3R2nIpXgvAYmOOUjD6Ec4UJs-zHsbm0","d": "Izy0WfYRG8kaHjvyoiSYfpEJltPyd-OmQp6y55MLfF0-5VggrL7GoVWGtZGh9aZzM6qGMfsihxNgfaCbdWu82a1H4Ujg5VJG0QDKiMMy4Bh1yKcIxn8Xhi8qquwS5RYojgpwI1TIjco0P4_TYKyVa7OY-fu35c08TKorkKQYSl7Z1xBXHbdxja55hjT0xW5VMFQWD5m93MWSIquK7v77J27MVm4bNXQ-aM5l0lDP7WZ88SzDzOPVdSxdlQXTzZntWJzV9hEFt3-8PvCjQCvlm8iBYA2ulX_Wg6grzOde-e57Ye-eEACnrwO_MKewcJYi77jp63Bp9N4R-VmZ1OmHAQ","e": "AQAB","use": "sig","kid": "Xw8a/2gbvr+Wb850yqrz+tSDZ6M=","qi": "y4IfGPwYWFGAfCMt1VSlTJQ5e7ETOkjgv7n9ZK0p5NOagvT0RVzLJwZAcbQpTqdxb5YFqXxFjgx6uVGTmHBDHbKcLtJjlOfMan0dIkESI636OQrUZvzoNbmkMPcoE5fw5LHCyiCcbNK08Ua45Lc3m_KYP2tzzJhnWe9UP4WHKTA","dp": "x_K5oeD13PG0HoA0evHV3w10XjgtMxSjhdy6LxtQkl3fjIGdMKmNuKEZvvnzGd95B4QL5jObO-wv8WNwXMFt8NOUEnmNQNcTfZbhAtoIE-4milhTPhzURBYMLfSplsQfcA5AjJcr-1FP-lT6MQoUu--bkzDPbft_9HawFl1PZgE","alg": "RS256","dq": "k7vBAHfSotrp7LXH8CsouY7Mz6XUDznRHQXxjILljZog69Hr4HJbAJnbXltEFg5BqUZFZm03IMtsX-KMPcMZx9IR3PF80XpXKt3z4K-ovp6MMhDboaY2YLyE_gpLKYCt5DWKTO5TOopb_qveDajpmustE_YhWvfv-ctubWLhbZk","n": "jGtWXv2mjg843kD0i3XQar633Ve4NOU6iF-5XYUzNAiEybitK7tv7AP8T_Pp0p2aupIp2XqjOra9PLGYTJKbD3GQNo4u-0lBlZWFTRRC7Fhh5ZzVxwapoMcCndITwYDB1QWbSKVPZWDSW0feIs4gsXgmUTQdCfLlGakFLjnbrQ5rvxZOFosrYQq7c2__HtBYBOzgPdIOPyANYGH6KXZ_xslueIw-MBdvzhwsG22sYeSOMZWct-nMHxcNhY2bK4wPfpez9zaaXnTKCS4Q9GG-KYQBV6te7RVTbBqKX1QsagZQ1QdMfO-y8sGpBNVD4W2YAfbJOZMq06DuDGEknrb2TQ"}
                                """;

    [SetUp]
    public void SetUp()
    {
        _clientCredentialsTokenRequestBuilder = new ClientCredentialsTokenRequestBuilderMock();
        var payloadClaimsCreatorMock = new PayloadClaimsCreatorMock();

        _httpMessageHandler = new MockHttpMessageHandlerWithCount();
        SetupNormalTokenEndpointResponse();

        var httpClientFactoryMock = new HttpClientFactoryMock(_httpMessageHandler);
        
        _cacheMock = new TokenCacheMock();

        _signingCredentialsReferenceMock = new SigningCredentialsReferenceMock
        {
            Jwk = Jwk1
        };

        _clientCredentialsFlow = new HelseIdClientCredentialsFlow(_clientCredentialsTokenRequestBuilder,
            payloadClaimsCreatorMock,
            httpClientFactoryMock,
            _cacheMock, 
            _signingCredentialsReferenceMock);        
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
        var tokenResponse = await _clientCredentialsFlow.GetTokenResponseAsync();

        tokenResponse.Should().BeOfType<AccessTokenResponse>();

        var accessTokenResponse = (AccessTokenResponse)tokenResponse;
        
        accessTokenResponse.AccessToken.Should().Be("access token from endpoint");
        accessTokenResponse.ExpiresIn.Should().Be(60);
    }

    [Test]
    public async Task GetTokenResponseAsync_builds_token_request_with_specified_organization_numbers()
    {
        await _clientCredentialsFlow.GetTokenResponseAsync(new OrganizationNumbers("parent", "child"));

        var payloadParameters = _clientCredentialsTokenRequestBuilder.TokenRequestParameters!.PayloadClaimParameters;
        payloadParameters.ParentOrganizationNumber.Should().Be("parent");
        payloadParameters.ChildOrganizationNumber.Should().Be("child");
    }
    
    [Test]
    public async Task GetTokenResponseAsync_caches_response_from_token_endpoint()
    {
        await _clientCredentialsFlow.GetTokenResponseAsync();

        var jsonWebKey1 = new JsonWebKey(Jwk1);

        _cacheMock.CachedData.Should().NotBeNullOrEmpty();
        _cacheMock.LastKeySet.Should().Be(HelseIdConstants.TokenResponseCacheKey + "_" + jsonWebKey1.Kid + "___"); 
    }
    
    [Test]
    public async Task GetTokenResponseAsync_caches_response_from_token_endpoint_keyed_on_organization_numbers()
    {
        await _clientCredentialsFlow.GetTokenResponseAsync(new OrganizationNumbers
        {
            ParentOrganization = "123",
            ChildOrganization = "456"
        });

        var jsonWebKey1 = new JsonWebKey(Jwk1);

        _cacheMock.CachedData.Should().NotBeNullOrEmpty();
        _cacheMock.LastKeySet.Should().Be(HelseIdConstants.TokenResponseCacheKey + "_" + jsonWebKey1.Kid + "__123_456"); 
    }
    
    [Test]
    public async Task GetTokenResponseAsync_caches_response_from_token_endpoint_keyed_on_keyid_from_signing_key()
    {
        _signingCredentialsReferenceMock.Jwk = Jwk2;
        
        await _clientCredentialsFlow.GetTokenResponseAsync();

        var jsonWebKey2 = new JsonWebKey(Jwk2);

        _cacheMock.CachedData.Should().NotBeNullOrEmpty();
        _cacheMock.LastKeySet.Should().Be(HelseIdConstants.TokenResponseCacheKey + "_" + jsonWebKey2.Kid + "___"); 
    }
    
    [Test]
    public async Task GetTokenResponseAsync_does_not_use_cached_token_when_signing_key_is_switched()
    {
        var jsonWebKey1 = new JsonWebKey(Jwk1);
        var jsonWebKey2 = new JsonWebKey(Jwk2);

        await _clientCredentialsFlow.GetTokenResponseAsync();

        _cacheMock.LastKeySet.Should().Be(HelseIdConstants.TokenResponseCacheKey + "_" + jsonWebKey1.Kid + "___"); 

        _signingCredentialsReferenceMock.Jwk = Jwk2;
        
        await _clientCredentialsFlow.GetTokenResponseAsync();

        // Verify that the cache key used to retrieve access tokens contains the second key
        // This will cause the token cache to return null, we thereby retrieve a new token 
        // that will be cached using the new kid as part of the cache key 
        _cacheMock.LastKeyGet.Should().Be(HelseIdConstants.TokenResponseCacheKey + "_" + jsonWebKey2.Kid + "___"); 
    }
    
    [Test]
    public async Task GetTokenResponseAsync_calls_token_endpoint_when_cached_token_expires()
    {
        _cacheMock.SetCachedDataFromObject(new AccessTokenResponse
        {
            AccessToken = "cached access token",
            ExpiresIn = 123
        });
        
        var firstTokenResponse = await _clientCredentialsFlow.GetTokenResponseAsync() as AccessTokenResponse;
        firstTokenResponse!.AccessToken.Should().Be("cached access token");

        _cacheMock.ResetCachedData();
        
        var secondTokenResponse = await _clientCredentialsFlow.GetTokenResponseAsync() as AccessTokenResponse;
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
        
        var tokenResponse = await _clientCredentialsFlow.GetTokenResponseAsync() as AccessTokenResponse;

        tokenResponse.Should().NotBeNull();
        tokenResponse.AccessToken.Should().Be("cached access token");
        tokenResponse.ExpiresIn.Should().Be(123);
    }

    [Test]
    public async Task GetTokenResponseAsync_returns_error_response_when_request_fails()
    {
        SetupErrorTokenResponse();

        var tokenResponse = await _clientCredentialsFlow.GetTokenResponseAsync();

        tokenResponse.Should().BeOfType<TokenErrorResponse>();

        var tokenErrorResponse = (TokenErrorResponse)tokenResponse;
        tokenErrorResponse.Error.Should().Be("error");
        tokenErrorResponse.ErrorDescription.Should().Be("error description");
    }
    
    [Test]
    public async Task GetTokenResponseAsync_does_not_cache_error_response()
    {
        SetupErrorTokenResponse();

        await _clientCredentialsFlow.GetTokenResponseAsync();

        _cacheMock.CachedData.Should().BeEmpty();
        _cacheMock.LastKeySet.Should().BeNullOrEmpty();
    }
    
    [Test]
    public async Task GetTokenResponseAsync_returns_error_response_when_token_response_is_invalid()
    {
        SetupInvalidTokenResponse();

        var tokenResponse = await _clientCredentialsFlow.GetTokenResponseAsync();

        tokenResponse.Should().BeOfType<TokenErrorResponse>();
    }

    [Test]
    public async Task GetTokenResponseAsync_returns_error_response_when_token_response_is_invalid_with_invalid_formatting()
    {
        SetupInvalidTokenResponse(HttpStatusCode.InternalServerError, "not json");

        var tokenResponse = await _clientCredentialsFlow.GetTokenResponseAsync();

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

        var tokenResponse = await _clientCredentialsFlow.GetTokenResponseAsync();

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

        var tokenResponse = await _clientCredentialsFlow.GetTokenResponseAsync();

        tokenResponse.Should().BeOfType<TokenErrorResponse>();

        var tokenErrorResponse = (TokenErrorResponse)tokenResponse;
        
        tokenErrorResponse.Error.Should().Be("invalid_request");
        tokenErrorResponse.ErrorDescription.Should().BeEmpty();
        tokenErrorResponse.RawResponse.Should().Be("{ \"Error\" : \"invalid_request\" }");
    }
    
    [Test]
    public async Task GetAccessTokenAsync_returns_AccessTokenResponse_for_normal_response()
    {
        var tokenResponse = await _clientCredentialsFlow.GetTokenResponseAsync();
        tokenResponse.Should().BeOfType<AccessTokenResponse>();
        var accessTokenResponse = (AccessTokenResponse)tokenResponse;
        accessTokenResponse.AccessToken.Should().Be("access token from endpoint");
    }
    
    [Test]
    public async Task GetAccessTokenAsync_throws_HelseIdException_when_request_fails()
    {
        SetupErrorTokenResponse();
        
        var tokenResponse = await _clientCredentialsFlow.GetTokenResponseAsync();
        tokenResponse.Should().BeOfType<TokenErrorResponse>();
        var tokenErrorResponse = (TokenErrorResponse)tokenResponse;
        tokenErrorResponse.Error.Should().Be("error");
        tokenErrorResponse.ErrorDescription.Should().Be("error description");
    }

    [Test]
    public async Task GetAccessTokenAsync_builds_token_request_with_specified_organization_numbers()
    {
        await _clientCredentialsFlow.GetTokenResponseAsync(new OrganizationNumbers("parent", "child"));

        var payloadParameters = _clientCredentialsTokenRequestBuilder.TokenRequestParameters!.PayloadClaimParameters;
        payloadParameters.ParentOrganizationNumber.Should().Be("parent");
        payloadParameters.ChildOrganizationNumber.Should().Be("child");
    }

    [Test]
    public async Task GetAccessTokenAsync_caches_response_from_token_endpoint()
    {
        await _clientCredentialsFlow.GetTokenResponseAsync();

        var jsonWebKey1 = new JsonWebKey(Jwk1);
        
        _cacheMock.CachedData.Should().NotBeNullOrEmpty();
        _cacheMock.LastKeySet.Should().Be(HelseIdConstants.TokenResponseCacheKey + "_" + jsonWebKey1.Kid + "___"); 
    }
    
    [Test]
    public async Task GetAccessTokenAsync_calls_token_endpoint_when_cached_token_expires()
    {
        _cacheMock.SetCachedDataFromObject(new AccessTokenResponse
        {
            AccessToken = "cached access token",
            ExpiresIn = 123
        });
        
        await _clientCredentialsFlow.GetTokenResponseAsync();
        _cacheMock.ResetCachedData();
        
        var secondTokenResponse = await _clientCredentialsFlow.GetTokenResponseAsync();
        secondTokenResponse.Should().BeOfType<AccessTokenResponse>();
        var secondAccessTokenResponse = (AccessTokenResponse)secondTokenResponse;
        secondAccessTokenResponse.AccessToken.Should().Be("access token from endpoint");
    }

    [Test]
    public async Task GetAccesTokenAsync_returns_cached_TokenResponse_if_available()
    {
        _cacheMock.SetCachedDataFromObject(new AccessTokenResponse()
        {
            AccessToken = "cached access token",
            ExpiresIn = 123
        });
        
        var tokenResponse = await _clientCredentialsFlow.GetTokenResponseAsync();
        tokenResponse.Should().BeOfType<AccessTokenResponse>();
        var accsessTokenResponse = (AccessTokenResponse)tokenResponse;

        accsessTokenResponse.AccessToken.Should().Be("cached access token");
    }


    public void Dispose()
    {
        _httpMessageHandler.Dispose();
    }
}
