using HelseID.Standard.Models.TokenRequests;

namespace HelseID.Standard.Interfaces.TokenRequests;

public interface IAuthorizationCodeTokenRequestBuilder 
    : ITokenRequestBuilder<AuthorizationCodeTokenRequestParameters> { }
