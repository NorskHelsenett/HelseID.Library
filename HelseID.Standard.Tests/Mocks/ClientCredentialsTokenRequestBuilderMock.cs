using HelseID.Standard.Interfaces.PayloadClaimCreators;
using HelseID.Standard.Interfaces.TokenRequests;
using HelseID.Standard.Models;
using HelseID.Standard.Models.Constants;
using HelseID.Standard.Models.TokenRequests;

namespace HelseID.Standard.Tests.Mocks;

public class ClientCredentialsTokenRequestBuilderMock : IClientCredentialsTokenRequestBuilder
{
    public Task<HelseIdTokenRequest> CreateTokenRequest(
        IPayloadClaimsCreator payloadClaimsCreator,
        ClientCredentialsTokenRequestParameters tokenRequestParameters,
        string? dPoPNonce = null)
    {
        return Task.FromResult(new HelseIdTokenRequest
        {
            GrantType = GrantTypes.ClientCredentials,
            Address = "https://helseid-sts.nhn.no/connect/token",
            ClientAssertion = "client assertion",
            ClientId = "client id",
            DPoPProofToken = "dpop proof",
            Scope = "scopes"
        });
    }
}
