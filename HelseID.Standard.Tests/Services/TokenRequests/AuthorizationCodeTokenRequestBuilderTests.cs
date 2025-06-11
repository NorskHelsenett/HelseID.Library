using FluentAssertions;
using HelseId.Standard.Models.TokenRequests;
using HelseId.Standard.Services.TokenRequests;
using HelseId.Standard.Tests.Mocks;

namespace HelseId.Standard.Tests.Services.TokenRequests;

[TestFixture, Ignore("Ikke klar")]
public class AuthorizationCodeTokenRequestBuilderTests : TokenRequestBuilderTests
{
    private AuthorizationCodeTokenRequestBuilder _authorizationCodeTokenRequestBuilder = null!;
    
    private AuthorizationCodeTokenRequestParameters _authorizationCodeTokenRequestParameters = null!;

    private const string Code = "code";
    
    private const string CodeVerifier = "CodeVerifier";

    private const string RedirectUri = "https://foo.bar";

    private ICollection<string> _resource = null!;

    [SetUp]
    public void Setup()
    {
        _authorizationCodeTokenRequestBuilder = new AuthorizationCodeTokenRequestBuilder(SigningTokenCreatorMock,
            DpoPProofCreatorMock,
            HelseIdEndpointsDiscovererMock,
            HelseIdConfiguration);

        _resource = new List<string>()
        {
            "one", "two",
        };

        _authorizationCodeTokenRequestParameters = new AuthorizationCodeTokenRequestParameters(
            Code,
            CodeVerifier,
            RedirectUri,
            _resource)
        {
            PayloadClaimParameters = PayloadClaimParameters,
        };
    }
    
    [Test]
    public async Task CreateTokenRequest_finds_token_endpoint()
    {
        await _authorizationCodeTokenRequestBuilder.CreateTokenRequest(
            PayloadClaimsCreatorMock,
            _authorizationCodeTokenRequestParameters);

        HelseIdEndpointsDiscovererMock.GetTokenEndpointFromHelseIdCalled.Should().BeTrue();
    }
    
    [Test]
    public async Task CreateTokenRequest_builds_client_assertion()
    {
        await _authorizationCodeTokenRequestBuilder.CreateTokenRequest(
            PayloadClaimsCreatorMock,
            _authorizationCodeTokenRequestParameters);

        SigningTokenCreatorMock.PayloadClaimsCreator.Should().Be(PayloadClaimsCreatorMock);
        SigningTokenCreatorMock.PayloadClaimParameters.Should().BeEquivalentTo(PayloadClaimParameters);
    }
    
    [Test]
    public async Task CreateTokenRequest_builds_dpop_proof()
    {
        await _authorizationCodeTokenRequestBuilder.CreateTokenRequest(
            PayloadClaimsCreatorMock,
            _authorizationCodeTokenRequestParameters);

        DpoPProofCreatorMock.Url.Should().Be(HelseIdEndpointsDiscovererMock.TokenEndpoint);
        DpoPProofCreatorMock.HttpMethod.Should().Be("POST");
        DpoPProofCreatorMock.DPoPNonce.Should().BeNull();
        DpoPProofCreatorMock.AccessToken.Should().BeNull();
    }
    
    [Test]
    public async Task CreateTokenRequest_builds_dpop_proof_with_dpop_nonce()
    {
        var dPoPNonce = "2bc376b6-68ac-46d8-837e-3b5e02530b62";
        
        await _authorizationCodeTokenRequestBuilder.CreateTokenRequest(
            PayloadClaimsCreatorMock,
            _authorizationCodeTokenRequestParameters,
            dPoPNonce);

        DpoPProofCreatorMock.DPoPNonce.Should().Be(dPoPNonce);
    }
    
    [Test]
    public async Task CreateTokenRequest_creates_request()
    {
        var request = await _authorizationCodeTokenRequestBuilder.CreateTokenRequest(
            PayloadClaimsCreatorMock,
            _authorizationCodeTokenRequestParameters);
        
        request.Should().NotBeNull();
        request.Address.Should().Be(HelseIdEndpointsDiscovererMock.TokenEndpoint);
        request.ClientAssertion.Should().Be(SigningTokenCreatorMock.Value);
        request.ClientId.Should().Be(ClientId);
        //request.Resource.Should().BeEquivalentTo(_resource);
        //request.Code.Should().Be(Code);
        //request.RedirectUri.Should().Be(RedirectUri);
        //request.CodeVerifier.Should().Be(CodeVerifier);
        request.GrantType.Should().Be("authorization_code");
        request.DPoPProofToken.Should().Be(DPoPProofCreatorMock.DPoPProof);
    }
}
