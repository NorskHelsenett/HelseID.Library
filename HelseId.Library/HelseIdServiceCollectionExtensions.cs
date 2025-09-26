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
    
    public static IHelseIdBuilder AddHelseIdConfigurationGetter<TService>(this IHelseIdBuilder helseIdBuilder) where TService : class, IHelseIdConfigurationGetter
    {
        helseIdBuilder.Services.AddSingleton<IHelseIdConfigurationGetter, TService>();
        return helseIdBuilder;
    }

    public static IHelseIdBuilder AddHelseIdConfigurationGetter(
        this IHelseIdBuilder helseIdBuilder,
        IHelseIdConfigurationGetter configurationGetterInstance)
    {
        helseIdBuilder.Services.AddSingleton(configurationGetterInstance);
        return helseIdBuilder;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="helseIdBuilder"></param>
    /// <param name="jsonWebKey"></param>
    /// <returns></returns>
    public static IHelseIdBuilder AddSigningCredential(this IHelseIdBuilder helseIdBuilder, string jsonWebKey)
    {
        helseIdBuilder.RemoveServiceRegistrations<ISigningCredentialReference>();
        
        var signingKey = new JsonWebKey(jsonWebKey);
        var signingCredentials = new SigningCredentials(signingKey, signingKey.Alg);
        helseIdBuilder.Services.AddSingleton<ISigningCredentialReference>(new StaticSigningCredentialReference(signingCredentials));
        return helseIdBuilder;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="helseIdBuilder"></param>
    /// <param name="signingCredential"></param>
    /// <returns></returns>
    public static IHelseIdBuilder AddSigningCredential(this IHelseIdBuilder helseIdBuilder, SigningCredentials signingCredential)  {
        helseIdBuilder.RemoveServiceRegistrations<ISigningCredentialReference>();
        helseIdBuilder.Services.AddSingleton<ISigningCredentialReference>(new StaticSigningCredentialReference(signingCredential));
        return helseIdBuilder;
    }
    
    public static IHelseIdBuilder AddSigningCredential(this IHelseIdBuilder helseIdBuilder, X509Certificate2 certificate, string algorithm)  
    {
        helseIdBuilder.RemoveServiceRegistrations<ISigningCredentialReference>();

        var x509SigningCredentials = new X509SigningCredentials(certificate, algorithm);
        var key = x509SigningCredentials.Key as X509SecurityKey;
        var jsonWebKey = JsonWebKeyConverter.ConvertFromX509SecurityKey(key, representAsRsaKey: true);
        var signingCredental = new SigningCredentials(jsonWebKey, algorithm);

        helseIdBuilder.Services.AddSingleton<ISigningCredentialReference>(new StaticSigningCredentialReference(signingCredental));
        return helseIdBuilder;
    }
    
    public static IHelseIdBuilder AddFileBasedSigningCredential(this IHelseIdBuilder helseIdBuilder, string fileName)  
    {
        helseIdBuilder.RemoveServiceRegistrations<ISigningCredentialReference>();

        helseIdBuilder.Services.AddSingleton<ISigningCredentialReference>(new FileBasedSigningCredentialReference(fileName));
        return helseIdBuilder;
    }
}

