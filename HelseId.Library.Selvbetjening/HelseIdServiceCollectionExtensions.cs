using HelseId.Library.Configuration;
using HelseId.Library.Selvbetjening.Interfaces;
using HelseId.Library.Selvbetjening.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HelseId.Library.Selvbetjening;

public static class HelseIdServiceCollectionExtensions
{
    public static IHelseIdBuilder AddSelvbetjeningKeyRotation(this IHelseIdBuilder helseIdBuilder)
    {
        RemoveServiceRegistrations(helseIdBuilder);
        helseIdBuilder.Services.AddSingleton<ISelvbetjeningSecretUpdater, SelvbetjeningSecretUpdater>();
        helseIdBuilder.Services.AddSingleton<IKeyManagementService, KeyManagementService>();
        helseIdBuilder.Services.AddSingleton<IClientSecretEndpoint, ClientSecretEndpoint>();
        
        return helseIdBuilder;
    }

    private static void RemoveServiceRegistrations(IHelseIdBuilder helseIdBuilder)
    {
        helseIdBuilder.RemoveServiceRegistrations<ISelvbetjeningSecretUpdater>();
        helseIdBuilder.RemoveServiceRegistrations<IKeyManagementService>();
        helseIdBuilder.RemoveServiceRegistrations<IClientSecretEndpoint>();
    }
}
