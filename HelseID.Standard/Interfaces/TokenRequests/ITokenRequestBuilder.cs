using HelseID.Standard.Interfaces.PayloadClaimCreators;
using HelseID.Standard.Models;
using HelseID.Standard.Models.TokenRequests;
using IdentityModel.Client;

namespace HelseID.Standard.Interfaces.TokenRequests;

public interface ITokenRequestBuilder<T> 
    where T : TokenRequestParameters
{
    Task<HelseIdTokenRequest> CreateTokenRequest(
        IPayloadClaimsCreator payloadClaimsCreator,
        T tokenRequestParameters,
        string? dPoPNonce = null);

}
