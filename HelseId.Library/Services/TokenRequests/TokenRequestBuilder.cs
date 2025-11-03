namespace HelseId.Library.Services.TokenRequests;

public abstract class TokenRequestBuilder
{
    private readonly ISigningTokenCreator _signingTokenCreator;
    private readonly IDPoPProofCreator _dPoPProofCreator;
    private readonly IHelseIdEndpointsDiscoverer _helseIdEndpointsDiscoverer;

    internal TokenRequestBuilder(
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
    
    protected Task<string> CreateClientAssertion(IPayloadClaimsCreator payloadClaimsCreator, PayloadClaimParameters payloadClaimParameters)
    {
        // HelseID requires a client assertion in order to recognize this client
        return _signingTokenCreator.CreateSigningToken(payloadClaimsCreator, payloadClaimParameters);
    }

    protected Task<string> CreateDPoPProof(string tokenEndpoint, string? dPoPNonce = null)
    {
        return _dPoPProofCreator.CreateDPoPProofForTokenRequest(HttpMethod.Post, tokenEndpoint, dPoPNonce: dPoPNonce);
    }
}
