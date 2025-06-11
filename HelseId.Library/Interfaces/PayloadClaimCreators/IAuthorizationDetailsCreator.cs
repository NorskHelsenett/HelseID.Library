namespace HelseId.Library.Interfaces.PayloadClaimCreators;

public interface IAuthorizationDetailsCreator
{
    PayloadClaim CreateAuthorizationDetails(PayloadClaimParameters payloadClaimParameters);
}
