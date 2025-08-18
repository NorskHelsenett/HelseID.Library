namespace HelseId.Library.Interfaces.Configuration;

public interface IHelseIdConfigurationGetter
{
    Task<HelseIdConfiguration> GetConfiguration();
}
