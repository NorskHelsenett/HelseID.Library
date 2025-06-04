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

public static class ServicesExtension
{
    public static IServiceCollection AddHelseId(this IServiceCollection services, HelseIdConfiguration helseIdConfiguration)
    {
        services.AddSingleton<IHelseIdMachineToMachineFlow, HelseIdMachineToMachineFlow>();
        services.AddSingleton<IClientCredentialsTokenRequestBuilder, ClientCredentialsTokenRequestBuilder>();
        services.AddSingleton<IDPoPProofCreator, DPoPProofCreator>();
        services.AddSingleton<IHelseIdEndpointsDiscoverer, HelseIdEndpointsDiscoverer>();
        services.AddSingleton<ISigningTokenCreator, SigningTokenCreator>();
        services.AddSingleton<IDiscoveryDocumentGetter, DiscoveryDocumentGetter>();
        services.AddSingleton<IPayloadClaimsCreator, ClientAssertionPayloadClaimsCreator>();
        services.AddSingleton<IAssertionDetailsCreator, AssertionDetailsCreator>();
        services.AddSingleton<IStructuredClaimsCreator, OrganizationNumberCreatorForMultiTenantClient>();
        services.AddSingleton(TimeProvider.System);
        services.AddHttpClient();
        services.AddSingleton(helseIdConfiguration);
        return services;
    }

    public static IServiceCollection AddLocalCaching(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddSingleton<ITokenCache, InMemoryTokenCache>();
        services.AddSingleton<IDiscoveryDocumentCache, InMemoryDiscoveryDocumentCache>();
        return services;
    }

    public static IServiceCollection AddDistributedCaching(this IServiceCollection services)
    {
        services.AddDistributedMemoryCache();   
        services.AddSingleton<ITokenCache, DistributedTokenCache>();
        services.AddSingleton<IDiscoveryDocumentCache, DistributedDiscoveryDocumentCache>();
        return services;
    }
}
