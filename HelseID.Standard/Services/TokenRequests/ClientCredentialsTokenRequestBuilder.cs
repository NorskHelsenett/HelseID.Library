using HelseId.Standard.Configuration;
using HelseId.Standard.Interfaces.Endpoints;
using HelseId.Standard.Interfaces.JwtTokens;
using HelseId.Standard.Interfaces.PayloadClaimCreators;
using HelseId.Standard.Interfaces.TokenRequests;
using HelseId.Standard.Models;
using HelseId.Standard.Models.Constants;
using HelseId.Standard.Models.TokenRequests;

namespace HelseId.Standard.Services.TokenRequests;

public class ClientCredentialsTokenRequestBuilder : TokenRequestBuilder, IClientCredentialsTokenRequestBuilder
{
    private readonly HelseIdConfiguration _helseIdConfiguration;

    public ClientCredentialsTokenRequestBuilder(
        ISigningTokenCreator signingTokenCreator,
        IDPoPProofCreator dPoPProofCreator,
        IHelseIdEndpointsDiscoverer helseIdEndpointsDiscoverer,
        HelseIdConfiguration helseIdConfiguration) : base(signingTokenCreator, dPoPProofCreator, helseIdEndpointsDiscoverer)
    {
        _helseIdConfiguration = helseIdConfiguration;
    }

    public async Task<HelseIdTokenRequest> CreateTokenRequest(
        IPayloadClaimsCreator payloadClaimsCreator,
        ClientCredentialsTokenRequestParameters tokenRequestParameters,
        string? dPoPNonce = null)
    {
        var tokenEndpoint = await FindTokenEndpoint();
        var clientAssertion = CreateClientAssertion(payloadClaimsCreator, tokenRequestParameters.PayloadClaimParameters);
        var dpopProof = CreateDPoPProof(tokenEndpoint, dPoPNonce);
        
        return new HelseIdTokenRequest
        {
            Address = tokenEndpoint,
            ClientAssertion = clientAssertion,
            ClientId = _helseIdConfiguration.ClientId,
            Scope = _helseIdConfiguration.Scope,
            DPoPProofToken = dpopProof,
            GrantType = GrantTypes.ClientCredentials
        };
    }
}
