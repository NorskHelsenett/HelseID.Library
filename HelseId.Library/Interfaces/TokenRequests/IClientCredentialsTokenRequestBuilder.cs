using HelseId.Library.Models.TokenRequests;

namespace HelseId.Library.Interfaces.TokenRequests;

public interface IClientCredentialsTokenRequestBuilder 
    : ITokenRequestBuilder<ClientCredentialsTokenRequestParameters> { }
