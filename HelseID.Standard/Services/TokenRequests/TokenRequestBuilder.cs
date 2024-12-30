using HelseID.Standard.Interfaces.ClientAssertions;
using HelseID.Standard.Interfaces.Endpoints;
using HelseID.Standard.Interfaces.JwtTokens;
using HelseID.Standard.Interfaces.PayloadClaimCreators;
using HelseID.Standard.Models.Payloads;
using IdentityModel.Client;

namespace HelseID.Standard.Services.TokenRequests;

public abstract class TokenRequestBuilder
{
    private readonly IClientAssertionsCreator _clientAssertionsCreator;
    private readonly IDPoPProofCreator _dPoPProofCreator;
    private readonly IHelseIdEndpointsDiscoverer _helseIdEndpointsDiscoverer;

    protected TokenRequestBuilder(
        IClientAssertionsCreator clientAssertionsCreator,
        IDPoPProofCreator dPoPProofCreator,
        IHelseIdEndpointsDiscoverer helseIdEndpointsDiscoverer)
    {
        _clientAssertionsCreator = clientAssertionsCreator;
        _dPoPProofCreator = dPoPProofCreator;
        _helseIdEndpointsDiscoverer = helseIdEndpointsDiscoverer;
    }
    
    protected async Task<string> FindTokenEndpoint()
    {
        return await _helseIdEndpointsDiscoverer.GetTokenEndpointFromHelseId();
    }
    
    protected ClientAssertion CreateClientAssertion(IPayloadClaimsCreator payloadClaimsCreator, PayloadClaimParameters payloadClaimParameters)
    {
        // HelseID requires a client assertion in order to recognize this client
        return _clientAssertionsCreator.CreateClientAssertion(payloadClaimsCreator, payloadClaimParameters);
    }

    protected string CreateDPoPProof(string tokenEndpoint, string? dPoPNonce = null)
    {
        return _dPoPProofCreator.CreateDPoPProof(tokenEndpoint, "POST", dPoPNonce: dPoPNonce);
    }
}
