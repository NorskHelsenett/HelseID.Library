# Configuration of HelseID.Library
When setting up HelseID.Library you must register a configuration with the basic setup of the client:

```csharp
public class HelseIdConfiguration
{
    public required string ClientId { get; init; }
    public required string Scope { get; init; }
    public required string IssuerUri { get; init; } = "https://helseid-sts.nhn.no";
    public SelvbetjeningConfiguration SelvbetjeningConfiguration { get; set; } = new(); 
}

public class SelvbetjeningConfiguration
{
    public string UpdateClientSecretEndpoint { get; init; } = "https://api.selvbetjening.nhn.no/v1/client-secret";
    public string SelvbetjeningScope { get; init; } = "nhn:selvbetjening/client";
}

```

The configuration can either be constructed manually:

```csharp
var helseIdConfiguration = new HelseIdConfiguration
{
    ClientId = "client id",
    Scope = "nhn:helseid-testapi/api nhn:selvbetjening/client",
    IssuerUri = "https://helseid-sts.test.nhn.no",
};
```

Or it can be setup from your configuration:

```csharp
var configurationSection = configuration.GetSection("HelseID");
var helseIdConfiguration = HelseIdConfiguration.ConfigurationFromAppSettings(configurationSection);
```

With the configuration in place you can setup the Client Credentials-flow:


```csharp
builder.Services.AddHelseIdClientCredentials(helseIdConfiguration);
```

Handling of key pairs used for client authentication [is described here](secrets.md).