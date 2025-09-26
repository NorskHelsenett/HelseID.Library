using HelseId.Library.Interfaces.Configuration;

namespace HelseId.Library.Tests.Mocks;

public class HelseIdConfigurationGetterMock : IHelseIdConfigurationGetter
{
    private readonly HelseIdConfiguration _configuration;
    public HelseIdConfigurationGetterMock(HelseIdConfiguration configuration) {
        _configuration = configuration;
    }
    
    public Task<HelseIdConfiguration> GetConfiguration()
    {
        return Task.FromResult(_configuration);
    }
}
