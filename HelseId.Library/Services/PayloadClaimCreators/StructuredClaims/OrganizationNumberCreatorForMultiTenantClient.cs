using HelseId.Library.Exceptions;
using HelseId.Library.Interfaces.PayloadClaimCreators;
using HelseId.Library.Models.Payloads;

namespace HelseId.Library.Services.PayloadClaimCreators.StructuredClaims;

public class OrganizationNumberCreatorForMultiTenantClient : IStructuredClaimsCreator {

    public bool CreateStructuredClaims(PayloadClaimParameters payloadClaimParameters, out Dictionary<string, object> structuredClaims)
    {
        if (payloadClaimParameters.UseOrganizationNumbers == false)
        {
            structuredClaims = new Dictionary<string, object>();
            return false;
        }
        
        if (string.IsNullOrEmpty(payloadClaimParameters.ParentOrganizationNumber))
        {
            throw new MissingParentOrganizationNumberException();
        }
        
        // When the client is of the multi-tenancy type, it will require a parent organization number claim.
        // In this case, HelseID will require an authorization details claim with the following structure:
        //
        //  "authorization_details":{
        //      "type":"helseid_authorization",
        //      "practitioner_role":{
        //          "organization":{
        //              "identifier": {
        //                  "system":"urn:oid:1.0.6523",
        //                  "type":"ENH",
        //                  "value":"NO:ORGNR:[parent orgnumber]:[child orgnumber]"
        //              }
        //          }
        //      }
        //  }
        //
        // We use anonymous types to insert the structured claim into the payload:

        var orgNumberDetails = new Dictionary<string, string>
        {
            { "system", "urn:oid:1.0.6523" },
            { "type", "ENH" },
            { "value", $"NO:ORGNR:{GetOrganizationNumberValue(payloadClaimParameters)}" }
        };

        var identifier = new Dictionary<string, object>
        {
            { "identifier", orgNumberDetails }
        };

        var organization = new Dictionary<string, object>
        {
            { "organization", identifier }
        };

        structuredClaims = new Dictionary<string, object>
        {
            { "type", "helseid_authorization" },
            { "practitioner_role", organization }
        };

        return true;
    }

    private static string GetOrganizationNumberValue(PayloadClaimParameters payloadClaimParameters)
    {
        var organizationNumberValue = payloadClaimParameters.ParentOrganizationNumber;
        if (!string.IsNullOrEmpty(payloadClaimParameters.ChildOrganizationNumber))
        {
            organizationNumberValue += $":{payloadClaimParameters.ChildOrganizationNumber}";
        }
        else
        {
            organizationNumberValue += ":";
        }
        return organizationNumberValue;
    }
}
