using HelseID.Standard.Models.Payloads;

namespace HelseID.Standard.Interfaces.PayloadClaimCreators;

public interface IAssertionDetailsCreator
{
    PayloadClaim CreateAssertionDetails(PayloadClaimParameters payloadClaimParameters);
}
