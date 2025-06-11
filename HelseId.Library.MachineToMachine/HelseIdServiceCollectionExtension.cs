using HelseId.Library.MachineToMachine.Interfaces;

namespace HelseId.Library.MachineToMachine;

public static class HelseIdServiceCollectionExtension
{
    public static IHelseIdBuilder AddHelseIdMachineToMachine(this IServiceCollection services, HelseIdConfiguration helseIdConfiguration)
    {
        var helseIdBuilder = new HelseIdBuilder(services);
        helseIdBuilder.AddHelseIdMachineToMachineInternal();
        helseIdBuilder.Services.AddSingleton(helseIdConfiguration);
        return helseIdBuilder;
    }

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
        
        helseIdBuilder.AddHelseIdSingleTenant();
        helseIdBuilder.AddHelseIdInMemoryCaching();
    }
}
