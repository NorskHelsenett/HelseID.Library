using HelseId.Library.SelfService.Configuration;
using HelseId.Library.SelfService.Interfaces;
using HelseId.Library.SelfService.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HelseId.Library.SelfService;

public static class HelseIdServiceCollectionExtensions
{
    public static IHelseIdBuilder AddSelvbetjeningKeyRotation(this IHelseIdBuilder helseIdBuilder, IConfigurationSection selvbetjeningConfiguration)
    {
        helseIdBuilder.RemoveServiceRegistrations<ISelvbetjeningSecretUpdater>();
        helseIdBuilder.Services.AddSingleton<ISelvbetjeningSecretUpdater, SelvbetjeningSecretUpdater>();
        helseIdBuilder.Services.AddSingleton<IKeyManagementService, KeyManagementService>();
        helseIdBuilder.Services.AddSingleton(new SelvbetjeningConfiguration
        {
            UpdateClientSecretEndpoint = selvbetjeningConfiguration.GetValue<string>("UpdateClientSecretEndpoint")!,
            SelvbetjeningScope = selvbetjeningConfiguration.GetValue<string>("Scope")!
        });
        
        return helseIdBuilder;
    }
    
    public static IHelseIdBuilder AddSelvbetjeningKeyRotation(this IHelseIdBuilder helseIdBuilder, string updateClientSecretEndpoint, string scope)
    {
        helseIdBuilder.RemoveServiceRegistrations<ISelvbetjeningSecretUpdater>();
        helseIdBuilder.Services.AddSingleton<ISelvbetjeningSecretUpdater, SelvbetjeningSecretUpdater>();
        helseIdBuilder.Services.AddSingleton<IKeyManagementService, KeyManagementService>();
        helseIdBuilder.Services.AddSingleton(new SelvbetjeningConfiguration
        {
            UpdateClientSecretEndpoint = updateClientSecretEndpoint,
            SelvbetjeningScope = scope
        });
        helseIdBuilder.Services.AddSingleton<IClientSecretEndpoint, ClientSecretEndpoint>();
        
        return helseIdBuilder;
    }
}
