using HelseId.Library.Models.Payloads;

namespace HelseId.Library.Interfaces.PayloadClaimCreators;

public interface IAuthorizationDetailsCreator
{
    PayloadClaim CreateAuthorizationDetails(PayloadClaimParameters payloadClaimParameters);
}
