using HelseId.Standard.Models.TokenRequests;

namespace HelseId.Standard.Interfaces.TokenRequests;

public interface IClientCredentialsTokenRequestBuilder 
    : ITokenRequestBuilder<ClientCredentialsTokenRequestParameters> { }
