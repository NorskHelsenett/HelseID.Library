using FluentAssertions;
using HelseID.Standard.Exceptions;
using HelseID.Standard.Services.PayloadClaimCreators;
using HelseID.Standard.Tests.Configuration;
using HelseID.Standard.Tests.Mocks;
using Microsoft.Extensions.Time.Testing;

namespace HelseID.Standard.Tests.Services.PayloadClaimCreators;

[TestFixture]
public class RequestObjectPayloadClaimsCreatorTests : ConfigurationTests
{
    private FakeTimeProvider _fakeTimeProvider = null!;
    private AuthorizationDetailsCreatorMock _authorizationDetailsCreatorMock = null!;
    private RequestObjectPayloadClaimsCreator _requestObjectPayloadClaimsCreator = null!;
    
    [SetUp]
    public void Setup()
    {
        PayloadClaimParameters.UseClientDetailsInClientAssertion = false;
        PayloadClaimParameters.UseClientDetailsInRequestObject = true;
        PayloadClaimParameters.UseRequestObjects = true;
        
        _fakeTimeProvider = new FakeTimeProvider();
        _fakeTimeProvider.SetUtcNow(new DateTime(2024, 12, 31, 13, 37, 01));
        
        _authorizationDetailsCreatorMock = new AuthorizationDetailsCreatorMock();
        
        _requestObjectPayloadClaimsCreator = new RequestObjectPayloadClaimsCreator(_fakeTimeProvider, _authorizationDetailsCreatorMock);
    }
    
    [Test]
    public void CreatePayloadClaims_checks_parameters()
    {
        PayloadClaimParameters.UseClientDetailsInClientAssertion = true;
        PayloadClaimParameters.UseClientDetailsInRequestObject = true;
        
        Action createPayloadClaims = () => _requestObjectPayloadClaimsCreator.CreatePayloadClaims(HelseIdConfiguration, PayloadClaimParameters);

        createPayloadClaims.Should().Throw<PayloadClaimParametersException>();
    }
    
    [Test]
    public void CreatePayloadClaims_calls_authorization_details_creator()
    {
        _requestObjectPayloadClaimsCreator.CreatePayloadClaims(HelseIdConfiguration, PayloadClaimParameters);

        _authorizationDetailsCreatorMock.PayloadClaimParameters.Should().NotBeNull();
    }

    [Test]
    public void CreatePayloadClaims_returns_empty_list_when_request_object_is_not_set_in_configuration()
    {
        PayloadClaimParameters.UseRequestObjects = false;
        
        var claims = _requestObjectPayloadClaimsCreator.CreatePayloadClaims(HelseIdConfiguration, PayloadClaimParameters).ToArray();

        claims.Should().NotBeNull();
        claims.Length.Should().Be(0);
    }

    [Test]
    public void CreatePayloadClaims_sets_expected_claims()
    {
        var claims = _requestObjectPayloadClaimsCreator.CreatePayloadClaims(HelseIdConfiguration, PayloadClaimParameters).ToArray();
        
        claims.Should().NotBeNull();
        claims.Length.Should().Be(7);
        
        claims[0].Key.Should().Be("iss");
        claims[0].Value.Should().Be(ClientId);
        
        claims[1].Key.Should().Be("aud");
        claims[1].Value.Should().Be(StsUrl);
        
        claims[2].Key.Should().Be("exp");
        claims[2].Value.Should().Be(1735648631);
        
        claims[3].Key.Should().Be("nbf");
        claims[3].Value.Should().Be(1735648621);
        
        claims[4].Key.Should().Be("client_id");
        claims[4].Value.Should().Be(ClientId);

        claims[5].Key.Should().Be("jti");
        claims[5].Value.Should().BeOfType<string>();
        var jti = claims[5].Value as string;
        jti.Should().HaveLength(32);
        
        claims[6].Key.Should().Be("authorization_details");
        claims[6].Value.Should().BeOfType<string>();
        claims[6].Value.Should().Be("some_object_value");
    }

    [Test]
    public void CreatePayloadClaims_does_not_set_authorization_details_when_payload_claim_parameters_does_not_have_any_usages()
    {
        PayloadClaimParameters.UseSfmId = false;
        
        var claims = _requestObjectPayloadClaimsCreator.CreatePayloadClaims(HelseIdConfiguration, PayloadClaimParameters).ToArray();
        
        claims.Length.Should().Be(6);
        _authorizationDetailsCreatorMock.PayloadClaimParameters.Should().BeNull();
    }
    
    [Test]
    public void CreatePayloadClaims_does_not_set_authorization_details_when_use_of_client_details_in_client_authorization_is_not_se()
    {
        PayloadClaimParameters.UseClientDetailsInRequestObject = false;
        
        var claims = _requestObjectPayloadClaimsCreator.CreatePayloadClaims(HelseIdConfiguration, PayloadClaimParameters).ToArray();
        
        claims.Length.Should().Be(6);
        _authorizationDetailsCreatorMock.PayloadClaimParameters.Should().BeNull();
    }
}
