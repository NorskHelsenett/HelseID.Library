namespace HelseId.Library.Interfaces.PayloadClaimCreators;

internal interface IAuthorizationDetailsCreator
{
    PayloadClaim CreateAuthorizationDetails(PayloadClaimParameters payloadClaimParameters);
}
