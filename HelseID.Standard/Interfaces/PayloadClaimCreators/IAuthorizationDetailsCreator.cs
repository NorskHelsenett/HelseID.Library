using HelseID.Standard.Models.Payloads;

namespace HelseID.Standard.Interfaces.PayloadClaimCreators;

public interface IAuthorizationDetailsCreator
{
    PayloadClaim CreateAuthorizationDetails(PayloadClaimParameters payloadClaimParameters);
}
