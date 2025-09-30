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
        var scope = tokenRequestParameters.Scope;
        if (string.IsNullOrEmpty(scope))
        {
            scope = helseIdConfiguration.Scope;
        }
        
        return new HelseIdTokenRequest
        {
            Address = tokenEndpoint,
            ClientAssertion = clientAssertion,
            ClientId = helseIdConfiguration.ClientId,
            Scope = scope,
            DPoPProofToken = dpopProof,
            GrantType = GrantTypes.ClientCredentials
        };
    }
}
