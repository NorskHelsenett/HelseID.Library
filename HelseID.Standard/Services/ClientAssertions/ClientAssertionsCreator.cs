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

    public string CreateClientAssertion(IPayloadClaimsCreator payloadClaimsCreator, PayloadClaimParameters payloadClaimParameters)
    {
        return _signingTokenCreator.CreateSigningToken(payloadClaimsCreator, payloadClaimParameters);
    }
}
