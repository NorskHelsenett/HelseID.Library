using HelseId.Standard.Models.TokenRequests;

namespace HelseId.Standard.Interfaces.TokenRequests;

public interface IAuthorizationCodeTokenRequestBuilder 
    : ITokenRequestBuilder<AuthorizationCodeTokenRequestParameters> { }
