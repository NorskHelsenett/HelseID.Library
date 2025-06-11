using HelseId.Standard.Models.Payloads;

namespace HelseId.Standard.Interfaces.PayloadClaimCreators;

public interface IAuthorizationDetailsCreator
{
    PayloadClaim CreateAuthorizationDetails(PayloadClaimParameters payloadClaimParameters);
}
