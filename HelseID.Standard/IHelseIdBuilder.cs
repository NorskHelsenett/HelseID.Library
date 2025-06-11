using Microsoft.Extensions.DependencyInjection;

namespace HelseId.Standard;

public interface IHelseIdBuilder
{
    IServiceCollection Services { get; }
}
