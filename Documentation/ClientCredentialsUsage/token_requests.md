# Requesting Access Tokens using HelseID.Library
Currently HelseID.Library only supports the Client Credentials grant, the Authorization Code grant will be released in a future version. 

When requesting an Access Token the library always returns a `TokenResponse` object:

```csharp
public abstract class TokenResponse
{
    public string RawResponse { get; set; } = "";
}

var tokenResponse = await helseIdClientCredentialsFlow.GetTokenResponseAsync();
```


## Passing scopes

Per default the library will request an Access Token containing all scopes from the initial configuration you provided. You can request a more narrow token by specifying one or several scopes in the request:

```csharp
var tokenResponse = await helseIdClientCredentialsFlow.GetTokenResponseAsync("scope1 scope2");
```

## Submitting organization numbers

The library also handles the submitting of organization information in the token request. You can always submit [a parent unit and a child unit](https://utviklerportal.nhn.no/informasjonstjenester/helseid/bruksmoenstre-og-eksempelkode/bruk-av-helseid/docs/tekniske-mekanismer/organisasjonsnumre_enmd) to the library, when using the Single-Tenant setup only the child unit will be used:

```csharp
var organizationNumbers = new OrganizationNumbers
{
    ParentOrganization = "parent", 
    ChildOrganization = "child"
};

var tokenResponse = await _helseIdClientCredentialsFlow.GetTokenResponseAsync(organizationNumbers);
```


## Handling the response from the Token endpoint
The TokenResponse will always be an instance of *either* `AccessTokenResponse` *or* `TokenErrorResponse`:

```csharp
public class AccessTokenResponse : TokenResponse
{
    public string AccessToken { get; init; }
    public int ExpiresIn { get; init; }
    public string Scope { get; set; }
    public string RejectedScope { get; set; }
}

public class TokenErrorResponse : TokenResponse
{
    public string Error { get; set; }
    public string ErrorDescription { get; set; } = "";
}
```

The `AccessTokenResponse` type has parameters matchig the relevant return values for the Client Credentials flow. The `RejectedScope` parameter is only set when using a multi-tenant client representing an organization that has not signed the required terms of use. More details can be found in the [NHN Utviklerportal](https://utviklerportal.nhn.no/informasjonstjenester/helseid/bruksmoenstre-og-eksempelkode/bruk-av-helseid/docs/teknisk-referanse/endepunkt/token-endepunktet_enmd).

To check if a Client Credentials token request was successful, you either check the type of the `TokenResponse` object:


```csharp
var tokenResponse = await helseIdClientCredentialsFlow.GetTokenResponseAsync();

if(tokenResponse is AccessTokenResponse accessTokenResponse) 
{
    // Act on successful response
}
else 
{
    var tokenErrorResponse = (TokenErrorResponse)tokenResponse;
    // Act on error response
}
```

Or you can use the supplied extension methods:

```csharp
if (tokenResponse.IsSuccessful(out var accessTokenResponse))
{
    // Act on successful response
}
else
{
    var errorResponse = tokenResponse.AsError();
    // Act on error response
    ...
}

```


