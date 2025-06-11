using HelseId.Standard.Configuration;
using HelseId.Standard.Interfaces.Endpoints;
using HelseId.Standard.Interfaces.JwtTokens;
using HelseId.Standard.Interfaces.PayloadClaimCreators;
using HelseId.Standard.Interfaces.TokenRequests;
using HelseId.Standard.Models;
using HelseId.Standard.Models.TokenRequests;

namespace HelseId.Standard.Services.TokenRequests;

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

    public async Task<HelseIdTokenRequest> CreateTokenRequest(
        IPayloadClaimsCreator payloadClaimsCreator,
        RefreshTokenRequestParameters tokenRequestParameters,
        string? dPoPNonce = null)
    {
        var tokenEndpoint = await FindTokenEndpoint();
        var clientAssertion = CreateClientAssertion(payloadClaimsCreator, tokenRequestParameters.PayloadClaimParameters);
        var dpopProof = CreateDPoPProof(tokenEndpoint, dPoPNonce);

        var request = new HelseIdTokenRequest
        {
            Address = tokenEndpoint,
            ClientAssertion = clientAssertion,
            ClientId = _helseIdConfiguration.ClientId,
            GrantType = "refresh_token",
            //RefreshToken = tokenRequestParameters.RefreshToken,
            DPoPProofToken = dpopProof,
            Scope = "",
        };
        if (tokenRequestParameters.HasResourceIndicator)
        {
            //request.Resource = tokenRequestParameters.Resource;
        }
        return request;
    }
}
