using HelseId.Library.Configuration;
using HelseId.Library.Interfaces.Caching;
using HelseId.Library.Interfaces.Endpoints;
using HelseId.Library.Interfaces.JwtTokens;
using HelseId.Library.Interfaces.PayloadClaimCreators;
using HelseId.Library.Interfaces.TokenRequests;
using HelseId.Library.Services.Caching;
using HelseId.Library.Services.Endpoints;
using HelseId.Library.Services.JwtTokens;
using HelseId.Library.Services.PayloadClaimCreators;
using HelseId.Library.Services.PayloadClaimCreators.DetailsCreators;
using HelseId.Library.Services.PayloadClaimCreators.StructuredClaims;
using HelseId.Library.Services.TokenRequests;
using Microsoft.Extensions.DependencyInjection;

namespace HelseId.Library;

public static class HelseIdServiceCollectionExtension
{
    public static IHelseIdBuilder AddHelseId(this IServiceCollection services, HelseIdConfiguration helseIdConfiguration)
    {
        var helseIdBuilder = new HelseIdBuilder(services);
        
        helseIdBuilder.Services.AddHelseId();
        helseIdBuilder.Services.AddSingleton(helseIdConfiguration);
        return helseIdBuilder;
    }

    public static IHelseIdBuilder AddHelseId(this IServiceCollection services)
    {
        var helseIdBuilder = new HelseIdBuilder(services);
        
        helseIdBuilder.Services.AddSingleton<IHelseIdMachineToMachineFlow, HelseIdMachineToMachineFlow>();
        helseIdBuilder.Services.AddSingleton<IClientCredentialsTokenRequestBuilder, ClientCredentialsTokenRequestBuilder>();
        helseIdBuilder.Services.AddSingleton<IDPoPProofCreator, DPoPProofCreator>();
        helseIdBuilder.Services.AddSingleton<IHelseIdEndpointsDiscoverer, HelseIdEndpointsDiscoverer>();
        helseIdBuilder.Services.AddSingleton<ISigningTokenCreator, SigningTokenCreator>();
        helseIdBuilder.Services.AddSingleton<IDiscoveryDocumentGetter, DiscoveryDocumentGetter>();
        helseIdBuilder.Services.AddSingleton<IPayloadClaimsCreator, ClientAssertionPayloadClaimsCreator>();
        helseIdBuilder.Services.AddSingleton<IAssertionDetailsCreator, AssertionDetailsCreator>();
        helseIdBuilder.Services.AddSingleton<IStructuredClaimsCreator, OrganizationNumberCreatorForSingleTenantClient>();
        helseIdBuilder.Services.AddSingleton(TimeProvider.System);
        helseIdBuilder.Services.AddHttpClient();
        
        helseIdBuilder.AddHelseIdSingleTenant();
        helseIdBuilder.AddHelseIdInMemoryCaching();
        
        return helseIdBuilder;
    }

    public static IHelseIdBuilder AddHelseIdSingleTenant(this IHelseIdBuilder helseIdBuilder)
    {
        RemoveServiceRegistrations<IStructuredClaimsCreator>(helseIdBuilder);
        helseIdBuilder.Services.AddSingleton<IStructuredClaimsCreator, OrganizationNumberCreatorForSingleTenantClient>();
        return helseIdBuilder;
    }

    public static IHelseIdBuilder AddHelseIdMultiTenant(this IHelseIdBuilder helseIdBuilder)
    {
        RemoveServiceRegistrations<IStructuredClaimsCreator>(helseIdBuilder);
        helseIdBuilder.Services.AddSingleton<IStructuredClaimsCreator, OrganizationNumberCreatorForMultiTenantClient>();
        return helseIdBuilder;
    }

    public static IHelseIdBuilder AddHelseIdInMemoryCaching(this IHelseIdBuilder services)
    {
        RemoveServiceRegistrations<ITokenCache>(services);
        RemoveServiceRegistrations<IDiscoveryDocumentCache>(services);
        services.Services.AddSingleton<ITokenCache, InMemoryTokenCache>();
        services.Services.AddSingleton<IDiscoveryDocumentCache, InMemoryDiscoveryDocumentCache>();
        return services;
    }

    public static IHelseIdBuilder AddHelseIdDistributedCaching(this IHelseIdBuilder services)
    {
        RemoveServiceRegistrations<ITokenCache>(services);
        RemoveServiceRegistrations<IDiscoveryDocumentCache>(services);
        services.Services.AddDistributedMemoryCache();   
        services.Services.AddSingleton<ITokenCache, DistributedTokenCache>();
        services.Services.AddSingleton<IDiscoveryDocumentCache, DistributedDiscoveryDocumentCache>();
        return services;
    }

    private static void RemoveServiceRegistrations<TService>(IHelseIdBuilder helseIdBuilder)
    {
        var existingServiceRegistration = helseIdBuilder.Services.Where(s => s.ServiceType == typeof(TService)).ToArray();
        foreach (var serviceRegistration in existingServiceRegistration)
        {
            helseIdBuilder.Services.Remove(serviceRegistration);
        }
    }
}
