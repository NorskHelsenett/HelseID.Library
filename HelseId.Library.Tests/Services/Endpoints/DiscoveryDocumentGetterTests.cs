using HelseId.Library.Tests.Mocks;

namespace HelseId.Library.Tests.Services.Endpoints;

[TestFixture]
public class DiscoveryDocumentGetterTests 
{
    private const string StsUrl = "https://helseid-sts.nhn.no";
    private DiscoveryDocumentCacheMock _discoveryDocumentCacheMock = null!;
    private DiscoveryDocumentGetter _discoveryDocumentGetter = null!;
    private HttpClientFactoryMock _httpClientFactoryMock = null!;
    

    private const string PrivateKey = 
        """
        {
            "kty": "EC",
            "d": "PTQlsiXQ-PU_aeG1cSXZmEtm_rJH7Q5lEtqn9hP-SOlNHurT3vpM6gMy28h59G8u",
            "use": "sig",
            "crv": "P-384",
            "x": "_fwQ_E2rqeBOQ0YYzQBCvZNK60-n4PUG7cbJelBkuAbfEqmnaMHNUsReIsnE3432",
            "y": "xbuUzn7GpWq7JuKgrY_QxskViWPyDk_MoIef5JXXPlWkdB24cQLVgm-Jgz8NOblZ",
            "alg": "ES384"
        }
        """;

    private const string MetadataEndpoint = "https://helseid-sts.nhn.no/.well-known/openid-configuration";
    private const string MetadataEndpointContent =
        "{\"issuer\":\"https://helseid-sts.nhn.no\",\"jwks_uri\":\"https://helseid-sts.nhn.no/.well-known/openid-configuration/jwks\",\"authorization_endpoint\":\"https://helseid-sts.nhn.no/connect/authorize\",\"token_endpoint\":\"https://helseid-sts.nhn.no/connect/token\",\"userinfo_endpoint\":\"https://helseid-sts.nhn.no/connect/userinfo\",\"end_session_endpoint\":\"https://helseid-sts.nhn.no/connect/endsession\",\"check_session_iframe\":\"https://helseid-sts.nhn.no/connect/checksession\",\"revocation_endpoint\":\"https://helseid-sts.nhn.no/connect/revocation\",\"introspection_endpoint\":\"https://helseid-sts.nhn.no/connect/introspect\",\"device_authorization_endpoint\":\"https://helseid-sts.nhn.no/connect/deviceauthorization\",\"backchannel_authentication_endpoint\":\"https://helseid-sts.nhn.no/connect/ciba\",\"pushed_authorization_request_endpoint\":\"https://helseid-sts.nhn.no/connect/par\",\"require_pushed_authorization_requests\":false,\"frontchannel_logout_supported\":true,\"frontchannel_logout_session_supported\":true,\"backchannel_logout_supported\":true,\"backchannel_logout_session_supported\":true,\"claims_supported\":[\"name\",\"family_name\",\"given_name\",\"middle_name\",\"helseid://claims/identity/assurance_level\",\"helseid://claims/identity/pid\",\"helseid://claims/identity/pid_pseudonym\",\"helseid://claims/identity/security_level\",\"helseid://claims/identity/network\",\"helseid://claims/hpr/hpr_number\",\"helseid://claims/client/client_name\",\"helseid://claims/client/client_tenancy\",\"helseid://claims/client/claims/orgnr_parent\",\"helseid://claims/client/claims/orgnr_child\",\"helseid://claims/client/claims/orgnr_supplier\",\"client_amr\"],\"scopes_supported\":[\"openid\",\"profile\",\"offline_access\",\"helseid://scopes/client/info\",\"helseid://scopes/client/client_name\",\"helseid://scopes/identity/assurance_level\",\"helseid://scopes/identity/pid\",\"helseid://scopes/identity/pid_pseudonym\",\"helseid://scopes/identity/security_level\",\"helseid://scopes/identity/network\",\"nhn:tillitsrammeverk:parameters\",\"nhn:sfm:journal-id\",\"helseid://scopes/hpr/hpr_number\"],\"grant_types_supported\":[\"authorization_code\",\"client_credentials\",\"refresh_token\",\"implicit\",\"urn:ietf:params:oauth:grant-type:device_code\",\"urn:openid:params:grant-type:ciba\",\"urn:ietf:params:oauth:grant-type:token-exchange\"],\"response_types_supported\":[\"code\",\"token\",\"id_token\",\"id_token token\",\"code id_token\",\"code token\",\"code id_token token\"],\"response_modes_supported\":[\"form_post\",\"query\",\"fragment\"],\"token_endpoint_auth_methods_supported\":[\"client_secret_basic\",\"client_secret_post\",\"private_key_jwt\"],\"id_token_signing_alg_values_supported\":[\"RS256\"],\"subject_types_supported\":[\"public\"],\"code_challenge_methods_supported\":[\"plain\",\"S256\"],\"request_parameter_supported\":true,\"request_object_signing_alg_values_supported\":[\"RS256\",\"RS384\",\"RS512\",\"PS256\",\"PS384\",\"PS512\",\"ES256\",\"ES384\",\"ES512\",\"HS256\",\"HS384\",\"HS512\"],\"prompt_values_supported\":[\"none\",\"login\",\"consent\",\"select_account\"],\"authorization_response_iss_parameter_supported\":true,\"backchannel_token_delivery_modes_supported\":[\"poll\"],\"backchannel_user_code_parameter_supported\":true,\"dpop_signing_alg_values_supported\":[\"RS256\",\"RS384\",\"RS512\",\"PS256\",\"PS384\",\"PS512\",\"ES256\",\"ES384\",\"ES512\"],\"expired_jwks_uri\":\"https://helseid-sts.nhn.no/connect/expiredjwks\",\"available_idps\":\"https://helseid-sts.nhn.no/connect/availableidps\"}";

