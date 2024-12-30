using HelseID.Standard.Interfaces.PayloadClaimCreators;
using HelseID.Standard.Models.TokenRequests;
using IdentityModel.Client;

namespace HelseID.Standard.Interfaces.TokenRequests;

public interface ITokenRequestBuilder<T, TT> 
    where T : TokenRequest
    where TT : TokenRequestParameters
{
    Task<T> CreateTokenRequest(
        IPayloadClaimsCreator payloadClaimsCreator,
        TT tokenRequestParameters,
        string? dPoPNonce = null);

}
