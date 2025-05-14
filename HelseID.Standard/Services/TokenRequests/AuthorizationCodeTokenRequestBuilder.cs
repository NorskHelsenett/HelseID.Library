using HelseID.Standard.Configuration;
using HelseID.Standard.Interfaces.ClientAssertions;
using HelseID.Standard.Interfaces.Endpoints;
using HelseID.Standard.Interfaces.JwtTokens;
using HelseID.Standard.Interfaces.PayloadClaimCreators;
using HelseID.Standard.Interfaces.TokenRequests;
using HelseID.Standard.Models;
using HelseID.Standard.Models.TokenRequests;
using IdentityModel;
using IdentityModel.Client;

namespace HelseID.Standard.Services.TokenRequests;

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

        return new HelseIdTokenRequest()
        {
            Address = tokenEndpoint,
            ClientAssertion = clientAssertion,
            ClientId = _helseIdConfiguration.ClientId,
            //Resource = tokenRequestParameters.Resource,
            //Code = tokenRequestParameters.Code,
            //RedirectUri = tokenRequestParameters.RedirectUri,
            //CodeVerifier = tokenRequestParameters.CodeVerifier,
            GrantType = OidcConstants.GrantTypes.AuthorizationCode,
            DPoPProofToken = dpopProof,
        };
    }
}
