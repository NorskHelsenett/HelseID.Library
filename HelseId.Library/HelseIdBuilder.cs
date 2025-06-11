using Microsoft.Extensions.DependencyInjection;

namespace HelseId.Library;

internal sealed class HelseIdBuilder : IHelseIdBuilder
{
    public HelseIdBuilder(IServiceCollection services)
    {
        Services = services;
    }

    public IServiceCollection Services { get; private set; }
}
