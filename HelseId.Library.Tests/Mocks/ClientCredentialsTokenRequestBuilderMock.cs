using HelseId.Library.ClientCredentials.Interfaces.TokenRequests;
using HelseId.Library.ClientCredentials.Models.TokenRequests;

namespace HelseId.Library.Tests.Mocks;

internal sealed class ClientCredentialsTokenRequestBuilderMock : IClientCredentialsTokenRequestBuilder
{
    internal ClientCredentialsTokenRequestParameters? TokenRequestParameters { get; private set; }
    
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