    private const string JwksEndpoint = "https://helseid-sts.nhn.no/.well-known/openid-configuration/jwks";
    private const string JwksEndpointContent =
        "{\"keys\":[{\"kty\":\"RSA\",\"use\":\"sig\",\"kid\":\"B8919616C1E79E875A74E0C6B0274E7130D50A63\",\"x5t\":\"uJGWFsHnnodadODGsCdOcTDVCmM\",\"e\":\"AQAB\",\"n\":\"yZ52clezDr0zUJAIliqL6ciicYMBWbHW9Hd5S32mX9KvEXAv4Oz0Mw9iiXcUZBPMl_Nsz2pL-KjY9c9mKx3No_5Lyi3fZSdqdpAebG9NjRG4jp8YH4O6XBPebi1Hugs3f9X4_OXdBkw0PFRGAET6SjHmpOt-qEpnbsZ_hRmNCyOjnCkL6Qecf4sArxElIPK0VCdDzCtfZzb4D650GYm7jPLmK1aFwsrsPlSucxP37jrXbszQ6TDGSDQMpYUX9IM4RIDeA2M1BrrQDi7e4wT2sgXBE57gFaaPzPVfCVq_o3Sh_8HJH1ZNw45aBtdJoCgabFi1DT-nceNAVlzVjxUgtuFxUsNyzybP7eoLNNx_AaBX-6WeLOxnQd5o1i6oh8TkoU9W0erFaHkD6YECQ6uQAMc2quUc4LOGaFQNtxCXqsixrNDvbDiUgHg7lwk6MH8iUOU5X5xPcuxwUrppTP74jTTMMnKY8r64tsLR_Un-gk-8j0rYTKg4U44XDeBqcNT5\",\"x5c\":[\"MIIGNjCCBB6gAwIBAgIKNKBuS6/YX\\u002BmJgTANBgkqhkiG9w0BAQsFADBoMQswCQYDVQQGEwJOTzEYMBYGA1UEYQwPTlRSTk8tOTgzMTYzMzI3MRMwEQYDVQQKDApCdXlwYXNzIEFTMSowKAYDVQQDDCFCdXlwYXNzIENsYXNzIDMgQ0EgRzIgU1QgQnVzaW5lc3MwHhcNMjMwNzMxMTM1MjQ0WhcNMjYxMTE3MjI1OTAwWjBzMQswCQYDVQQGEwJOTzEbMBkGA1UECgwSTk9SU0sgSEVMU0VORVRUIFNGMRAwDgYDVQQLDAdIZWxzZUlEMRswGQYDVQQDDBJOT1JTSyBIRUxTRU5FVFQgU0YxGDAWBgNVBGEMD05UUk5PLTk5NDU5ODc1OTCCAaIwDQYJKoZIhvcNAQEBBQADggGPADCCAYoCggGBAMmednJXsw69M1CQCJYqi\\u002BnIonGDAVmx1vR3eUt9pl/SrxFwL\\u002BDs9DMPYol3FGQTzJfzbM9qS/io2PXPZisdzaP\\u002BS8ot32UnanaQHmxvTY0RuI6fGB\\u002BDulwT3m4tR7oLN3/V\\u002BPzl3QZMNDxURgBE\\u002Bkox5qTrfqhKZ27Gf4UZjQsjo5wpC\\u002BkHnH\\u002BLAK8RJSDytFQnQ8wrX2c2\\u002BA\\u002BudBmJu4zy5itWhcLK7D5UrnMT9\\u002B46127M0Okwxkg0DKWFF/SDOESA3gNjNQa60A4u3uME9rIFwROe4BWmj8z1Xwlav6N0of/ByR9WTcOOWgbXSaAoGmxYtQ0/p3HjQFZc1Y8VILbhcVLDcs8mz\\u002B3qCzTcfwGgV/ulnizsZ0HeaNYuqIfE5KFPVtHqxWh5A\\u002BmBAkOrkADHNqrlHOCzhmhUDbcQl6rIsazQ72w4lIB4O5cJOjB/IlDlOV\\u002BcT3LscFK6aUz\\u002B\\u002BI00zDJymPK\\u002BuLbC0f1J/oJPvI9K2EyoOFOOFw3ganDU\\u002BQIDAQABo4IBVTCCAVEwCQYDVR0TBAIwADAfBgNVHSMEGDAWgBSBRZwplWY4N\\u002B07VE6OMuWWIXnVZDAdBgNVHQ4EFgQUAdigSQk1lF5Khg1/CkMT5PAcuSowDgYDVR0PAQH/BAQDAgZAMB8GA1UdIAQYMBYwCgYIYIRCARoBAwIwCAYGBACPegEBMDsGA1UdHwQ0MDIwMKAuoCyGKmh0dHA6Ly9jcmwuYnV5cGFzc2NhLmNvbS9CUENsM0NhRzJTVEJTLmNybDBvBggrBgEFBQcBAQRjMGEwJwYIKwYBBQUHMAGGG2h0dHA6Ly9vY3NwYnMuYnV5cGFzc2NhLmNvbTA2BggrBgEFBQcwAoYqaHR0cDovL2NydC5idXlwYXNzY2EuY29tL0JQQ2wzQ2FHMlNUQlMuY2VyMCUGCCsGAQUFBwEDBBkwFzAVBggrBgEFBQcLAjAJBgcEAIvsSQECMA0GCSqGSIb3DQEBCwUAA4ICAQBmpPtARpWz5ETgaRa84cfbRbZcPbs\\u002BxCRvGRvA8Ez2FSJ9C1bwhT08jsjkeXIzkRyoTiNTijxhf5FIDgZNzM0XiV5JzIbNM8QeZhJ8nQWHBpy3dFqSWyrNml5Jz/PepTs8KQgQ\\u002BgzgwB2BtvSnt2hFgEubmwIQ8j/S9zJaKCMVPcQPOQ8\\u002B8kWUtESa7I4SOvhLvuSg08IQ6npLpEr9U\\u002BdMMWgBSnUDwYJ0r9JqtUZNXEtnZPDiDfcvyCggI640Iv1PBPT\\u002B7v\\u002B\\u002BFhrAGNXhUgvS5QYTwtL1TUCMSnDtW1UhAMt\\u002BsLmYYeclli\\u002BfD0inpP0xQ6ZDGyNN141DggW4\\u002BDMfyvVoOdFu36fzwgHc5ED2PvAeyKXMpJUsh7BMZteaEltyn4LbQlgZ1GEsqreiG4mnoYIeeCjd0UKIgDz6fAtuICJTFqWMb\\u002BQSnefpk6SFN/OoY8RPNZBRIbuGTZHdmmorDi2NSGIAGCtCsW\\u002BmfKY1DMns/SGdeZfd/Q80uKR6T4ZVyy1mDTKOqZ17uUdfkCWnLKhxJdGm44VTdvtOCf8jbPsX9soBg320rTLJZKlWFzSq6fznSOL4peoHgYkGFY2NfuTrHuN/2FDxm74geOKZSvRxs9/3IVjY8dGNkqu1Jo\\u002BgU5yqii5N5QSgl1kGyOXPPrkCjQlWGTsh7ytkkq\\u002BsgQ==\"],\"alg\":\"RS256\"}]}";
    
