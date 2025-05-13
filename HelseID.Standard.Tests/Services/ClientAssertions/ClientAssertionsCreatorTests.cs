using FluentAssertions;
using HelseID.Standard.Models.Payloads;
using HelseID.Standard.Services.ClientAssertions;
using HelseID.Standard.Tests.Mocks;

namespace HelseID.Standard.Tests.Services.ClientAssertions;

[TestFixture]
public class ClientAssertionsCreatorTests
{
    private SigningTokenCreatorMock _signingTokenCreatorMock = null!;
    private PayloadClaimsCreatorMock _payloadClaimsCreatorMock = null!;
    private ClientAssertionsCreator _assertionsCreator = null!;
    private PayloadClaimParameters _payloadClaimParameters = null!;

    [SetUp]
    public void Setup()
    {
        _signingTokenCreatorMock = new SigningTokenCreatorMock();
        _assertionsCreator = new ClientAssertionsCreator(_signingTokenCreatorMock);

        _payloadClaimsCreatorMock = new PayloadClaimsCreatorMock();
        _payloadClaimParameters = new PayloadClaimParameters
        {
            ParentOrganizationNumber = "994598759"
        };
    }

    [Test]
    public void CreateClientAssertion_uses_SigningTokenCreator()
    {
        _assertionsCreator.CreateClientAssertion(_payloadClaimsCreatorMock, _payloadClaimParameters);

        _signingTokenCreatorMock.PayloadClaimsCreator.Should().Be(_payloadClaimsCreatorMock);
        _signingTokenCreatorMock.PayloadClaimParameters.Should().Be(_payloadClaimParameters);
    }

    [Test]
    public void CreateClientAssertion_returns_client_assertion()
    {
        var result = _assertionsCreator.CreateClientAssertion(_payloadClaimsCreatorMock, _payloadClaimParameters);
        
        result.Should()
            .Contain("eyJhbGciOiJFUzM4NCIsInR5cCI6IkpXVCJ9.eyJpc3MiOiI0YTg2MzkwNS0zNjQ4LTQzY2ItYTBmZi03MTBhMTZmMjgxNjQifQ.");
    }
    
}
