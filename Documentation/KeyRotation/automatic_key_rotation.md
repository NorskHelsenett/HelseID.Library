# Setting up automatic key rotation using the Selvbetjening API

The package `HelseId.Library.Selvbetjening` allows your client to automatically rotate the signing keys used for client authentication.

The setup is done by calling the `AddSelvbetjeningKeyRotation()` method:

```csharp
services.AddHelseIdClientCredentials(helseIdConfiguration)
        .AddSelvbetjeningKeyRotation()
```

Configuration is done by modifying the SelvbetjeningConfiguration property of the HelseIdConfiguration object. Note that your client must have access to the `nhn:selvbetjening/client` scope to access the API:

```csharp
var helseIdConfiguration = // setup earlier

var selvbetjeningConfiguration = new SelvbetjeningConfiguration 
{
    UpdateClientSecretEndpoint = "url for testing";
}

helseidConfiguration.SelvbetjeningConfiguration = selvbetjeningConfiguration;
```


Key rotation is done by calling the `ISelvbetjeningSecretUpdater.UpdateClientSecret()` method:

```csharp
ISelvbetjeningSecretUpdater secretUpdater = // from the service locator

var clientSecretResult = await secretUpdater.UpdateClientSecret();
```

Finally you will use the ClientSecretResult to update your local system with the new private key:

```csharp
// The new private key in Json Web Key format
// This must be stored and made available for your client
var newPrivateKey = clientSecretResult.PrivateJsonWebKey;

// The expiration date for the new private key
// The next key rotation must be performed before this date 
var expirationDate = clientSecretResult.ExpirationDate;
```
