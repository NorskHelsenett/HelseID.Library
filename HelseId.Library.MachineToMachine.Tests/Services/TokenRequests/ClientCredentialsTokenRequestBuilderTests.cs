using HelseId.Library.MachineToMachine.Models.TokenRequests;
using HelseId.Library.MachineToMachine.Services.TokenRequests;
using HelseId.Standard.Tests.Services.TokenRequests;

namespace HelseId.Library.MachineToMachine.Tests.Services.TokenRequests;

[TestFixture]
public class ClientCredentialsTokenRequestBuilderTests : TokenRequestBuilderTests
{
    private ClientCredentialsTokenRequestBuilder _clientCredentialsTokenRequestBuilder = null!;

    private ClientCredentialsTokenRequestParameters _clientCredentialsTokenRequestParameters = null!;
    
    [SetUp]
    public void Setup()
    {
        _clientCredentialsTokenRequestBuilder = new ClientCredentialsTokenRequestBuilder(
            SigningTokenCreatorMock,
            DpoPProofCreatorMock,
            HelseIdEndpointsDiscovererMock,
            HelseIdConfiguration);
        
        _clientCredentialsTokenRequestParameters = new ClientCredentialsTokenRequestParameters()
        {
            PayloadClaimParameters = PayloadClaimParameters,
        };
    }

    [Test]
    public async Task CreateTokenRequest_finds_token_endpoint()
    {
        await _clientCredentialsTokenRequestBuilder.CreateTokenRequest(
            PayloadClaimsCreatorMock,
            _clientCredentialsTokenRequestParameters);

        HelseIdEndpointsDiscovererMock.GetTokenEndpointFromHelseIdCalled.Should().BeTrue();
    }
    
    [Test]
    public async Task CreateTokenRequest_builds_client_assertion()
    {
        await _clientCredentialsTokenRequestBuilder.CreateTokenRequest(
            PayloadClaimsCreatorMock,
            _clientCredentialsTokenRequestParameters);

        SigningTokenCreatorMock.PayloadClaimsCreator.Should().Be(PayloadClaimsCreatorMock);
        SigningTokenCreatorMock.PayloadClaimParameters.Should().BeEquivalentTo(PayloadClaimParameters);
    }
    
    [Test]
    public async Task CreateTokenRequest_builds_dpop_proof()
    {
        await _clientCredentialsTokenRequestBuilder.CreateTokenRequest(
            PayloadClaimsCreatorMock,
            _clientCredentialsTokenRequestParameters);

        DpoPProofCreatorMock.Url.Should().Be(HelseIdEndpointsDiscovererMock.TokenEndpoint);
        DpoPProofCreatorMock.HttpMethod.Should().Be("POST");
        DpoPProofCreatorMock.DPoPNonce.Should().BeNull();
        DpoPProofCreatorMock.AccessToken.Should().BeNull();
    }
    
    [Test]
    public async Task CreateTokenRequest_builds_dpop_proof_with_dpop_nonce()
    {
        var dPoPNonce = "2bc376b6-68ac-46d8-837e-3b5e02530b62";
        
        await _clientCredentialsTokenRequestBuilder.CreateTokenRequest(
            PayloadClaimsCreatorMock,
            _clientCredentialsTokenRequestParameters,
            dPoPNonce);

        DpoPProofCreatorMock.DPoPNonce.Should().Be(dPoPNonce);
    }
    
    [Test]
    public async Task CreateTokenRequest_creates_request()
    {
        var request = await _clientCredentialsTokenRequestBuilder.CreateTokenRequest(
            PayloadClaimsCreatorMock,
            _clientCredentialsTokenRequestParameters);
        
        request.Should().NotBeNull();
        request.Address.Should().Be(HelseIdEndpointsDiscovererMock.TokenEndpoint);
        request.ClientAssertion.Should().Be(SigningTokenCreatorMock.Value);
        request.ClientId.Should().Be(ClientId);
        request.Scope.Should().Be(Scope);
        request.GrantType.Should().Be("client_credentials");
        request.DPoPProofToken.Should().Be(DPoPProofCreatorMock.DPoPProof);
    }
}
