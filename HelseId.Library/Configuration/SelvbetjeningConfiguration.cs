namespace HelseId.Library.Configuration;

/// <summary>
/// Configuration for automatic key rotation in HelseID Selvbetjening
/// </summary>
public class SelvbetjeningConfiguration
{
    /// <summary>
    /// The url of the API endpoint
    /// </summary>
    public string UpdateClientSecretEndpoint { get; init; } = "https://api.selvbetjening.nhn.no/v1/client-secret";
    
    /// <summary>
    /// The scope required to call the HelseID Selvbetjening API 
    /// </summary>
    public string SelvbetjeningScope { get; init; } = "nhn:selvbetjening/client";
}
