using HelseID.Standard.Configuration;
using HelseID.Standard.Interfaces.ClientAssertions;
using HelseID.Standard.Interfaces.Endpoints;
using HelseID.Standard.Interfaces.JwtTokens;
using HelseID.Standard.Interfaces.PayloadClaimCreators;
using HelseID.Standard.Interfaces.TokenRequests;
using HelseID.Standard.Models.TokenRequests;
using IdentityModel;
using IdentityModel.Client;

namespace HelseID.Standard.Services.TokenRequests;

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

    public async Task<ClientCredentialsTokenRequest> CreateTokenRequest(
        IPayloadClaimsCreator payloadClaimsCreator,
        ClientCredentialsTokenRequestParameters tokenRequestParameters,
        string? dPoPNonce = null)
    {
        var tokenEndpoint = await FindTokenEndpoint();
        var clientAssertion = CreateClientAssertion(payloadClaimsCreator, tokenRequestParameters.PayloadClaimParameters);
        var dpopProof = CreateDPoPProof(tokenEndpoint, dPoPNonce);
        
        // This class comes from the IdentityModel library and abstracts a request for a token using the client credential grant
        return new ClientCredentialsTokenRequest
        {
            Address = tokenEndpoint,
            ClientAssertion = new ClientAssertion
            {
                Type = "urn:ietf:params:oauth:client-assertion-type:jwt-bearer",
                Value = clientAssertion
            },
            ClientId = _helseIdConfiguration.ClientId,
            Scope = _helseIdConfiguration.Scope,
            GrantType = OidcConstants.GrantTypes.ClientCredentials,
            ClientCredentialStyle = ClientCredentialStyle.PostBody,
            DPoPProofToken = dpopProof,
        };
    }
}
