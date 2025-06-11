using HelseId.Standard.Interfaces.Endpoints;
using HelseId.Standard.Interfaces.JwtTokens;
using HelseId.Standard.Interfaces.PayloadClaimCreators;
using HelseId.Standard.Models.Payloads;

namespace HelseId.Standard.Services.TokenRequests;

public abstract class TokenRequestBuilder
{
    private readonly ISigningTokenCreator _signingTokenCreator;
    private readonly IDPoPProofCreator _dPoPProofCreator;
    private readonly IHelseIdEndpointsDiscoverer _helseIdEndpointsDiscoverer;

    protected TokenRequestBuilder(
        ISigningTokenCreator signingTokenCreator,
        IDPoPProofCreator dPoPProofCreator,
        IHelseIdEndpointsDiscoverer helseIdEndpointsDiscoverer)
    {
        _signingTokenCreator = signingTokenCreator;
        _dPoPProofCreator = dPoPProofCreator;
        _helseIdEndpointsDiscoverer = helseIdEndpointsDiscoverer;
    }
    
    protected async Task<string> FindTokenEndpoint()
    {
        return await _helseIdEndpointsDiscoverer.GetTokenEndpointFromHelseId();
    }
    
    protected string CreateClientAssertion(IPayloadClaimsCreator payloadClaimsCreator, PayloadClaimParameters payloadClaimParameters)
    {
        // HelseID requires a client assertion in order to recognize this client
        return _signingTokenCreator.CreateSigningToken(payloadClaimsCreator, payloadClaimParameters);
    }

    protected string CreateDPoPProof(string tokenEndpoint, string? dPoPNonce = null)
    {
        return _dPoPProofCreator.CreateDPoPProof(tokenEndpoint, "POST", dPoPNonce: dPoPNonce);
    }
}
