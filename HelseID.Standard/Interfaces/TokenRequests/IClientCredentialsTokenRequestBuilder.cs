using HelseID.Standard.Models.TokenRequests;

namespace HelseID.Standard.Interfaces.TokenRequests;

public interface IClientCredentialsTokenRequestBuilder 
    : ITokenRequestBuilder<ClientCredentialsTokenRequestParameters> { }
