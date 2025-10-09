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

builder.Services
    .AddHelseIdClientCredentials(helseIdConfiguration)
    .AddJwkForClientAuthentication(YOUR_PRIVATE_KEY_HERE);

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

To retrieve a DPoP Proof you can use the `IDPoPProofCreatorForApiCalls`:

```csharp
...
var url = "URL TO THE HTTP ENDPOINT";
var dPoPProof = await dPoPProofCreator.CreateDPoPProofForApiCall(url, "GET", accessTokenResponse);
...
```

Finally to make a request to an API you can do the following using our SetDPoPTokenAndProof extension method to set both Access Token and DPoP proof on the http request:
```csharp
...
var apiRequest = new HttpRequestMessage(HttpMethod.Get, url);
apiRequest.SetDPoPTokenAndProof(accessTokenResponse, dPoPProof);

var response = await httpClient.SendAsync(apiRequest);
...
```
