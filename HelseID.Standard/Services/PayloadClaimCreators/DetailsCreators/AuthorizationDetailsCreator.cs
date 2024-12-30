using HelseID.Standard.Interfaces.PayloadClaimCreators;
using HelseID.Standard.Models.Payloads;

namespace HelseID.Standard.Services.PayloadClaimCreators.DetailsCreators;

public class AuthorizationDetailsCreator : DetailsCreator, IAuthorizationDetailsCreator
{
    public AuthorizationDetailsCreator(List<IStructuredClaimsCreator> structuredClaimsCreators) : base(structuredClaimsCreators)
    {
        DetailsName = "authorization_details";
    }
    
    public PayloadClaim CreateAuthorizationDetails(PayloadClaimParameters payloadClaimParameters)
    {
        return CreateDetails(payloadClaimParameters);
    }
}
