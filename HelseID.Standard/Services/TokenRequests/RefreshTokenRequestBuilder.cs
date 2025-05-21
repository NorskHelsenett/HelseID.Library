using HelseID.Standard.Configuration;
using HelseID.Standard.Interfaces.Endpoints;
using HelseID.Standard.Interfaces.JwtTokens;
using HelseID.Standard.Interfaces.PayloadClaimCreators;
using HelseID.Standard.Interfaces.TokenRequests;
using HelseID.Standard.Models;
using HelseID.Standard.Models.TokenRequests;

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
