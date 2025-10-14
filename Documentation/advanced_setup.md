# Advanced setup of HelseID.Library
Configuration of the library is normally done using extension methods on the IServiceCollection interface:


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

The library includes support for both single-tenant and multi-tenant setups:
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

The library handles caching of tokens and metadata, either in memory of the process or using a distributed cache. Be aware that a distributed cache must be setup elsewhere in your application:

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



