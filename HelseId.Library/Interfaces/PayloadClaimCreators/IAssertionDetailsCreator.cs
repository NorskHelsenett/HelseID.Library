using HelseId.Library.Models.Payloads;

namespace HelseId.Library.Interfaces.PayloadClaimCreators;

public interface IAssertionDetailsCreator
{
    PayloadClaim CreateAssertionDetails(PayloadClaimParameters payloadClaimParameters);
}
