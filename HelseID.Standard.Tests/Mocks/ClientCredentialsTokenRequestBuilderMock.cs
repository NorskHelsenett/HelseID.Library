using HelseId.Standard.Interfaces.PayloadClaimCreators;
using HelseId.Standard.Interfaces.TokenRequests;
using HelseId.Standard.Models;
using HelseId.Standard.Models.Constants;
using HelseId.Standard.Models.TokenRequests;

namespace HelseId.Standard.Tests.Mocks;

public class ClientCredentialsTokenRequestBuilderMock : IClientCredentialsTokenRequestBuilder
{
    public ClientCredentialsTokenRequestParameters TokenRequestParameters { get; private set; }
    
    public Task<HelseIdTokenRequest> CreateTokenRequest(
        IPayloadClaimsCreator payloadClaimsCreator,
        ClientCredentialsTokenRequestParameters tokenRequestParameters,
        string? dPoPNonce = null)
    {
        TokenRequestParameters = tokenRequestParameters;
        
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
