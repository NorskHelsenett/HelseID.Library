using FluentAssertions;
using HelseID.Standard.Services.JwtTokens;
using HelseID.Standard.Tests.Configuration;
using HelseID.Standard.Tests.Mocks;

namespace HelseID.Standard.Tests.Services.JwtTokens;

[TestFixture]
public class SigningTokenCreatorTests : ConfigurationTests
{
    private SigningTokenCreator _signingTokenCreator = null!;
    private PayloadClaimsCreatorMock _claimsCreatorMock = null!;

    [SetUp]
    public void Setup()
    {
        _claimsCreatorMock = new();
        
        _signingTokenCreator = new SigningTokenCreator(HelseIdConfiguration);
    }

    [Test]
    public void CreateSigningToken_sets_parameters_and_configuration_to_signing_token_creator()
    {
        _signingTokenCreator.CreateSigningToken(_claimsCreatorMock, PayloadClaimParameters);

        _claimsCreatorMock.PayloadClaimParameters.Should().BeEquivalentTo(PayloadClaimParameters);
        _claimsCreatorMock.HelseIdConfiguration.Should().BeEquivalentTo(HelseIdConfiguration);
    }

    
    [Test]
    public void CreateSigningToken_creates_a_jwt_token()
    {
        var jwt = _signingTokenCreator.CreateSigningToken(_claimsCreatorMock, PayloadClaimParameters);

        jwt.Should().NotBeNullOrEmpty();
        jwt.Should().Be("eyJhbGciOiJSUzM4NCIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJkMDYxMmIyNy0xNzFkLTRlNjEtODdmMS05YjU3NGMwMmQxOTUiLCJzdWIiOiJkMDYxMmIyNy0xNzFkLTRlNjEtODdmMS05YjU3NGMwMmQxOTUiLCJqdGkiOiIzYWZiM2Y1Zi04MzVlLTQ5YzAtOTczNi1kN2JhNTUwMmE4M2EifQ.bOgX616YAJzGj6r4PyOQQ4PXsAt1wWT6se7-wLmnNlZ09TzsCR_hECOqKRe7DZxmi_mIq7iMZ9F58ZLM96CqmMx7BqQwXSMc4iUNCXrIqxgYtbGaVo4HxEpKvfxfJUjNoGmwjmKrOZ_TUAiiXKkcIHJ9ZKHjg_TwovUActXMZTkFlGSXBgaLlFZZGiVxOwz4DmRkaQ2rO4IS-Oao2CyWqvgyBbMaYDmWcTzYZ0Al5LN0pySFeMUNDhedPbZIGgmnFgH-4qRWB1HUd1o5NLgoS--6gLCjvj38E9sEGhBDm8-Eua-ZnwHXFcDJ25daiutupuo96u8-3eVXJc2wRp2KIg");
    }
}
