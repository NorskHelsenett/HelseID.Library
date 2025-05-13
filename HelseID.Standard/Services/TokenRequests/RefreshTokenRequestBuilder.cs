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

public class RefreshTokenRequestBuilder : TokenRequestBuilder, IRefreshTokenRequestBuilder
{
    private readonly HelseIdConfiguration _helseIdConfiguration;

    public RefreshTokenRequestBuilder(
        ISigningTokenCreator signingTokenCreator,
        IDPoPProofCreator dPoPProofCreator,
        IHelseIdEndpointsDiscoverer helseIdEndpointsDiscoverer,
        HelseIdConfiguration helseIdConfiguration) : base(signingTokenCreator, dPoPProofCreator, helseIdEndpointsDiscoverer)
    {
        _helseIdConfiguration = helseIdConfiguration;
    }

    public async Task<RefreshTokenRequest> CreateTokenRequest(
        IPayloadClaimsCreator payloadClaimsCreator,
        RefreshTokenRequestParameters tokenRequestParameters,
        string? dPoPNonce = null)
    {
        var tokenEndpoint = await FindTokenEndpoint();
        var clientAssertion = CreateClientAssertion(payloadClaimsCreator, tokenRequestParameters.PayloadClaimParameters);
        var dpopProof = CreateDPoPProof(tokenEndpoint, dPoPNonce);

        var request = new RefreshTokenRequest
        {
            Address = tokenEndpoint,
            ClientAssertion = new ClientAssertion
            {
                Type = "urn:ietf:params:oauth:client-assertion-type:jwt-bearer",
                Value = clientAssertion
            },
            ClientId = _helseIdConfiguration.ClientId,
            GrantType = OidcConstants.GrantTypes.RefreshToken,
            ClientCredentialStyle = ClientCredentialStyle.PostBody,
            RefreshToken = tokenRequestParameters.RefreshToken,
            DPoPProofToken = dpopProof,
        };
        if (tokenRequestParameters.HasResourceIndicator)
        {
            request.Resource = tokenRequestParameters.Resource;
        }
        return request;
    }
}
