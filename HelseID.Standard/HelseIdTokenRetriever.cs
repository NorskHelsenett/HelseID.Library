using HelseID.Standard.Interfaces.PayloadClaimCreators;
using HelseID.Standard.Interfaces.TokenRequests;
using HelseID.Standard.Models.Payloads;
using HelseID.Standard.Models.TokenRequests;
using HelseID.Standard.Services.TokenRequests;
using IdentityModel.Client;

namespace HelseID.Standard;

public interface IHelseIdTokenRetriever
{
    Task<string> GetTokenAsync();
}

public class HelseIdTokenRetriever : IHelseIdTokenRetriever
{
    private readonly IClientCredentialsTokenRequestBuilder _clientCredentialsTokenRequestBuilder;
    private readonly IPayloadClaimsCreator _payloadClaimsCreator;

    public HelseIdTokenRetriever(IClientCredentialsTokenRequestBuilder clientCredentialsTokenRequestBuilder, IPayloadClaimsCreator payloadClaimsCreator)
    {
        _clientCredentialsTokenRequestBuilder = clientCredentialsTokenRequestBuilder;
        _payloadClaimsCreator = payloadClaimsCreator;
    }
    public async Task<string> GetTokenAsync()
    {
        var clientCredentialsTokenRequestParameters = new ClientCredentialsTokenRequestParameters
        {
            PayloadClaimParameters = new PayloadClaimParameters { UseOrganizationNumbers = false }
        };
        var request = await _clientCredentialsTokenRequestBuilder.CreateTokenRequest(_payloadClaimsCreator, clientCredentialsTokenRequestParameters);
 
        var result = await new HttpClient().RequestClientCredentialsTokenAsync(request);
        
        return result.AccessToken!;
    }
}
