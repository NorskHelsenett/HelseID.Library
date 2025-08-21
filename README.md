# HelseId.Library
The easiest way to integrate with HelseID! This library conforms to the requirements from the HelseID security profile for the machine-to-machine flow. 

**Warning:** This is still a work in progress, so do not use it in production. You can expect major breaking changes with each update.



## How to use:
The library follows the traditional service builder-pattern that is used in most .NET applications.

```csharp
// Read configuration variables from the HelseID-section in appsettings.json
var helseIdConfiguration = HelseIdConfiguration.ConfigurationFromAppSettings();
// Setup a client for machine to machine
services.AddHelseIdMachineToMachine(helseIdConfiguration);
```

Then later you will use an instance of the `IHelseIdMachineToMachineFlow` to retrieve an access token:
```csharp
var accessToken = await helseIdMachineToMachineFlow.GetAccessToken();
```
