using HelseId.Library.SelfService.Interfaces;
using HelseId.Library.SelfService.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HelseId.Library.SelfService;

public static class HelseIdServiceCollectionExtensions
{
    public static IHelseIdBuilder AddSelvbetjeningKeyRotation (this IHelseIdBuilder helseIdBuilder)
    {
        helseIdBuilder.RemoveServiceRegistrations<ISelvbetjeningSecretUpdater>();
        helseIdBuilder.Services.AddSingleton<ISelvbetjeningSecretUpdater, SelvbetjeningSecretUpdater>();
        helseIdBuilder.Services.AddSingleton<IKeyManagementService, KeyManagementService>();
        return helseIdBuilder;
    }
}
