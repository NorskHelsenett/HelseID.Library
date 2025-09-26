using HelseId.Library.ClientCredentials.Models.TokenRequests;

namespace HelseId.Library.ClientCredentials.Interfaces.TokenRequests;

public interface IClientCredentialsTokenRequestBuilder 
    : ITokenRequestBuilder<ClientCredentialsTokenRequestParameters> { }
