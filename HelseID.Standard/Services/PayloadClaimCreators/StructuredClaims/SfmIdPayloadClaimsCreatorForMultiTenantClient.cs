using HelseID.Standard.Exceptions;
using HelseID.Standard.Interfaces.PayloadClaimCreators;
using HelseID.Standard.Models.Payloads;

namespace HelseID.Standard.Services.PayloadClaimCreators.StructuredClaims;

public class SfmIdPayloadClaimsCreatorForMultiTenantClient : IStructuredClaimsCreator 
{
    public bool CreateStructuredClaims(PayloadClaimParameters payloadClaimParameters, out Dictionary<string, object> structuredClaims)
    {
        if (payloadClaimParameters.UseSfmId == false)
        {
            structuredClaims = new Dictionary<string, object>();
            return false;
        }
        
        if (string.IsNullOrEmpty(payloadClaimParameters.SfmJournalId))
        {
            throw new MissingSfmJournalIdException();
        }
        
        var journalId = new Dictionary<string, string>
        {
            { "journal_id", payloadClaimParameters.SfmJournalId },
        };
        
        structuredClaims = new Dictionary<string, object>
        {
            { "type", "nhn:sfm:journal-id" },
            { "value", journalId }
        };

        return true;
    }
}
