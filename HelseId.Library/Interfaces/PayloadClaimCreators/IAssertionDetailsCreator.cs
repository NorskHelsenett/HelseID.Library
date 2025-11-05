namespace HelseId.Library.Interfaces.PayloadClaimCreators;

internal interface IAssertionDetailsCreator
{
    PayloadClaim CreateAssertionDetails(PayloadClaimParameters payloadClaimParameters);
}
