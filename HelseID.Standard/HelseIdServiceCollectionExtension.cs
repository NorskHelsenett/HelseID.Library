using HelseID.Standard.Configuration;
using HelseID.Standard.Interfaces.Caching;
using HelseID.Standard.Interfaces.Endpoints;
using HelseID.Standard.Interfaces.JwtTokens;
using HelseID.Standard.Interfaces.PayloadClaimCreators;
using HelseID.Standard.Interfaces.TokenRequests;
using HelseID.Standard.Services.Caching;
using HelseID.Standard.Services.Endpoints;
using HelseID.Standard.Services.JwtTokens;
using HelseID.Standard.Services.PayloadClaimCreators;
using HelseID.Standard.Services.PayloadClaimCreators.DetailsCreators;
using HelseID.Standard.Services.PayloadClaimCreators.StructuredClaims;
using HelseID.Standard.Services.TokenRequests;
using Microsoft.Extensions.DependencyInjection;

namespace HelseID.Standard;

public static class HelseIdServiceCollectionExtension
{
    public static IServiceCollection AddHelseId(this IServiceCollection services, HelseIdConfiguration helseIdConfiguration)
    {
        services.AddHelseId();
        services.AddSingleton(helseIdConfiguration);
        return services;
    }

    public static IServiceCollection AddHelseId(this IServiceCollection services)
    {
        services.AddSingleton<IHelseIdMachineToMachineFlow, HelseIdMachineToMachineFlow>();
        services.AddSingleton<IClientCredentialsTokenRequestBuilder, ClientCredentialsTokenRequestBuilder>();
        services.AddSingleton<IDPoPProofCreator, DPoPProofCreator>();
        services.AddSingleton<IHelseIdEndpointsDiscoverer, HelseIdEndpointsDiscoverer>();
        services.AddSingleton<ISigningTokenCreator, SigningTokenCreator>();
        services.AddSingleton<IDiscoveryDocumentGetter, DiscoveryDocumentGetter>();
        services.AddSingleton<IPayloadClaimsCreator, ClientAssertionPayloadClaimsCreator>();
        services.AddSingleton<IAssertionDetailsCreator, AssertionDetailsCreator>();
        services.AddSingleton<IStructuredClaimsCreator, OrganizationNumberCreatorForSingleTenantClient>();
        services.AddSingleton(TimeProvider.System);
        services.AddHttpClient();
        
        services.AddHelseIdSingleTenant();
        services.AddHelseIdInMemoryCaching();
        
        return services;
    }

    public static IServiceCollection AddHelseIdSingleTenant(this IServiceCollection services)
    {
        RemoveServiceRegistrations<IStructuredClaimsCreator>(services);
        services.AddSingleton<IStructuredClaimsCreator, OrganizationNumberCreatorForSingleTenantClient>();
        return services;
    }

    public static IServiceCollection AddHelseIdMultiTenant(this IServiceCollection services)
    {
        RemoveServiceRegistrations<IStructuredClaimsCreator>(services);
        services.AddSingleton<IStructuredClaimsCreator, OrganizationNumberCreatorForMultiTenantClient>();
        return services;
    }

    public static IServiceCollection AddHelseIdInMemoryCaching(this IServiceCollection services)
    {
        RemoveServiceRegistrations<ITokenCache>(services);
        RemoveServiceRegistrations<IDiscoveryDocumentCache>(services);
        services.AddSingleton<ITokenCache, InMemoryTokenCache>();
        services.AddSingleton<IDiscoveryDocumentCache, InMemoryDiscoveryDocumentCache>();
        return services;
    }

    public static IServiceCollection AddHelseIdDistributedCaching(this IServiceCollection services)
    {
        RemoveServiceRegistrations<ITokenCache>(services);
        RemoveServiceRegistrations<IDiscoveryDocumentCache>(services);
        services.AddDistributedMemoryCache();   
        services.AddSingleton<ITokenCache, DistributedTokenCache>();
        services.AddSingleton<IDiscoveryDocumentCache, DistributedDiscoveryDocumentCache>();
        return services;
    }

    private static void RemoveServiceRegistrations<TService>(IServiceCollection services)
    {
        var existingServiceRegistration = services.Where(s => s.ServiceType == typeof(TService)).ToArray();
        foreach (var serviceRegistration in existingServiceRegistration)
        {
            services.Remove(serviceRegistration);
        }
    }
}
