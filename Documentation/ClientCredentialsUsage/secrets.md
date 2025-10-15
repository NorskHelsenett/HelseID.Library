# Handling of signing keys in HelseID.Library
HelseID requires use of a signing key to do [client authentication](https://utviklerportal.nhn.no/informasjonstjenester/helseid/bruksmoenstre-og-eksempelkode/bruk-av-helseid/docs/tekniske-mekanismer/bruk_av_client_assertion_enmd). The public key is registered via HelseID Selvbetjening while the private key is protected and is **only accessible** for use by the application itself.

HelseID.Library builds on the [SigningCredentials](https://learn.microsoft.com/en-us/dotnet/api/microsoft.identitymodel.tokens.signingcredentials) abstraction in .NET, any implementation should be usable via the `AddSigningCredentialForClientAuthentication` method:

```csharp
var signingCredential = ...;
services 
    .AddHelseIdClientCredentials(helseIdConfiguration)
    .AddSigningCredentialForClientAuthentication(signingCredential);
```

The library supports more convenient methods for the most common use cases:

## Registering a JWK stored in a string

```csharp
var jwkPrivateKey = "PRIVATE KEY IN JWK FORMAT";
services 
    .AddHelseIdClientCredentials(helseIdConfiguration)
    .AddJwkForClientAuthentication(jwkPrivateKey);
```


## Registering a JWK stored in a file in the local file system

```csharp
var privateKeyFileName = "jwk file name";
services 
    .AddHelseIdClientCredentials(helseIdConfiguration)
    .AddJwkFileForClientAuthentication(privateKeyFileName);
```


## Registering a private key stored in an X509Certificate

```csharp
// The certificate must contain an accessible private key
var certificate = x509CertificateInstance;
var algorithm = "PS256";
services 
    .AddHelseIdClientCredentials(helseIdConfiguration)
    .AddX509CertificateForForClientAuthentication(certificate, algorithm);
```




