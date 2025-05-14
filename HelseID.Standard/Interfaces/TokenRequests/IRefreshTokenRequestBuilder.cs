using HelseID.Standard.Models.TokenRequests;
using IdentityModel.Client;

namespace HelseID.Standard.Interfaces.TokenRequests;

public interface IRefreshTokenRequestBuilder : ITokenRequestBuilder<RefreshTokenRequestParameters> { }
