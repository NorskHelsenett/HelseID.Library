# Advanced setup of HelseID.Library
You can set up the library by using extension methods on the IServiceCollection interface:


```csharp
IServiceCollection services; // From the generic host builder

// The most basic setup is done by registering your configuration 
// and then registering the private key used for client authentication
services 
    .AddHelseIdClientCredentials(helseIdConfiguration)
    .AddJwkForClientAuthentication(YOUR_PRIVATE_KEY_HERE);

...

// With the service locator in place you can register a dependency to a 
// IHelseIdClientCredentialsFlow that will handle all integration with HelseID
IHelseIdClientCredentialsFlow helseIdClientCredentialsFlow; // From DI

var tokenResponse = await helseIdClientCredentialsFlow.GetTokenResponseAsync();

```

The library includes support for both [single-tenant and multi-tenant](https://selvbetjening.nhn.no/docs#multitenancy) setups:
```csharp
services 
    .AddHelseIdClientCredentials(helseIdConfiguration)
    .AddJwkForClientAuthentication(YOUR_PRIVATE_KEY_HERE)
    .AddHelseIdSingleTenant(); // This is the default behavior

// OR

services 
    .AddHelseIdClientCredentials(helseIdConfiguration)
    .AddJwkForClientAuthentication(YOUR_PRIVATE_KEY_HERE)
    .AddHelseIdMultiTenant();

```

The library handles caching of tokens and metadata, either in memory, or using [a distributed cache](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/distributed). Be aware that a distributed cache needs a setup by your application:

```csharp
services 
    .AddHelseIdClientCredentials(helseIdConfiguration)
    .AddJwkForClientAuthentication(YOUR_PRIVATE_KEY_HERE)
    .AddHelseIdInMemoryCaching(); // This is the default behavior

// OR

services 
    .AddHelseIdClientCredentials(helseIdConfiguration)
    .AddJwkForClientAuthentication(YOUR_PRIVATE_KEY_HERE)
    .AddHelseIdDistributedCaching();

```



