using FluentAssertions;
using HelseID.Standard.Exceptions;
using HelseID.Standard.Models.Payloads;
using HelseID.Standard.Services.PayloadClaimCreators.StructuredClaims;

namespace HelseID.Standard.Tests.Services.PayloadClaimCreators.StructuredClaims;

[TestFixture]
public class OrganizationNumberCreatorForMultiTenantClientTests
{
    private const string ParentOrganizationNumber = "111222333";
    private const string ChildOrganizationNumber = "222333444";
    
    private PayloadClaimParameters _payloadClaimParameters = null!;
    private OrganizationNumberCreatorForMultiTenantClient _organizationNumberCreatorForMultiTenantClient = null!;

    [SetUp]
    public void Setup()
    {
        _payloadClaimParameters = new PayloadClaimParameters
        {
            UseOrganizationNumbers = true,
            ParentOrganizationNumber = ParentOrganizationNumber,
            ChildOrganizationNumber = ChildOrganizationNumber,
        };

        _organizationNumberCreatorForMultiTenantClient = new OrganizationNumberCreatorForMultiTenantClient();
    }
    
    [Test]
    public void CreateStructuredClaims_sets_structured_claims()
    {
        var value = _organizationNumberCreatorForMultiTenantClient.CreateStructuredClaims(_payloadClaimParameters, out var structuredClaims);
        value.Should().BeTrue();
        
        structuredClaims.Should().NotBeNull();
        
        structuredClaims["type"].Should().NotBeNull();
        structuredClaims["type"].Should().Be("helseid_authorization");
        
        structuredClaims["practitioner_role"].Should().NotBeNull();
        var practitionerRole = structuredClaims["practitioner_role"] as Dictionary<string, object>;
        practitionerRole.Should().NotBeNull();
        
        practitionerRole!["organization"].Should().NotBeNull();
        var organization = practitionerRole["organization"] as Dictionary<string, object>;
        organization.Should().NotBeNull();
        
        organization!["identifier"].Should().NotBeNull();
        var identifier = organization["identifier"] as Dictionary<string, string>;
        identifier.Should().NotBeNull();
        
        identifier!["system"].Should().NotBeNull();
        identifier!["system"].Should().Be("urn:oid:1.0.6523");
        
        identifier!["type"].Should().NotBeNull();
        identifier!["type"].Should().Be("ENH");

        identifier!["value"].Should().NotBeNull();
        identifier!["value"].Should().Be("NO:ORGNR:111222333:222333444");
    }
    
    [Test]
    public void CreateStructuredClaims_sets_structured_claims_without_child_organization_number()
    {
        _payloadClaimParameters.ChildOrganizationNumber = "";
        
        var value = _organizationNumberCreatorForMultiTenantClient.CreateStructuredClaims(_payloadClaimParameters, out var structuredClaims);
        value.Should().BeTrue();
        
        var practitionerRole = structuredClaims["practitioner_role"] as Dictionary<string, object>;
        var organization = practitionerRole!["organization"] as Dictionary<string, object>;
        var identifier = organization!["identifier"] as Dictionary<string, string>;

        identifier!["value"].Should().NotBeNull();
        identifier!["value"].Should().Be("NO:ORGNR:111222333:");
    }

    [Test]
    public void CreateStructuredClaims_sets_false_value_when_UseOrganizationNumbers_is_false()
    {
        _payloadClaimParameters.UseOrganizationNumbers = false;
        
        var value = _organizationNumberCreatorForMultiTenantClient.CreateStructuredClaims(_payloadClaimParameters, out var structuredClaims);
        value.Should().BeFalse();
        structuredClaims.Should().BeEmpty();
    }
    
    [Test]
    public void CreateStructuredClaims_throws_if_parent_organization_number_is_null_or_empty()
    {
        _payloadClaimParameters.ParentOrganizationNumber = "";
        
        Action createStructuredClaimns = () => 
            _organizationNumberCreatorForMultiTenantClient.CreateStructuredClaims(_payloadClaimParameters, out var structuredClaims);

        createStructuredClaimns.Should().Throw<MissingParentOrganizationNumberException>();
    }
}
