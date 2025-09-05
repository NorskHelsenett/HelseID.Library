using HelseId.Library.Tests.Configuration;
using HelseId.Library.Tests.Mocks;
using Microsoft.Extensions.Time.Testing;
using Microsoft.IdentityModel.JsonWebTokens;

namespace HelseId.Library.Tests.Services.JwtTokens;

[TestFixture]
public class DPoPProofCreatorTests : ConfigurationTests
{
    private const string Url = "https://helseid-sts.nhn.no/connect/token";
    private const string HttpMethod = "POST";
    private FakeTimeProvider _fakeTimeProvider = null!;    
    private DPoPProofCreator _dPoPProofCreator = null!;
    
    [SetUp]
    public void Setup()
    {
        _fakeTimeProvider = new FakeTimeProvider();
        _fakeTimeProvider.SetUtcNow(new DateTimeOffset(2025, 1, 4, 13, 37, 00, TimeSpan.FromHours(0)));
        
        _dPoPProofCreator = new DPoPProofCreator(CredentialReference, _fakeTimeProvider);
    }
    
    [Test]
    public async Task CreateDPoPProofForTokenRequest_sets_standard_dpop_proof()
    {
        var dPoPProof = await _dPoPProofCreator.CreateDPoPProofForTokenRequest(Url, HttpMethod);

        dPoPProof.Should().NotBeNullOrEmpty();

        var token = GetJsonWebToken(dPoPProof);

        token.GetHeaderValue<object>("alg").Should().Be("RS384");
        token.GetHeaderValue<object>("typ").Should().Be("dpop+jwt");
        JsonSerializer.Serialize(token.GetHeaderValue<object>("jwk")).Should().Be("""{"kty":"RSA","n":"yegEZ3dzO9IkGqk_L8gCKLfMSlc9-MSmqYbcWhCTV_HpKCVSk16S-8XFdMgb8J0ZVxIPO_1udCv9V98y79atLrynewm9AzcVLqj8pj4zaYvtdlGMcGNBUXaLjhw57xrnsEOaF-3yg8NDXGwsJeUomPSvb1tu5qMZKPQoAwjwxe0KJXVFmriGom3RsoP2Uh4ce-1iLgVJx-h2_IqZyaCsySWP6doMKV2aRXToyDm6KH592dbMJ49Wqax5Vc2ucnWjfcR7Bj5YtDiaslv8btyNVmXwJVcEzobMrpXqnwHv_7xdkav1GvO6aU1XGIlKLPijie6W9i2JdmBDr4td0BRf0Q","e":"AQAB","alg":"RS384"}""");
        
        token.GetClaim("iat").Value.Should().Be("1735997820");
        token.GetClaim("jti").Value.Should().NotBeNullOrEmpty();
        token.GetClaim("htm").Value.Should().Be("POST");
        token.GetClaim("htu").Value.Should().Be("https://helseid-sts.nhn.no/connect/token");
    }
    
    [Test]
    public async Task CreateDPoPProofForTokenRequest_sets_dpop_proof_with_nonce()
    {
        var dPoPProof = await _dPoPProofCreator.CreateDPoPProofForTokenRequest(Url, HttpMethod, dPoPNonce: "2bc376b6-68ac-46d8-837e-3b5e02530b62");

        var token = GetJsonWebToken(dPoPProof);

        token.GetClaim("nonce").Value.Should().Be("2bc376b6-68ac-46d8-837e-3b5e02530b62");
    }
    
