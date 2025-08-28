using HelseId.Library.ClientCredentials.Interfaces.TokenRequests;
using HelseId.Library.ClientCredentials.Models.TokenRequests;
using HelseId.Library.Interfaces.Configuration;

namespace HelseId.Library.ClientCredentials.Services.TokenRequests;

internal class ClientCredentialsTokenRequestBuilder : TokenRequestBuilder, IClientCredentialsTokenRequestBuilder
{
    private readonly IHelseIdConfigurationGetter _configurationGetter;

    public ClientCredentialsTokenRequestBuilder(
        ISigningTokenCreator signingTokenCreator,
        IDPoPProofCreator dPoPProofCreator,
        IHelseIdEndpointsDiscoverer helseIdEndpointsDiscoverer,
        IHelseIdConfigurationGetter helseIdConfigurationGetter) : base(signingTokenCreator, dPoPProofCreator, helseIdEndpointsDiscoverer)
    {
        _configurationGetter = helseIdConfigurationGetter;
    }

    public async Task<HelseIdTokenRequest> CreateTokenRequest(
        IPayloadClaimsCreator payloadClaimsCreator,
        ClientCredentialsTokenRequestParameters tokenRequestParameters,
        string? dPoPNonce = null)
    {
        var tokenEndpoint = await FindTokenEndpoint();
        var clientAssertion = await CreateClientAssertion(payloadClaimsCreator, tokenRequestParameters.PayloadClaimParameters);
        var dpopProof = await CreateDPoPProof(tokenEndpoint, dPoPNonce);

        var helseIdConfiguration = await _configurationGetter.GetConfiguration();
        
        return new HelseIdTokenRequest
        {
            Address = tokenEndpoint,
            ClientAssertion = clientAssertion,
            ClientId = helseIdConfiguration.ClientId,
            Scope = helseIdConfiguration.Scope,
            DPoPProofToken = dpopProof,
            GrantType = GrantTypes.ClientCredentials
        };
    }
}
