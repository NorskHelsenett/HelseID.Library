using FluentAssertions;
using HelseID.Standard.Models.TokenRequests;
using HelseID.Standard.Services.TokenRequests;
using HelseID.Standard.Tests.Mocks;
using IdentityModel.Client;

namespace HelseID.Standard.Tests.Services.TokenRequests;

[TestFixture, Ignore("Ikke klar")]
public class RefreshTokenRequestBuilderTests : TokenRequestBuilderTests
{
    private RefreshTokenRequestBuilder _refreshTokenRequestBuilder = null!;
    
    private RefreshTokenRequestParameters _refreshTokenRequestParameters = null!;
    
    private const string RefreshToken = "refresh_token";
    private const string Resource = "resource";
 
    [SetUp]
    public void Setup()
    {
        _refreshTokenRequestBuilder = new RefreshTokenRequestBuilder(SigningTokenCreatorMock,
            DpoPProofCreatorMock,
            HelseIdEndpointsDiscovererMock,
            HelseIdConfiguration);

        _refreshTokenRequestParameters = new RefreshTokenRequestParameters(RefreshToken, Resource)
        {
            PayloadClaimParameters = PayloadClaimParameters,
        };
    }

    [Test]
    public async Task CreateTokenRequest_finds_token_endpoint()
    {
        await _refreshTokenRequestBuilder.CreateTokenRequest(
            PayloadClaimsCreatorMock,
            _refreshTokenRequestParameters);

        HelseIdEndpointsDiscovererMock.GetTokenEndpointFromHelseIdCalled.Should().BeTrue();
    }
    
    [Test]
    public async Task CreateTokenRequest_builds_client_assertion()
    {
        await _refreshTokenRequestBuilder.CreateTokenRequest(
            PayloadClaimsCreatorMock,
            _refreshTokenRequestParameters);

        SigningTokenCreatorMock.PayloadClaimsCreator.Should().Be(PayloadClaimsCreatorMock);
        SigningTokenCreatorMock.PayloadClaimParameters.Should().BeEquivalentTo(PayloadClaimParameters);
    }
    
    [Test]
    public async Task CreateTokenRequest_builds_dpop_proof()
    {
        await _refreshTokenRequestBuilder.CreateTokenRequest(
            PayloadClaimsCreatorMock,
            _refreshTokenRequestParameters);

        DpoPProofCreatorMock.Url.Should().Be(HelseIdEndpointsDiscovererMock.TokenEndpoint);
        DpoPProofCreatorMock.HttpMethod.Should().Be("POST");
        DpoPProofCreatorMock.DPoPNonce.Should().BeNull();
        DpoPProofCreatorMock.AccessToken.Should().BeNull();
    }
    
    [Test]
    public async Task CreateTokenRequest_builds_dpop_proof_with_dpop_nonce()
    {
        var dPoPNonce = "2bc376b6-68ac-46d8-837e-3b5e02530b62";
        
        await _refreshTokenRequestBuilder.CreateTokenRequest(
            PayloadClaimsCreatorMock,
            _refreshTokenRequestParameters,
            dPoPNonce);

        DpoPProofCreatorMock.DPoPNonce.Should().Be(dPoPNonce);
    }
    
    [Test]
    public async Task CreateTokenRequest_creates_request()
    {
        var request = await _refreshTokenRequestBuilder.CreateTokenRequest(
            PayloadClaimsCreatorMock,
            _refreshTokenRequestParameters);
        
        request.Should().NotBeNull();
        request.Address.Should().Be(HelseIdEndpointsDiscovererMock.TokenEndpoint);
        request.ClientAssertion.Should().Be(SigningTokenCreatorMock.Value);
        request.ClientId.Should().Be(ClientId);
        request.GrantType.Should().Be("refresh_token");
        //request.RefreshToken.Should().Be(RefreshToken);
        request.DPoPProofToken.Should().Be(DPoPProofCreatorMock.DPoPProof);
        //request.Resource.Should().BeEquivalentTo(Resource);
    }
    
    [Test]
    public async Task CreateTokenRequest_does_not_set_resource()
    {
        _refreshTokenRequestParameters = new RefreshTokenRequestParameters(RefreshToken)
        {
            PayloadClaimParameters = PayloadClaimParameters,
        };

        var request = await _refreshTokenRequestBuilder.CreateTokenRequest(
            PayloadClaimsCreatorMock,
            _refreshTokenRequestParameters);
        
        //request.Resource.Should().BeEquivalentTo(new HashSet<string>());
    }
}
