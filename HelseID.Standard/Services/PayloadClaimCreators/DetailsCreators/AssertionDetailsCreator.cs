using HelseID.Standard.Interfaces.PayloadClaimCreators;
using HelseID.Standard.Models.Payloads;

namespace HelseID.Standard.Services.PayloadClaimCreators.DetailsCreators;

public class AssertionDetailsCreator : DetailsCreator, IAssertionDetailsCreator
{
    public AssertionDetailsCreator(List<IStructuredClaimsCreator> structuredClaimsCreators) : base(structuredClaimsCreators)
    {
        DetailsName = "assertion_details";
    }
    public AssertionDetailsCreator(IEnumerable<IStructuredClaimsCreator> structuredClaimsCreators) : base(structuredClaimsCreators.ToList())
    {
        DetailsName = "assertion_details";
    }
    
    public PayloadClaim CreateAssertionDetails(PayloadClaimParameters payloadClaimParameters)
    {
        return CreateDetails(payloadClaimParameters);
    }
}
