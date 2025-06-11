using HelseId.Standard.Interfaces.PayloadClaimCreators;
using HelseId.Standard.Models;
using HelseId.Standard.Models.TokenRequests;

namespace HelseId.Standard.Interfaces.TokenRequests;

public interface ITokenRequestBuilder<T> 
    where T : TokenRequestParameters
{
    Task<HelseIdTokenRequest> CreateTokenRequest(
        IPayloadClaimsCreator payloadClaimsCreator,
        T tokenRequestParameters,
        string? dPoPNonce = null);

}
