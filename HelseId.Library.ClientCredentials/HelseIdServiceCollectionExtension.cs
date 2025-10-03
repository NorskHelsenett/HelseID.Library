using HelseId.Library.Interfaces.Configuration;
using HelseId.Library.Services.Configuration;

namespace HelseId.Library.ClientCredentials;

public static class HelseIdServiceCollectionExtension
{
    /// <summary>
    /// Registers the HelseID client credentials flow for a single-tenant client with caching of tokens in local memory
    /// </summary>
    /// <param name="services"></param>
    /// <param name="helseIdConfiguration">A configuration object that matches the HelseID client to be used</param>
    /// <returns></returns>
    public static IHelseIdBuilder AddHelseIdClientCredentials(this IServiceCollection services, HelseIdConfiguration helseIdConfiguration)
    {
        var helseIdBuilder = new HelseIdBuilder(services);
        helseIdBuilder.Services.AddSingleton(helseIdConfiguration);
        helseIdBuilder.Services.AddSingleton<IHelseIdConfigurationGetter, RegisteredSingletonHelseIdConfigurationGetter>();
        helseIdBuilder.Services.AddSingleton<IHelseIdClientCredentialsFlow, HelseIdClientCredentialsFlow>();
        helseIdBuilder.Services.AddSingleton<IClientCredentialsTokenRequestBuilder, ClientCredentialsTokenRequestBuilder>();
        helseIdBuilder.Services.AddSingleton<IPayloadClaimsCreator, ClientAssertionPayloadClaimsCreator>();

        helseIdBuilder.AddHelseIdSingleTenant();
        helseIdBuilder.AddHelseIdInMemoryCaching();
        
        return helseIdBuilder;
    }}
