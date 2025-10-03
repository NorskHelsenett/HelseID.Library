# HelseId.Library
The easiest way to integrate with HelseID! This library conforms to the requirements from the HelseID security profile for the machine-to-machine flow. 

**Warning:** This is still a work in progress, so do not use it in production. You can expect major breaking changes with each update.



## How to use:
A simple setting where the configuration is hard-coded:

```csharp
    // This comes from the .NET Generic Host (https://learn.microsoft.com/en-us/dotnet/core/extensions/generic-host?tabs=appbuilder)
    HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
    
    var helseIdConfiguration = new HelseIdConfiguration
    {
        ClientId = "7e3816ca-7d11-41cd-be55-fb9e8954e058",
        Scope = "nhn:hgd-persontjenesten-api/restricted-access nhn:selvbetjening/client",
        StsUrl = "https://helseid-sts.test.nhn.no",
    };

// Read configuration variables from the HelseID-section in appsettings.json

var helseIdConfiguration = HelseIdConfiguration.ConfigurationFromAppSettings();
// Setup a client for machine to machine
services.AddHelseIdMachineToMachine(helseIdConfiguration);
```

Then later you will use an instance of the `IHelseIdMachineToMachineFlow` to retrieve an access token:
```csharp
var accessToken = await helseIdMachineToMachineFlow.GetAccessToken();
```
