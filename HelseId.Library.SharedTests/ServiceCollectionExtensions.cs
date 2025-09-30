using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Helseid.Library.SharedTests;

public static class ServiceCollectionExtensions
{
    public static void EnsureSingletonRegistration<TServiceType, TImplementationType>(this ServiceCollection serviceCollection)
    {
        var registeredService = RegisteredServiceShouldBeSingleton<TServiceType>(serviceCollection);
        registeredService.ImplementationType.Should().BeSameAs(typeof(TImplementationType));
    }

    public static void EnsureSingletonRegistration<TServiceType, TImplementationType>(this ServiceCollection serviceCollection, TImplementationType objectInstance)
    {
        var registeredService = RegisteredServiceShouldBeSingleton<TServiceType>(serviceCollection);
        registeredService.ImplementationInstance.Should().BeSameAs(objectInstance);
    }
    
    public static void EnsureSingletonRegistration<TServiceType>(this ServiceCollection serviceCollection, TServiceType objectInstance)
    {
        serviceCollection.EnsureSingletonRegistration<TServiceType, TServiceType>(objectInstance);
    }

    private static ServiceDescriptor RegisteredServiceShouldBeSingleton<TServiceType>(ServiceCollection serviceCollection)
    {
        var registeredService = serviceCollection.SingleOrDefault(s => s.ServiceType == typeof(TServiceType));
        registeredService.Should().NotBeNull();
        registeredService.Lifetime.Should().Be(ServiceLifetime.Singleton);
        return registeredService;
    }

}
