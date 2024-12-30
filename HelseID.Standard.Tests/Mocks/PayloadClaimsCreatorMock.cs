using HelseID.Standard.Configuration;
using HelseID.Standard.Interfaces.PayloadClaimCreators;
using HelseID.Standard.Models.Payloads;
using Microsoft.IdentityModel.JsonWebTokens;

namespace HelseID.Standard.Tests.Mocks;

public class PayloadClaimsCreatorMock : IPayloadClaimsCreator
{
    private const string ClientId = "d0612b27-171d-4e61-87f1-9b574c02d195";

    public HelseIdConfiguration HelseIdConfiguration { get; set; } = null!;
    public PayloadClaimParameters PayloadClaimParameters { get; set; } = null!;
    
    public Dictionary<string, object> Claims { get; set; } = new()
    {
        { JwtRegisteredClaimNames.Iss, ClientId },
        {  JwtRegisteredClaimNames.Sub, ClientId },
        { JwtRegisteredClaimNames.Jti, "3afb3f5f-835e-49c0-9736-d7ba5502a83a" },
    };
    
    public Dictionary<string, object> CreatePayloadClaims(HelseIdConfiguration configuration, PayloadClaimParameters payloadClaimParameters)
    {
        HelseIdConfiguration = configuration;
        PayloadClaimParameters = payloadClaimParameters;
        
        return Claims;
    }
}