    [Test]
    public async Task CreateDPoPProofForApiCall_sets_dpop_proof_with_access_token_hash()
    {
        var dPoPProof = await _dPoPProofCreator.CreateDPoPProofForApiCall(Url, HttpMethod, accessToken: "eyJhbGciOiJSUzI1NiIsImtpZCI6Ijc4NjY3RjkwREMxMUJGMDRCRDk0NjdEMUY5MTIwQzRBNDM0MEI0Q0YiLCJ4NXQiOiJlR1pfa053UnZ3UzlsR2ZSLVJJTVNrTkF0TTgiLCJ0eXAiOiJhdCtqd3QifQ.eyJpc3MiOiJodHRwczovL2hlbHNlaWQtc3RzLnRlc3QubmhuLm5vIiwibmJmIjoxNzM1OTkzODkzLCJpYXQiOjE3MzU5OTM4OTMsImV4cCI6MTczNTk5NDQ5MywiYXVkIjoibmhuOnRlc3QtcHVibGljLXNhbXBsZWNvZGUiLCJjbmYiOnsiamt0IjoiYW9lenZJZTMyUmRGcFRQNEZJeHdCWWI2VkdYMGRwM2Vjdm9WakZrZFhRayJ9LCJzY29wZSI6WyJvcGVuaWQiLCJwcm9maWxlIiwibmhuOnRlc3QtcHVibGljLXNhbXBsZWNvZGUvYXV0aG9yaXphdGlvbi1jb2RlIiwiaGVsc2VpZDovL3Njb3Blcy9pZGVudGl0eS9waWQiLCJoZWxzZWlkOi8vc2NvcGVzL2lkZW50aXR5L3BpZF9wc2V1ZG9ueW0iLCJoZWxzZWlkOi8vc2NvcGVzL2lkZW50aXR5L2Fzc3VyYW5jZV9sZXZlbCIsImhlbHNlaWQ6Ly9zY29wZXMvaHByL2hwcl9udW1iZXIiLCJoZWxzZWlkOi8vc2NvcGVzL2lkZW50aXR5L25ldHdvcmsiLCJoZWxzZWlkOi8vc2NvcGVzL2lkZW50aXR5L3NlY3VyaXR5X2xldmVsIiwib2ZmbGluZV9hY2Nlc3MiXSwiYW1yIjpbInB3ZCJdLCJjbGllbnRfaWQiOiJoZWxzZWlkLXNhbXBsZS1hcGktYWNjZXNzIiwiY2xpZW50X2FtciI6InByaXZhdGVfa2V5X2p3dCIsImhlbHNlaWQ6Ly9jbGFpbXMvY2xpZW50L2NsYWltcy9vcmducl9wYXJlbnQiOiI5OTk5Nzc3NzQiLCJzdWIiOiJjNFNyaEtPYnBvRmJYXHUwMDJCdm9aVUhsRzlSQ0tkSGFDWEFTdXhsdGtDUkFRVW89IiwiYXV0aF90aW1lIjoxNzM1OTkzODkyLCJpZHAiOiJ0ZXN0aWRwLW9pZGMiLCJoZWxzZWlkOi8vY2xhaW1zL2lkZW50aXR5L3BpZCI6IjE1ODQ5MTk3MzUyIiwiaGVsc2VpZDovL2NsYWltcy9pZGVudGl0eS9zZWN1cml0eV9sZXZlbCI6IjQiLCJoZWxzZWlkOi8vY2xhaW1zL2lkZW50aXR5L2Fzc3VyYW5jZV9sZXZlbCI6ImhpZ2giLCJoZWxzZWlkOi8vY2xhaW1zL2hwci9ocHJfbnVtYmVyIjoiNTY1NTUwODQ2IiwibmFtZSI6IkxJVlNUUkVUVCBCRVZFUiIsImhlbHNlaWQ6Ly9jbGFpbXMvaWRlbnRpdHkvbmV0d29yayI6ImludGVybmV0dCIsImhlbHNlaWQ6Ly9jbGFpbXMvY2xpZW50L2NsaWVudF9uYW1lIjoiaGVsc2VpZC1zYW1wbGUtYXBpLWFjY2VzcyIsImhlbHNlaWQ6Ly9jbGFpbXMvY2xpZW50L2NsaWVudF90ZW5hbmN5Ijoic2luZ2xlLXRlbmFudCIsInNpZCI6IkNDMjQ2MkMyMzhDRjk0NDMzNTFBMzIwMDhFRTlGMTVDIiwianRpIjoiRkY4RkQ2RDMwMzUwNDkwRkExNDcwMjgxMUI3NTE5OEYifQ.f2n0rzuDAu1B4v9tGkKBTryzdBc_-nuaZxE-N0pr0cxxa6Rx8vH7WcnGd63Ie989YqhLcKi5fY_9Meym-ipJbJzxw-vOnn9D7C-oyeeqZcH8aRWzGpmvPmaeIsuc0Kj5Jxnf6QnBDUteHhb7qP7Jec4S9goOUm_hQvnA_sgAPYMK14Pou5E-av0Pc-CVFzxlhhexCApO07vQ5FsfDo17ZIQSmMIz-m3bSDthKreWEnap2Jg_ZRmoSnbbfNCXST6Fm11oND-hQeo9EHhgocZoAme8xtFUPKESAZVWbsOqkr7UBR5Nh8Kiaid_ZGUYmoyHltjPa5H_Cg1qmjctfWvdud8IIcwAlRpKLdYV5JcJxSJQKGL7vRz1bzEtZbimMS9_PBobwOSpD96HUyhYG7tqhqddxKK3-vzKx0T7MP-uYjxMA-I5DZKGjbrTxdgEip7DkjbZZ3cs-mUc7KbFapt6ijdo9CbVyILPcr7BrKBaNRb9YJD49epTLbchmSqHIQBb");

        var token = GetJsonWebToken(dPoPProof);

        token.GetClaim("ath").Value.Should().Be("WbI2YNTVO69Q7eY-08UfxZFD9hekl99kqd5WOw5f_4k");
    }

