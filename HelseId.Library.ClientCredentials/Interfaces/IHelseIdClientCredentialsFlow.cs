namespace HelseId.Library.ClientCredentials.Interfaces;

public interface IHelseIdClientCredentialsFlow
{
    /// <summary>
    /// Retrieves an Access Token from HelseID using the default setup for the client.
    /// For a single-tenant client the Access Token will include the parent organization number that the
    /// client is set up with. For a multi-tenant client the Access Token will not contain any organization
    /// numbers.
    /// </summary>
    /// <returns>Returns a Token Response that can be either an AccessTokenResonse or a TokenErrorResponse</returns>
    Task<TokenResponse> GetTokenResponseAsync();

    /// <summary>
    /// Retrieves an Access Token from HelseID using the given organization numbers.
    /// For a single-tenant client only a child organization nubmer should be included.
    /// The Access Token will include the parent organization number that the client is set up with and
    /// the given child organization number as long as it exists in the clients whitelist.
    /// For a multi-tenant client the Access Token will contain the organization numbers supplied as long
    /// as a delegation exists from that organization to the supplier.
    /// </summary>
    /// <returns>Returns a Token Response that can be either an AccessTokenResonse or a TokenErrorResponse</returns>
    Task<TokenResponse> GetTokenResponseAsync(OrganizationNumbers organizationNumbers);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="scope"></param>
    /// <returns></returns>
    Task<TokenResponse> GetTokenResponseAsync(string scope);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="scope"></param>
    /// <param name="organizationNumbers"></param>
    /// <returns></returns>
    Task<TokenResponse> GetTokenResponseAsync(string scope, OrganizationNumbers organizationNumbers);

}
