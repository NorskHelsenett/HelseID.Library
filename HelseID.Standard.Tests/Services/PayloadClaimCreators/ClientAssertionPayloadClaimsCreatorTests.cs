using FluentAssertions;
using HelseID.Standard.Exceptions;
using HelseID.Standard.Services.PayloadClaimCreators;
using HelseID.Standard.Tests.Configuration;
using HelseID.Standard.Tests.Mocks;
using Microsoft.Extensions.Time.Testing;

namespace HelseID.Standard.Tests.Services.PayloadClaimCreators;

[TestFixture]
public class ClientAssertionPayloadClaimsCreatorTests : ConfigurationTests
{
    private FakeTimeProvider _fakeTimeProvider = null!;
    private AssertionDetailsCreatorMock _assertionDetailsCreatorMock = null!;
    private ClientAssertionPayloadClaimsCreator _clientAssertionPayloadClaimsCreator = null!;
    
    [SetUp]
    public void Setup()
    {
        _fakeTimeProvider = new FakeTimeProvider();
        _fakeTimeProvider.SetUtcNow(new DateTime(2024, 12, 31, 13, 37, 00));
        
        _assertionDetailsCreatorMock = new();
        
        _clientAssertionPayloadClaimsCreator = new ClientAssertionPayloadClaimsCreator(_fakeTimeProvider, _assertionDetailsCreatorMock);
    }

    [Test]
    public void CreatePayloadClaims_checks_parameters()
    {
        PayloadClaimParameters.UseClientDetailsInRequestObject = true;
        
        Action createPayloadClaims = () => _clientAssertionPayloadClaimsCreator.CreatePayloadClaims(HelseIdConfiguration, PayloadClaimParameters);

        createPayloadClaims.Should().Throw<PayloadClaimParametersException>();
    }
    
    [Test]
    public void CreatePayloadClaims_calls_assertion_details_creator()
    {
        _clientAssertionPayloadClaimsCreator.CreatePayloadClaims(HelseIdConfiguration, PayloadClaimParameters);

        _assertionDetailsCreatorMock.PayloadClaimParameters.Should().NotBeNull();
    }

    [Test]
    public void CreatePayloadClaims_sets_expected_claims()
    {
        var claims = _clientAssertionPayloadClaimsCreator.CreatePayloadClaims(HelseIdConfiguration, PayloadClaimParameters).ToArray();
        
        claims.Should().NotBeNull();
        claims.Length.Should().Be(7);
        
        claims[0].Key.Should().Be("iss");
        claims[0].Value.Should().Be(ClientId);
        
        claims[1].Key.Should().Be("sub");
        claims[1].Value.Should().Be(ClientId);
        
        claims[2].Key.Should().Be("aud");
        claims[2].Value.Should().Be(StsUrl);
        
        claims[3].Key.Should().Be("exp");
        claims[3].Value.Should().Be(1735648650);
        
        claims[4].Key.Should().Be("iat");
        claims[4].Value.Should().Be(1735648620);
        
        claims[5].Key.Should().Be("jti");
        claims[5].Value.Should().BeOfType<string>();
        var jti = claims[5].Value as string;
        jti.Should().HaveLength(32);
        
        claims[6].Key.Should().Be("assertion_details");
        claims[6].Value.Should().BeOfType<string>();
        claims[6].Value.Should().Be("some_object_value");
    }

    [Test]
    public void CreatePayloadClaims_does_not_set_assertion_details_when_payload_claim_parameters_does_not_have_any_usages()
    {
        PayloadClaimParameters.UseSfmId = false;
        
        var claims = _clientAssertionPayloadClaimsCreator.CreatePayloadClaims(HelseIdConfiguration, PayloadClaimParameters).ToArray();
        
        claims.Length.Should().Be(6);
        _assertionDetailsCreatorMock.PayloadClaimParameters.Should().BeNull();
    }
    
    [Test]
    public void CreatePayloadClaims_does_not_set_assertion_details_when_use_of_client_details_in_client_assertion_is_not_set()
    {
        PayloadClaimParameters.UseClientDetailsInClientAssertion = false;
        
        var claims = _clientAssertionPayloadClaimsCreator.CreatePayloadClaims(HelseIdConfiguration, PayloadClaimParameters).ToArray();
        
        claims.Length.Should().Be(6);
        _assertionDetailsCreatorMock.PayloadClaimParameters.Should().BeNull();
    }
}
