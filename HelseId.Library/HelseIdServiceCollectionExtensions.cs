using HelseId.Library.Services.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HelseId.Library;

public static class HelseIdServiceCollectionExtensions
{
    /// <summary>
    /// Setup HelseID to use the single-tenant pattern
    /// </summary>
    /// <param name="helseIdBuilder"></param>
    /// <returns></returns>
    public static IHelseIdBuilder AddHelseIdSingleTenant(this IHelseIdBuilder helseIdBuilder)
    {
        helseIdBuilder.RemoveServiceRegistrations<IStructuredClaimsCreator>();
        helseIdBuilder.Services.AddSingleton<IStructuredClaimsCreator, OrganizationNumberCreatorForSingleTenantClient>();
        return helseIdBuilder;
    }

    /// <summary>
    /// Setup HelseID to use the multi-tenant pattern
    /// </summary>
    /// <param name="helseIdBuilder"></param>
    /// <returns></returns>
    public static IHelseIdBuilder AddHelseIdMultiTenant(this IHelseIdBuilder helseIdBuilder)
    {
        helseIdBuilder.RemoveServiceRegistrations<IStructuredClaimsCreator>();
        helseIdBuilder.Services.AddSingleton<IStructuredClaimsCreator, OrganizationNumberCreatorForMultiTenantClient>();
        return helseIdBuilder;
    }

    /// <summary>
    /// Setup HelseID to cache tokens in memory on the server
    /// </summary>
    /// <param name="helseIdBuilder"></param>
    /// <returns></returns>
    public static IHelseIdBuilder AddHelseIdInMemoryCaching(this IHelseIdBuilder helseIdBuilder)
    {
        helseIdBuilder.RemoveServiceRegistrations<ITokenCache>();
        helseIdBuilder.RemoveServiceRegistrations<IDiscoveryDocumentCache>();
        helseIdBuilder.Services.AddSingleton<ITokenCache, InMemoryTokenCache>();
        helseIdBuilder.Services.AddSingleton<IDiscoveryDocumentCache, InMemoryDiscoveryDocumentCache>();
        return helseIdBuilder;
    }

    /// <summary>
    /// Setup HelseID to cache tokens in a distributed cache.
    /// An IDistributedCache implementation must be registered. 
    /// </summary>
    /// <param name="helseIdBuilder"></param>
    /// <returns></returns>
    public static IHelseIdBuilder AddHelseIdDistributedCaching(this IHelseIdBuilder helseIdBuilder)
    {
        helseIdBuilder.RemoveServiceRegistrations<ITokenCache>();
        helseIdBuilder.RemoveServiceRegistrations<IDiscoveryDocumentCache>();
        helseIdBuilder.Services.AddSingleton<ITokenCache, DistributedTokenCache>();
        helseIdBuilder.Services.AddSingleton<IDiscoveryDocumentCache, DistributedDiscoveryDocumentCache>();
        return helseIdBuilder;
    }
    
    /// <summary>
    /// Adds a Json Web Key stored in a string as the signing credential 
    /// </summary>
    /// <param name="helseIdBuilder"></param>
    /// <param name="jsonWebKey">The Json Web Key</param>
    /// <returns></returns>
    public static IHelseIdBuilder AddJwkForClientAuthentication(this IHelseIdBuilder helseIdBuilder, string jsonWebKey)
    {
        var signingKey = new JsonWebKey(jsonWebKey);
        var signingCredentials = new SigningCredentials(signingKey, signingKey.Alg);
        return helseIdBuilder.AddSigningCredentialForClientAuthentication(signingCredentials);
    }
    
    /// <summary>
    /// Adds a Json Web Key stored in a file as the signing credential
    /// </summary>
    /// <param name="helseIdBuilder"></param>
    /// <param name="jwkFileName">The full path of the file containing the Json Web Key</param>
    /// <returns></returns>
    public static IHelseIdBuilder AddJwkFileForClientAuthentication(this IHelseIdBuilder helseIdBuilder, string jwkFileName)  
    {
        helseIdBuilder.RemoveServiceRegistrations<ISigningCredentialReference>();

        helseIdBuilder.Services.AddSingleton<ISigningCredentialReference>(new FileBasedSigningCredentialReference(jwkFileName));
        return helseIdBuilder;
    }
    
    /// <summary>
    /// Adds a generic signing credential as the signing credential to be used for client authentication
    /// </summary>
    /// <param name="helseIdBuilder"></param>
    /// <param name="signingCredential">The signing credentials</param>
    /// <returns></returns>
    public static IHelseIdBuilder AddSigningCredentialForClientAuthentication(this IHelseIdBuilder helseIdBuilder, SigningCredentials signingCredential)  
    {
        helseIdBuilder.RemoveServiceRegistrations<ISigningCredentialReference>();
        
        helseIdBuilder.Services.AddSingleton<ISigningCredentialReference>(new StaticSigningCredentialReference(signingCredential));
        
        return helseIdBuilder;
    }
    
    /// <summary>
    /// Adds a X509Certificate2 as the signing credential to be used for client authentication
    /// </summary>
    /// <param name="helseIdBuilder"></param>
    /// <param name="certificate">The X509Certificate2 to be used</param>
    /// <param name="algorithm">The signing algorithm to be used</param>
    /// <returns></returns>
    public static IHelseIdBuilder AddX509CertificateForForClientAuthentication(this IHelseIdBuilder helseIdBuilder, X509Certificate2 certificate, string algorithm)  
    {
        helseIdBuilder.RemoveServiceRegistrations<ISigningCredentialReference>();

        var x509SigningCredentials = new X509SigningCredentials(certificate, algorithm);
        var key = x509SigningCredentials.Key as X509SecurityKey;
        var jsonWebKey = JsonWebKeyConverter.ConvertFromX509SecurityKey(key, representAsRsaKey: true);
        var signingCredental = new SigningCredentials(jsonWebKey, algorithm);

        helseIdBuilder.Services.AddSingleton<ISigningCredentialReference>(new StaticSigningCredentialReference(signingCredental));
        return helseIdBuilder;
    }
}

