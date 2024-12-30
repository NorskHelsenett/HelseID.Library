using HelseID.Standard.Interfaces.ClientAssertions;
using HelseID.Standard.Interfaces.JwtTokens;
using HelseID.Standard.Interfaces.PayloadClaimCreators;
using HelseID.Standard.Models.Payloads;
using IdentityModel;
using IdentityModel.Client;

namespace HelseID.Standard.Services.ClientAssertions;

/// <summary>
/// This class creates a client assertion. This assertion includes a JWT token that establishes the identity of this
/// client application for the STS. See https://www.rfc-editor.org/rfc/rfc7523#section-2.2 for more information
/// about this mechanism.
/// </summary>
public class ClientAssertionsCreator : IClientAssertionsCreator
{
    private readonly ISigningTokenCreator _signingTokenCreator;
    
    public ClientAssertionsCreator(ISigningTokenCreator signingTokenCreator)
    {
        _signingTokenCreator = signingTokenCreator;
    }

    public ClientAssertion CreateClientAssertion(IPayloadClaimsCreator payloadClaimsCreator, PayloadClaimParameters payloadClaimParameters)
    {
        var token = _signingTokenCreator.CreateSigningToken(payloadClaimsCreator, payloadClaimParameters);
        
        // TODO: create logging for this
        // Console.WriteLine("This is the security token that is sent to HelseID as part of the client assertion:");
        // Console.WriteLine(token);
        // Console.WriteLine("-----");
        
        return new ClientAssertion
        {
            Value = token,
            Type = OidcConstants.ClientAssertionTypes.JwtBearer
        };
    }
}
