using HelseId.Library.Tests.Configuration;
using HelseId.Library.Tests.Mocks;
using Microsoft.IdentityModel.JsonWebTokens;

namespace HelseId.Library.Tests.Services.JwtTokens;

[TestFixture]
public class SigningTokenCreatorTests : ConfigurationTests
{
    private SigningTokenCreator _signingTokenCreator = null!;
    private PayloadClaimsCreatorMock _claimsCreatorMock = null!;

    [SetUp]
    public void Setup()
    {
        _claimsCreatorMock = new();
        
        _signingTokenCreator = new SigningTokenCreator(new HelseIdConfigurationGetterMock(HelseIdConfiguration));
    }

    [Test]
    public async Task CreateSigningToken_sets_parameters_and_configuration_to_signing_token_creator()
    {
        await _signingTokenCreator.CreateSigningToken(_claimsCreatorMock, PayloadClaimParameters);

        _claimsCreatorMock.PayloadClaimParameters.Should().BeEquivalentTo(PayloadClaimParameters);
        _claimsCreatorMock.HelseIdConfiguration.Should().BeEquivalentTo(HelseIdConfiguration);
    }

    
    [Test]
    public async Task CreateSigningToken_creates_a_jwt_token()
    {
        var jwt = await _signingTokenCreator.CreateSigningToken(_claimsCreatorMock, PayloadClaimParameters);

        jwt.Should().NotBeNullOrEmpty();
        jwt.Should().Be("eyJhbGciOiJSUzM4NCIsInR5cCI6ImNsaWVudC1hdXRoZW50aWNhdGlvbitqd3QifQ.eyJpc3MiOiJkMDYxMmIyNy0xNzFkLTRlNjEtODdmMS05YjU3NGMwMmQxOTUiLCJzdWIiOiJkMDYxMmIyNy0xNzFkLTRlNjEtODdmMS05YjU3NGMwMmQxOTUiLCJqdGkiOiIzYWZiM2Y1Zi04MzVlLTQ5YzAtOTczNi1kN2JhNTUwMmE4M2EifQ.PQhRkjeS899WxFDYJYjlA5ejnTAjzSdr9kwjmYojApLhbqLYWhm3FbRr8Pc1_YjWUf8jgPmVIKo321xBLAjzCWdXram_Iqy4DcA9WbbxJOEiFE4wQzCI8RmNy9QAWvoqih1FgRoltzKEyfsxPkquMNxjuIknZMEICm-Wxf7RFk1xxg5AdaVfh0iCVHwQfEbhM88mGz4ESCo02Bx5L6SUgBlYTgzcWU8rMcbQHVByrymNsEYVvjvsSJ0jhpcmR_vTiobkR98INomSMd_XEh0E8xF3TQQqswfwYYj4h0w1F7fzF6AA71U7UFBzJn23KBVFc5H64Lk2_8CkB-RV7TZKTQ");
    }
    
    [Test]
    public async Task CreateSigningToken_sets_type_header_from_parameters()
    {
        PayloadClaimParameters.TokenType = "123";
        var jwt = await _signingTokenCreator.CreateSigningToken(_claimsCreatorMock, PayloadClaimParameters);
        var parsedJwt = new JsonWebTokenHandler().ReadJsonWebToken(jwt);
        parsedJwt.Typ.Should().Be("123");

    }
}
