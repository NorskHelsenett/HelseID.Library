using Microsoft.Extensions.Configuration;

namespace HelseId.Library.Configuration;

/// <summary>
/// This class contains configurations that correspond to clients in HelseID.
/// </summary>
public class HelseIdConfiguration
{
    public string ClientId { get; private set; }
    public string Scope { get; private set; }
    public string StsUrl { get; private set; }
    public string MetadataUrl { get; }
    
    public SelvbetjeningConfiguration SelvbetjeningConfiguration { get; set; } 
    
    // These are used for clients that are using resource indicators against the PAR and token endpoints:
    public List<string> ResourceIndicators { get; private set; } = new();    
    
    /// <summary>
    /// Creates a HelseIdConfiguration object from the given configuration section in appsettings.json
    /// </summary>
    /// <param name="configurationSection">The configuration section</param>
    /// <returns>Returns a HelseIdConfiguration object</returns>
    public static HelseIdConfiguration ConfigurationFromAppSettings(IConfigurationSection configurationSection)
    {
        var clientId = configurationSection.GetValue<string>("ClientId")!;
        var stsUrl = configurationSection.GetValue<string>("StsUrl")!;
        var scope = configurationSection.GetValue<string>("Scope")!;
 
        return new HelseIdConfiguration(clientId, scope, stsUrl);
    }
    
    /// <summary>
    /// Creates a HelseIdConfiguration object from the HelseID section of the given configuration 
    /// </summary>
    /// <returns>Returns a HelseIdConfiguration object</returns>
    public static HelseIdConfiguration ConfigurationFromAppSettings(IConfiguration configuration)
    {
        var configurationSection = configuration.GetSection("HelseID");
        return ConfigurationFromAppSettings(configurationSection); 
    }
 
    public HelseIdConfiguration(
         string clientId,
         string scope,
         string stsUrl,
         List<string>? resourceIndicators = null)
     {
         ClientId = clientId;
         Scope = scope;
         StsUrl = stsUrl;
  
         MetadataUrl = MetadataUrlFromStsUrl(stsUrl);
 
         if (resourceIndicators != null)
         {
             ResourceIndicators = resourceIndicators;
         }
     }
 
     private static string MetadataUrlFromStsUrl(string stsUrl)
     {
         var metadataUrl = stsUrl;
         if (metadataUrl.EndsWith('/'))
         {
             metadataUrl = metadataUrl.TrimEnd('/');
         }
         metadataUrl += "/.well-known/openid-configuration";
 
         return metadataUrl;
     }
}
