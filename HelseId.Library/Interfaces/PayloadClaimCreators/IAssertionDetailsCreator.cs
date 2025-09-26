namespace HelseId.Library.Interfaces.PayloadClaimCreators;

public interface IAssertionDetailsCreator
{
    PayloadClaim CreateAssertionDetails(PayloadClaimParameters payloadClaimParameters);
}
