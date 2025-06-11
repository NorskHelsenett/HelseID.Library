using Microsoft.Extensions.DependencyInjection;

namespace HelseId.Library;

public static class HelseIdServiceCollectionExtensions
{
    public static IHelseIdBuilder AddHelseIdSingleTenant(this IHelseIdBuilder helseIdBuilder)
    {
        helseIdBuilder.RemoveServiceRegistrations<IStructuredClaimsCreator>();
        helseIdBuilder.Services.AddSingleton<IStructuredClaimsCreator, OrganizationNumberCreatorForSingleTenantClient>();
        return helseIdBuilder;
    }

    public static IHelseIdBuilder AddHelseIdMultiTenant(this IHelseIdBuilder helseIdBuilder)
    {
        helseIdBuilder.RemoveServiceRegistrations<IStructuredClaimsCreator>();
        helseIdBuilder.Services.AddSingleton<IStructuredClaimsCreator, OrganizationNumberCreatorForMultiTenantClient>();
        return helseIdBuilder;
    }

    public static IHelseIdBuilder AddHelseIdInMemoryCaching(this IHelseIdBuilder helseIdBuilder)
    {
        helseIdBuilder.RemoveServiceRegistrations<ITokenCache>();
        helseIdBuilder.RemoveServiceRegistrations<IDiscoveryDocumentCache>();
        helseIdBuilder.Services.AddSingleton<ITokenCache, InMemoryTokenCache>();
        helseIdBuilder.Services.AddSingleton<IDiscoveryDocumentCache, InMemoryDiscoveryDocumentCache>();
        return helseIdBuilder;
    }

    public static IHelseIdBuilder AddHelseIdDistributedCaching(this IHelseIdBuilder helseIdBuilder)
    {
        helseIdBuilder.RemoveServiceRegistrations<ITokenCache>();
        helseIdBuilder.RemoveServiceRegistrations<IDiscoveryDocumentCache>();
        helseIdBuilder.Services.AddDistributedMemoryCache();   
        helseIdBuilder.Services.AddSingleton<ITokenCache, DistributedTokenCache>();
        helseIdBuilder.Services.AddSingleton<IDiscoveryDocumentCache, DistributedDiscoveryDocumentCache>();
        return helseIdBuilder;
    }
}
