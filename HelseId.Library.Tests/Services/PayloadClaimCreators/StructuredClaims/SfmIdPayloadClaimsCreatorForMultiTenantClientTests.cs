namespace HelseId.Standard.Tests.Services.PayloadClaimCreators.StructuredClaims;

[TestFixture]
public class SfmIdPayloadClaimsCreatorForMultiTenantClientTests: ConfigurationTests
{
    private const string SfmJournalId = "231231234-34213412-432423-4233";
    
    private PayloadClaimParameters _payloadClaimParameters = null!;
    private SfmIdPayloadClaimsCreatorForMultiTenantClient _clientAssertionPayloadClaimsCreator = null!;
    
    [SetUp]
    public void Setup()
    {
        _payloadClaimParameters = new PayloadClaimParameters
        {
            UseSfmId = true,
            SfmJournalId = SfmJournalId,
        };

        _clientAssertionPayloadClaimsCreator = new SfmIdPayloadClaimsCreatorForMultiTenantClient();
    }

    [Test]
    public void CreateStructuredClaims_sets_structured_claims()
    {
        var booleanValue = _clientAssertionPayloadClaimsCreator.CreateStructuredClaims(_payloadClaimParameters, out var structuredClaims);
        booleanValue.Should().BeTrue();

        structuredClaims.Should().NotBeNull();
        
        structuredClaims["type"].Should().NotBeNull();
        structuredClaims["type"].Should().Be("nhn:sfm:journal-id");
        
        structuredClaims["value"].Should().NotBeNull();
        var value = structuredClaims["value"] as Dictionary<string, string>;
        value.Should().NotBeNull();
        value!["journal_id"].Should().NotBeNull();
        value!["journal_id"].Should().Be(SfmJournalId);
    }
    
    [Test]
    public void CreateStructuredClaims_sets_false_value_when_UseSfmId_is_false()
    {
        _payloadClaimParameters.UseSfmId = false;
        
        var value = _clientAssertionPayloadClaimsCreator.CreateStructuredClaims(_payloadClaimParameters, out var structuredClaims);
        value.Should().BeFalse();
        structuredClaims.Should().BeEmpty();
    }
    
    [Test]
    public void CreateStructuredClaims_throws_if_sfm_jorunal_id_is_null_or_empty()
    {
        _payloadClaimParameters.SfmJournalId = "";
        
        Action createStructuredClaimns = () => 
            _clientAssertionPayloadClaimsCreator.CreateStructuredClaims(_payloadClaimParameters, out var structuredClaims);

        createStructuredClaimns.Should().Throw<MissingSfmJournalIdException>();
    }
}
