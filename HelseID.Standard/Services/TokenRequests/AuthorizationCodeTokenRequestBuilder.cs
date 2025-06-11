using HelseId.Standard.Configuration;
using HelseId.Standard.Interfaces.Endpoints;
using HelseId.Standard.Interfaces.JwtTokens;
using HelseId.Standard.Interfaces.PayloadClaimCreators;
using HelseId.Standard.Interfaces.TokenRequests;
using HelseId.Standard.Models;
using HelseId.Standard.Models.TokenRequests;

namespace HelseId.Standard.Services.TokenRequests;

public class AuthorizationCodeTokenRequestBuilder : TokenRequestBuilder, IAuthorizationCodeTokenRequestBuilder
{
    private readonly HelseIdConfiguration _helseIdConfiguration;

    public AuthorizationCodeTokenRequestBuilder(
        ISigningTokenCreator signingTokenCreator,
        IDPoPProofCreator dPoPProofCreator,
        IHelseIdEndpointsDiscoverer helseIdEndpointsDiscoverer,
        HelseIdConfiguration helseIdConfiguration) : base(signingTokenCreator, dPoPProofCreator, helseIdEndpointsDiscoverer)
    {
        _helseIdConfiguration = helseIdConfiguration;
    }

    public async Task<HelseIdTokenRequest> CreateTokenRequest(
        IPayloadClaimsCreator payloadClaimsCreator,
        AuthorizationCodeTokenRequestParameters tokenRequestParameters,
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
            //Resource = tokenRequestParameters.Resource,
            //Code = tokenRequestParameters.Code,
            //RedirectUri = tokenRequestParameters.RedirectUri,
            //CodeVerifier = tokenRequestParameters.CodeVerifier,
            GrantType = "authorization_code",
            DPoPProofToken = dpopProof,
            Scope = "",
        };
    }
}
