using HelseId.Library.Interfaces.Configuration;
using HelseId.Library.Services.Configuration;

namespace HelseId.Library.MachineToMachine;

public static class HelseIdServiceCollectionExtension
{
    /// <summary>
    /// Registers the HelseID machine to machine flow for a single-tenant client with caching of tokens in local memory
    /// </summary>
    /// <param name="services"></param>
    /// <param name="helseIdConfiguration">A configuration object that matches the HelseID client to be used</param>
    /// <returns></returns>
    public static IHelseIdBuilder AddHelseIdMachineToMachine(this IServiceCollection services, HelseIdConfiguration helseIdConfiguration)
    {
        var helseIdBuilder = new HelseIdBuilder(services);
        helseIdBuilder.AddHelseIdMachineToMachineInternal();
        helseIdBuilder.Services.AddSingleton(helseIdConfiguration);
        helseIdBuilder.Services.AddSingleton<IHelseIdConfigurationGetter, RegisteredSingletonHelseIdConfigurationGetter>();
        return helseIdBuilder;
    }

    /// <summary>
    /// Registers the HelseID machine to machine flow for a single-tenant client with caching of tokens in local memory.
    /// A configuration object must be registered as a singleton by the consumer.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IHelseIdBuilder AddHelseIdMachineToMachine(this IServiceCollection services)
    {
        var helseIdBuilder = new HelseIdBuilder(services);
        helseIdBuilder.AddHelseIdMachineToMachineInternal();        
        return helseIdBuilder;
    }

    private static void AddHelseIdMachineToMachineInternal(this HelseIdBuilder helseIdBuilder)
    {
        helseIdBuilder.Services.AddSingleton<IHelseIdMachineToMachineFlow, HelseIdMachineToMachineFlow>();
        helseIdBuilder.Services.AddSingleton<IClientCredentialsTokenRequestBuilder, ClientCredentialsTokenRequestBuilder>();
        helseIdBuilder.Services.AddSingleton<IPayloadClaimsCreator, ClientAssertionPayloadClaimsCreator>();

        helseIdBuilder.AddHelseIdSingleTenant();
        helseIdBuilder.AddHelseIdInMemoryCaching();
    }
}
