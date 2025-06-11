using HelseId.Standard.Interfaces.PayloadClaimCreators;
using HelseId.Standard.Models.Payloads;
using Microsoft.IdentityModel.JsonWebTokens;

namespace HelseId.Standard.Tests.Mocks;

public class StructuredClaimsCreatorMock : IStructuredClaimsCreator
{
    public PayloadClaimParameters PayloadClaimParameters { get; set; } = null!;
    
    public bool SetNullValue { get; set; }
    
    public Dictionary<string, object> StructuredClaims { get; set; } = new()
    {
        { JwtRegisteredClaimNames.Jti, "3afb3f5f-835e-49c0-9736-d7ba5502a83a" },
    };
    
    public bool CreateStructuredClaims(PayloadClaimParameters payloadClaimParameters, out Dictionary<string, object> structuredClaims)
    {
        PayloadClaimParameters = payloadClaimParameters;
        if (SetNullValue == false)
        {
            structuredClaims = StructuredClaims;
            return true;
        }

        structuredClaims = new Dictionary<string, object>();
        return false;
    }
}
