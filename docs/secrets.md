# Handling of signing keys in HelseID.Library
HelseID requires use of a signing key to do client authentication. The public key is registered via HelseID Selvbetjening while the private key is protected and only accessible for use the application itself.

HelseID.Library builds on the `SigningCredentials` abstraction in .NET, any implementation should be usable via the `AddSigningCredentialForClientAuthentication` method:

```csharp
var signingCredential = ...;
services 
    .AddHelseIdClientCredentials(helseIdConfiguration)
    .AddSigningCredentialForClientAuthentication(signingCredential);
```

The library supports more convenient methods for the most common use cases.

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




