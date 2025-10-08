using HelseId.Library.Services.Caching;
using HelseId.Library.Services.Configuration;
using HelseId.Library.Services.Endpoints;
using HelseId.Library.Services.JwtTokens;
using HelseId.Library.Services.PayloadClaimCreators.DetailsCreators;
using HelseId.Library.Services.PayloadClaimCreators.StructuredClaims;
using Microsoft.IdentityModel.Tokens;

namespace HelseId.Library.ClientCredentials;

public static class ClientCredentialsFlowFactory
{
    public static IHelseIdClientCredentialsFlow GetSingleTenant(HelseIdConfiguration configuration, string jwkPrivateKey)
    {
        return GetClientCredentialsFlowWithStructuredClaimsCreators(configuration,
            jwkPrivateKey,
            new OrganizationNumberCreatorForSingleTenantClient());
    }

    public static IHelseIdClientCredentialsFlow GetMultiTenant(HelseIdConfiguration configuration, string jwkPrivateKey)
    {
        return GetClientCredentialsFlowWithStructuredClaimsCreators(configuration,
            jwkPrivateKey,
            new OrganizationNumberCreatorForMultiTenantClient());
    }

    private static HelseIdClientCredentialsFlow GetClientCredentialsFlowWithStructuredClaimsCreators(
        HelseIdConfiguration configuration, 
        string jwkPrivateKey,
        params IStructuredClaimsCreator[] structuredClaimsCreators)
    {
        var configurationGetter = new RegisteredSingletonHelseIdConfigurationGetter(configuration);
        var jsonWebKey = new JsonWebKey(jwkPrivateKey);
        var signingCredentialsReference = new StaticSigningCredentialReference(new SigningCredentials(jsonWebKey, jsonWebKey.Alg));
        var signingTokenCreator = new SigningTokenCreator(configurationGetter, signingCredentialsReference);

        var dpopProofCreator = new DPoPProofCreator(signingCredentialsReference, TimeProvider.System);

        var httpClientFacotry = new InternalHttpClientFactory();
        var discoveryDocumentCache = new InMemoryDiscoveryDocumentCache(TimeProvider.System);

        var discoveryDocumentGetter = new DiscoveryDocumentGetter(configurationGetter, 
            httpClientFacotry, 
            discoveryDocumentCache);
        var endpointDiscoverer = new HelseIdEndpointsDiscoverer(discoveryDocumentGetter);

        var tokenRequestBulder = new ClientCredentialsTokenRequestBuilder(signingTokenCreator,
            dpopProofCreator,
            endpointDiscoverer,
            configurationGetter);

        var assertionDetailsCreator = new AssertionDetailsCreator(structuredClaimsCreators);
        var payloadClaimsCreator = new ClientAssertionPayloadClaimsCreator(TimeProvider.System, assertionDetailsCreator); 
        var tokenCache = new InMemoryTokenCache(TimeProvider.System);

        return new HelseIdClientCredentialsFlow(tokenRequestBulder,
            payloadClaimsCreator,
            httpClientFacotry,
            tokenCache,
            signingCredentialsReference);   
    }
    
    /// <summary>
    /// This is a basic implementation of the HttpClientFactory that reuses a socket connection for 30 seconds.
    /// We cannot use the regular HttpClientFactory since it depends on the ServiceLocator functionality
    /// normally used i .NET apps.
    /// This version follows the recommendations from Microsoft: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-5.0#alternatives-to-ihttpclientfactory-1
    /// </summary>
    private class InternalHttpClientFactory : IHttpClientFactory
    {
        private static readonly SocketsHttpHandler SocketsHttpHandler = new()
        {
            PooledConnectionLifetime = TimeSpan.FromSeconds(30) // Lifetime of the HelseID DNS record
        };
        
        public HttpClient CreateClient(string name)
        {
            return new HttpClient(SocketsHttpHandler, false);
        }
    }
}
