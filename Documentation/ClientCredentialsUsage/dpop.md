# Using DPoP with HelseID.Library

The library handles creation of DPoP proofs using the `IDPoPProofCreatorForApiRequests` interface:

```csharp
IDPoPProofCreatorForApiRequests dPopProofCreator = ...;

var dPopProof = await dPopProofCreator.CreateDPoPProofForApiRequest(httpMethod, url, accessTokenResponse);
```

To perform a Http request with the DPoP proof header and the Authorization header set you can either use the supplied extension method for the `HttpRequestMessage` class or you can set the headers manually:
```csharp
...
var apiRequest = new HttpRequestMessage(HttpMethod.Get, url);
apiRequest.SetDPoPTokenAndProof(accessTokenResponse, dPoPProof);

var response = await httpClient.SendAsync(apiRequest);
...
```

Or set the headers manually
```csharp
// Either directly on the HttpClient:
var httpClient = new HttpClient();
httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("DPoP", accessTokenResponse.AccessToken);
httpClient.DefaultRequestHeaders.Add("DPoP", dpopProof);

// Or on the HttpRequestMessage:
var apiRequest = new HttpRequestMessage();
apiRequest.Headers.Authorization = new AuthenticationHeaderValue("DPoP", accessTokenResponse.AccessToken);
apiRequest.Headers.Add("DPoP", dpopProof);
```
