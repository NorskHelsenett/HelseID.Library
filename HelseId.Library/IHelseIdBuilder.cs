using Microsoft.Extensions.DependencyInjection;

namespace HelseId.Library;

public interface IHelseIdBuilder
{
    IServiceCollection Services { get; }
}
