# HelseId.Library
The easiest way to integrate with HelseID! This library conforms to the requirements from the HelseID [security profile](https://utviklerportal.nhn.no/informasjonstjenester/helseid/protokoller-og-sikkerhetsprofil/sikkerhetsprofil/docs/sikkerhetsprofil_enmd) for the client credentials grant (also known as machine-to-machine). 

**Warning:** This is still a work in progress, so you can expect some breaking changes.

## To get started with HelseID

See the docs on https://selvbetjening.nhn.no/docs

## How to use the library:

You start by installing the NuGet package `HelseID.Library.ClientCredentials`.

The simplest setting is where the configuration is hard-coded:


```csharp
// This comes from the .NET Generic Host (https://learn.microsoft.com/en-us/dotnet/core/extensions/generic-host?tabs=appbuilder)

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
    
var helseIdConfiguration = new HelseIdConfiguration
{
    ClientId = "7e3816ca-7d11-41cd-be55-fb9e8954e058",
    Scope = "nhn:hgd-persontjenesten-api/restricted-access nhn:selvbetjening/client",
    IssuerUri = "https://helseid-sts.test.nhn.no",
};

builder.Services
    .AddHelseIdClientCredentials(helseIdConfiguration)
    .AddJwkForClientAuthentication(YOUR_PRIVATE_KEY_HERE);

var host = builder.Build();
host.Run();
// The service is now configured
```

Later on you will need to use an instance of the `IHelseIdClientCredentialsFlow` to retrieve an Access token response:

```csharp
// This is constructed by the service locator
IHelseIdClientCredentialsFlow helseIdClientCredentialsFlow;

...

var tokenResponse = await helseIdClientCredentialsFlow.GetTokenResponseAsync();

// If the token response is successful, you will get an AccessTokenResponse object:
if (tokenResponse.IsSuccessful(out var accessTokenResponse))
{
    ...
}
else
{
    // If the token response failed, you can inspect the error response from the TokenErrorResponse object:
    var errorResponse = tokenResponse.AsError();
    Console.WriteLine(errorResponse.Error + " " + errorResponse.ErrorDescription);
    ...
}

```
If you have a [multi-tenant client](https://utviklerportal.nhn.no/informasjonstjenester/helseid/bruksmoenstre-og-eksempelkode/bruk-av-helseid/docs/klientinstanser/bruk_av_multitenante_klienter_enmd/), you will probably need to setup organization numbers; see the document [Performing token requests](Documentation/ClientCredentialsUsage/token_requests.md) for this scenario.


To retrieve a [DPoP Proof](https://utviklerportal.nhn.no/informasjonstjenester/helseid/bruksmoenstre-og-eksempelkode/bruk-av-helseid/docs/dpop/dpop_enmd) you can use the `IDPoPProofCreatorForApiRequests`:

```csharp
...
// This is constructed by the service locator
IDPoPProofCreatorForApiRequests dPoPProofCreator;

var url = "URL TO THE HTTP ENDPOINT";
var dPoPProof = await dPoPProofCreator.CreateDPoPProofForApiRequest(HttpMethod.Get, url, accessTokenResponse);
...
```

Finally, to make a request to an API you can do the following: using our SetDPoPTokenAndProof extension method to set both Access Token and DPoP proof on the HTTP request:
```csharp
...
var apiRequest = new HttpRequestMessage(HttpMethod.Get, url);
apiRequest.SetDPoPTokenAndProof(accessTokenResponse, dPoPProof);

var response = await httpClient.SendAsync(apiRequest);
...
```

## Read more here:
* [Configuration](Documentation/ClientCredentialsUsage/configuration.md)
* [Handling of secrets](Documentation/ClientCredentialsUsage/secrets.md)
* [Performing token requests](Documentation/ClientCredentialsUsage/token_requests.md)
* [Using DPoP](Documentation/ClientCredentialsUsage/dpop.md)
* [Advanced setup](Documentation/ClientCredentialsUsage/advanced_setup.md)
* [Automatic key rotation](Documentation/KeyRotation/automatic_key_rotation.md)