using HelseId.Standard.Models.Payloads;

namespace HelseId.Standard.Interfaces.PayloadClaimCreators;

public interface IAssertionDetailsCreator
{
    PayloadClaim CreateAssertionDetails(PayloadClaimParameters payloadClaimParameters);
}
