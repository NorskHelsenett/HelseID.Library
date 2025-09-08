# Client tools
The client tools are useful for preparing authenticated requests to a Resource Server.
Examples of resources requiring authentication with HelseID is found at [utviklerportal.nhn.no](https://utviklerportal.nhn.no/).

## Installation
```xml
<dependency>
    <groupId>no.helseid</groupId>
    <artifactId>client</artifactId>
</dependency>
```

## Getting started

### Configuration of a client
All clients are identified by a client id alongside a client assertion signed by a private key only known by the client.
Relevant information is passed as a `Client` object.

```java

// NB: Load your private key from a secure storage
KeyReference myPrivateKeyReference = JWKKeyReference.parse("""
  {"alg":"PS256","d":"...","dp":"...","dq":"...","e":"...","key_ops":["sign"],"kty":"RSA","n":"...","p":"...","q":"...","qi":"...","kid":"..."}
""");

// Configure your client
Client client = new Client("my-client-id", myPrivateKeyReference, List.of("nhn:helseid/scope1", "nhn:helseid/scope2"));
```

### Client Credentials Grant
The client credentials grant is suited for authentication where no user is involved.

```java
Client client = new Client(CLIENT_ID, myPrivateKeyReference, SCOPE);
DPoPProofCreator dPoPProofCreator = DPoP.getDPoPProofCreator(client.keyReference());
ClientCredentials clientCredentials = new ClientCredentials.Builder(AUTHORITY)
    .withClient(client)
    .build();

TokenResponse tokenResponse = clientCredentials.getAccessToken();

if (tokenResponse instanceof ErrorResponse errorResponse) {
  // Handle error
}

if (tokenResponse instanceof AccessTokenResponse accessTokenResponse) {
    URI resourceServer = URI.create("https://api.selvbetjening.test.nhn.no/v1/client/");
    String dPoPProof = dPoPProofCreator.createDPoPProof(resourceServer, "GET", accessTokenResponse.accessToken);
    
    // Perform request to resourceServer
}
```
An example implementation for both [single-tenant](../examples/src/main/java/no/helseid/ClientCredentialsSingleTenantExample.java) and [multi-tenant](../examples/src/main/java/no/helseid/ClientCredentialsMultiTenantExample.java) is found in the examples module.

### Token Exchange Grant
Not yet implemented, only suitable in combination with a resource server