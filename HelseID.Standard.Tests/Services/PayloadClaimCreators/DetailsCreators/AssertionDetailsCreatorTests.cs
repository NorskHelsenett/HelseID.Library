using FluentAssertions;
using HelseID.Standard.Exceptions;
using HelseID.Standard.Interfaces.PayloadClaimCreators;
using HelseID.Standard.Models.Payloads;
using HelseID.Standard.Services.PayloadClaimCreators.DetailsCreators;
using HelseID.Standard.Tests.Mocks;

namespace HelseID.Standard.Tests.Services.PayloadClaimCreators.DetailsCreators;

[TestFixture]
public class AssertionDetailsCreatorTests
{
    private PayloadClaimParameters _parameters = null!;
    
    private AssertionDetailsCreator _assertionDetailsCreator = null!;
    private AuthorizationDetailsCreator _authorizationDetailsCreator = null!;
    
    private Dictionary<string, object> _sfmClaims = null!;
    private Dictionary<string, object> _organizationClaims = null!;
    private StructuredClaimsCreatorMock _structuredClaimsCreatorMockForSfm = null!;
    private StructuredClaimsCreatorMock _structuredClaimsCreatorMockForOrganizations = null!;

    private const string SfmJournalId = "8079c363-59b0-4a06-8385-d547a75c4c32";
    
    private List<IStructuredClaimsCreator> _structuredClaimsClaimsCreators = null!;
    
    [SetUp]
    public void Setup()
    {
        _parameters = new PayloadClaimParameters()
        {
            ParentOrganizationNumber = "333444888",
            ChildOrganizationNumber = "888999777",
            SfmJournalId = SfmJournalId,
        };

        _sfmClaims = new Dictionary<string, object>()
        {
            { "type", "nhn:sfm:journal-id" },
            { "value", "something" },
        };

        _organizationClaims = new Dictionary<string, object>()
        {
            { "type", "helseid_authorization" },
            { "practitioner_role", "organization" }
        };

        _structuredClaimsCreatorMockForSfm = new StructuredClaimsCreatorMock
        {
            StructuredClaims = _sfmClaims,
        };

        _structuredClaimsCreatorMockForOrganizations = new StructuredClaimsCreatorMock
        {
            StructuredClaims = _organizationClaims,
        };
        
        _structuredClaimsClaimsCreators =
        [
            _structuredClaimsCreatorMockForSfm,
        ];
        
        _assertionDetailsCreator = new AssertionDetailsCreator(_structuredClaimsClaimsCreators);
        _authorizationDetailsCreator = new AuthorizationDetailsCreator(_structuredClaimsClaimsCreators);
    }

    [Test]
    public void CreateAssertionDetails_sets_payload_claims()
    {
        _assertionDetailsCreator.CreateAssertionDetails(_parameters);

        _structuredClaimsCreatorMockForSfm.PayloadClaimParameters.Should().NotBeNull();
        _structuredClaimsCreatorMockForSfm.PayloadClaimParameters.SfmJournalId.Should().Be(SfmJournalId);
    }

    [Test]
    public void CreateAssertionDetails_throws_exeption_if_value_is_null()
    {
         _structuredClaimsCreatorMockForSfm.SetNullValue = true;
         
         Action createAssertionDetails = () => _assertionDetailsCreator.CreateAssertionDetails(_parameters);

         createAssertionDetails.Should().Throw<MissingValueFromStructuredClaimsCreatorException>();
    }
    
    [Test]
    public void CreateAssertionDetails_should_return_assertion_details()
    {
        var payloadClaim = _assertionDetailsCreator.CreateAssertionDetails(_parameters);

        payloadClaim.Should().NotBeNull();
        payloadClaim.Name.Should().Be("assertion_details");
        payloadClaim.Value.Should().Be(_sfmClaims);
    }
    
    [Test]
    public void CreateStructuredClaims_gets_structured_claims()
    {
        _structuredClaimsClaimsCreators =
        [
            _structuredClaimsCreatorMockForSfm,
            _structuredClaimsCreatorMockForOrganizations,
        ];
        
        _assertionDetailsCreator = new AssertionDetailsCreator(_structuredClaimsClaimsCreators);
        
        var payloadClaim = _assertionDetailsCreator.CreateAssertionDetails(_parameters);
        
        payloadClaim.Should().NotBeNull();
        payloadClaim.Name.Should().Be("assertion_details");

        List<object> value = new List<object>()
        {
            _sfmClaims,
            _organizationClaims
        };
        
        payloadClaim.Value.Should().BeEquivalentTo(value);

        /*
        var securityTokenDescriptor = new SecurityTokenDescriptor
        {
            Claims = new Dictionary<string, object>(){ {structuredClaim.Name, structuredClaim.Value} },
            SigningCredentials = GetClientAssertionSigningCredentials(),
        };
        
        var tokenHandler = new JsonWebTokenHandler
        {
            SetDefaultTimesOnTokenCreation = false
        };

        var token = tokenHandler.CreateToken(securityTokenDescriptor);
        */
    }
    
    [Test]
    public void CreateAuthorizationDetails_should_return_authorization_details()
    {
        var payloadClaim = _authorizationDetailsCreator.CreateAuthorizationDetails(_parameters);

        payloadClaim.Should().NotBeNull();
        payloadClaim.Name.Should().Be("authorization_details");
        payloadClaim.Value.Should().Be(_sfmClaims);
    }
    
    /*
    private static SigningCredentials GetClientAssertionSigningCredentials()
    {
        var securityKey = new JsonWebKey(CommonRsaKey.JwkValue);
        return new SigningCredentials(securityKey, CommonRsaKey.Algorithm);
    }
    */
}
