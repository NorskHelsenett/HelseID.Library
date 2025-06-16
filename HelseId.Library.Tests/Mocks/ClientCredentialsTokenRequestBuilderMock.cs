using HelseId.Library.MachineToMachine.Interfaces.TokenRequests;
using HelseId.Library.MachineToMachine.Models.TokenRequests;

namespace HelseId.Library.Tests.Mocks;

public class ClientCredentialsTokenRequestBuilderMock : IClientCredentialsTokenRequestBuilder
{
    public ClientCredentialsTokenRequestParameters? TokenRequestParameters { get; private set; }
    
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
