using HelseId.Library.Interfaces.Configuration;

namespace HelseId.Library.Services.Configuration;

public class RegisteredSingletonHelseIdConfigurationGetter : IHelseIdConfigurationGetter
{
    private readonly HelseIdConfiguration _configuration;

    public RegisteredSingletonHelseIdConfigurationGetter(HelseIdConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<HelseIdConfiguration> GetConfiguration()
    {
        return Task.FromResult(_configuration);
    }
}
