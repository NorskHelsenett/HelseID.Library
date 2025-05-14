using HelseID.Standard.Configuration;
using HelseID.Standard.Interfaces.Endpoints;
using HelseID.Standard.Interfaces.JwtTokens;
using HelseID.Standard.Interfaces.PayloadClaimCreators;
using HelseID.Standard.Interfaces.TokenRequests;
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
    public static void AddHelseId(this IServiceCollection services, HelseIdConfiguration helseIdConfiguration)
    {
        services.AddSingleton<IHelseIdTokenRetriever, HelseIdTokenRetriever>();
        services.AddSingleton<IClientCredentialsTokenRequestBuilder, ClientCredentialsTokenRequestBuilder>();
        services.AddSingleton<IDPoPProofCreator, DPoPProofCreator>();
        services.AddSingleton<IHelseIdEndpointsDiscoverer, HelseIdEndpointsDiscoverer>();
        services.AddSingleton<ISigningTokenCreator, SigningTokenCreator>();
        services.AddSingleton<IDiscoveryDocumentGetter, DiscoveryDocumentGetter>();
        services.AddSingleton<IPayloadClaimsCreator, ClientAssertionPayloadClaimsCreator>();
        services.AddSingleton<IAssertionDetailsCreator, AssertionDetailsCreator>();
        services.AddSingleton<IStructuredClaimsCreator, OrganizationNumberCreatorForMultiTenantClient>();
        services.AddSingleton(TimeProvider.System);
        services.AddMemoryCache();
        services.AddHttpClient();
        services.AddSingleton(helseIdConfiguration);
    }
}