    private readonly HelseIdConfiguration _mockConfiguration = new()
    {
        ClientId = "clientid",
        Scope = "scope",
        IssuerUri = StsUrl,
    };
    
    [SetUp]
    public void Setup()
    {
        _discoveryDocumentCacheMock = new DiscoveryDocumentCacheMock();
        
        var mockHttpMessageHandler = new MockHttpMessageHandlerWithCount();
        mockHttpMessageHandler
            .When(MetadataEndpoint)
            .Respond(new StringContent(MetadataEndpointContent));
        
        mockHttpMessageHandler
            .When(JwksEndpoint)
            .Respond(new StringContent(JwksEndpointContent));

        _httpClientFactoryMock = new HttpClientFactoryMock(mockHttpMessageHandler);

        _discoveryDocumentGetter =
            new DiscoveryDocumentGetter(new HelseIdConfigurationGetterMock(_mockConfiguration), _httpClientFactoryMock, _discoveryDocumentCacheMock);
    }

    [Test]
    public async Task GetDiscoveryDocument_calls_the_endpoint()
    {
        var result = await _discoveryDocumentGetter.GetDiscoveryDocument();

        result.Should().NotBeNull();
        result.AuthorizeEndpoint.Should().Be("https://helseid-sts.nhn.no/connect/authorize");
    }
    
    [Test]
    public async Task GetDiscoveryDocument_caches_the_response()
    {
        await _discoveryDocumentGetter.GetDiscoveryDocument();

        _discoveryDocumentCacheMock.CachedData.Should().NotBeNull();
    }
    
    [Test]
    public async Task GetDiscoveryDocument_does_not_call_the_endpoint_when_the_cache_is_set()
    {
        await _discoveryDocumentGetter.GetDiscoveryDocument();
        
        await _discoveryDocumentGetter.GetDiscoveryDocument();

        _httpClientFactoryMock.RequestCount.Should().Be(1);
    }
}
