using HelseId.Standard.Interfaces.PayloadClaimCreators;
using HelseId.Standard.Models.Payloads;

namespace HelseId.Standard.Services.PayloadClaimCreators.DetailsCreators;

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
