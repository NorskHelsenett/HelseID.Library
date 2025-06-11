using HelseId.Standard.Configuration;
using HelseId.Standard.Interfaces.Caching;
using HelseId.Standard.Interfaces.Endpoints;
using HelseId.Standard.Interfaces.JwtTokens;
using HelseId.Standard.Interfaces.PayloadClaimCreators;
using HelseId.Standard.Interfaces.TokenRequests;
using HelseId.Standard.Services.Caching;
using HelseId.Standard.Services.Endpoints;
using HelseId.Standard.Services.JwtTokens;
using HelseId.Standard.Services.PayloadClaimCreators;
using HelseId.Standard.Services.PayloadClaimCreators.DetailsCreators;
using HelseId.Standard.Services.PayloadClaimCreators.StructuredClaims;
using HelseId.Standard.Services.TokenRequests;
using Microsoft.Extensions.DependencyInjection;

namespace HelseId.Standard;

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
