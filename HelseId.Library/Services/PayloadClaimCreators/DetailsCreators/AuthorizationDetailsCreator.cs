using HelseId.Library.Interfaces.PayloadClaimCreators;
using HelseId.Library.Models.Payloads;

namespace HelseId.Library.Services.PayloadClaimCreators.DetailsCreators;

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
