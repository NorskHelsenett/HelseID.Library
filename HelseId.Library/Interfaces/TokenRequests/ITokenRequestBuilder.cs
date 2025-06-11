using HelseId.Library.Interfaces.PayloadClaimCreators;
using HelseId.Library.Models;
using HelseId.Library.Models.TokenRequests;

namespace HelseId.Library.Interfaces.TokenRequests;

public interface ITokenRequestBuilder<T> 
    where T : TokenRequestParameters
{
    Task<HelseIdTokenRequest> CreateTokenRequest(
        IPayloadClaimsCreator payloadClaimsCreator,
        T tokenRequestParameters,
        string? dPoPNonce = null);

}
