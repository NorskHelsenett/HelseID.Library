using FluentAssertions;
using HelseId.Standard.Exceptions;
using HelseId.Standard.Services.PayloadClaimCreators;
using HelseId.Standard.Tests.Configuration;
using HelseId.Standard.Tests.Mocks;
using Microsoft.Extensions.Time.Testing;

namespace HelseId.Standard.Tests.Services.PayloadClaimCreators;

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
        claims.Length.Should().Be(8);
        
        claims.Should().Contain(c => c.Key == "iss" && (string)c.Value == ClientId);
        claims.Should().Contain(c => c.Key == "sub" && (string)c.Value == ClientId);
        claims.Should().Contain(c => c.Key == "aud" && (string)c.Value == StsUrl);
        claims.Should().Contain(c => c.Key == "exp" && (long)c.Value == 1735648630);
        claims.Should().Contain(c => c.Key == "iat" && (long)c.Value == 1735648620);
        claims.Should().Contain(c => c.Key == "jti" && ((string)c.Value).Length == 32);
        claims.Should().Contain(c => c.Key == "assertion_details" && (string)c.Value == "some_object_value");
        claims.Should().Contain(c => c.Key == "nbf" && (long)c.Value == 1735648620);
    }

    [Test]
    public void CreatePayloadClaims_does_not_set_assertion_details_when_payload_claim_parameters_does_not_have_any_usages()
    {
        PayloadClaimParameters.UseSfmId = false;
        
        var claims = _clientAssertionPayloadClaimsCreator.CreatePayloadClaims(HelseIdConfiguration, PayloadClaimParameters).ToArray();
        
        claims.Length.Should().Be(7);
        _assertionDetailsCreatorMock.PayloadClaimParameters.Should().BeNull();
    }
    
    [Test]
    public void CreatePayloadClaims_does_not_set_assertion_details_when_use_of_client_details_in_client_assertion_is_not_set()
    {
        PayloadClaimParameters.UseClientDetailsInClientAssertion = false;
        
        var claims = _clientAssertionPayloadClaimsCreator.CreatePayloadClaims(HelseIdConfiguration, PayloadClaimParameters).ToArray();
        
        claims.Length.Should().Be(7);
        _assertionDetailsCreatorMock.PayloadClaimParameters.Should().BeNull();
    }
}