    [Test]
    public async Task CreateDPoPProofForTokenRequest_sets_standard_dpop_proof_with_elliptic_curve()
    {
        _dPoPProofCreator = new DPoPProofCreator(CredentialWithEcKey, _fakeTimeProvider);
        
        var dPoPProof = await _dPoPProofCreator.CreateDPoPProofForTokenRequest(Url, HttpMethod);

        var token = GetJsonWebToken(dPoPProof);

        token.GetHeaderValue<object>("alg").Should().Be("ES384");
        JsonSerializer.Serialize(token.GetHeaderValue<object>("jwk")).Should().Be("""{"kty":"EC","x":"_fwQ_E2rqeBOQ0YYzQBCvZNK60-n4PUG7cbJelBkuAbfEqmnaMHNUsReIsnE3432","y":"xbuUzn7GpWq7JuKgrY_QxskViWPyDk_MoIef5JXXPlWkdB24cQLVgm-Jgz8NOblZ","crv":"P-384"}""");
    }
    
    [Test]
    public void CreateDPoPProofForTokenRequest_throws_when_an_invalid_key_is_used()
    {
        _dPoPProofCreator = new DPoPProofCreator(CredentialWithInvalidKey, _fakeTimeProvider);

        Func<Task> createDPoPProof = async () => await _dPoPProofCreator.CreateDPoPProofForTokenRequest(Url, HttpMethod);
        
        createDPoPProof.Should().ThrowAsync<InvalidKeyTypeForDPoPProofException>().Result.WithMessage("An invalid key was set for the DPoP proof.");
    }
    
    [Test, Ignore("Støtter ikke x509 i vår kode nå")]
    public async Task CreateDPoPProofForTokenRequest_sets_standard_dpop_proof_with_x509_certificate()
    {
        var configurationGetter = new HelseIdConfigurationGetterMock(SetHelseIdConfigurationWithX509());
        _dPoPProofCreator = new DPoPProofCreator(CredentialReference, _fakeTimeProvider);
        
        var dPoPProof = await _dPoPProofCreator.CreateDPoPProofForTokenRequest(Url, HttpMethod);

        var token = GetJsonWebToken(dPoPProof);

        token.GetHeaderValue<object>("alg").Should().Be("RS384");
        JsonSerializer.Serialize(token.GetHeaderValue<object>("jwk")).Should().StartWith("""{"kty":"RSA","n":""");
        JsonSerializer.Serialize(token.GetHeaderValue<object>("jwk")).Should().EndWith(""","e":"AQAB","alg":"RS384"}""");
    }
    
    [Test]
    public async Task CreateDPoPProofForTokenRequest_rejects_url_with_query_string_parameter()
    {
        _dPoPProofCreator = new DPoPProofCreator(CredentialWithEcKey, _fakeTimeProvider);
        
        Func<Task> createProofWithInvalidUrl = async () => await _dPoPProofCreator.CreateDPoPProofForTokenRequest("https://server.com/?query=a&string=b", HttpMethod);

        await createProofWithInvalidUrl.Should().ThrowAsync<HelseIdException>();
    }
    
    private static JsonWebToken GetJsonWebToken(string dPoPProof)
    {
        dPoPProof.Should().NotBeNullOrEmpty();
        JsonWebTokenHandler handler = new JsonWebTokenHandler();
        var token = handler.ReadToken(dPoPProof) as JsonWebToken;
        token.Should().NotBeNull();
        return token!;
    }
}
