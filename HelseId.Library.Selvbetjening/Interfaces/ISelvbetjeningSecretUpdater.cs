using HelseId.Library.Selvbetjening.Models;

namespace HelseId.Library.Selvbetjening.Interfaces;

public interface ISelvbetjeningSecretUpdater
{
    /// <summary>
    /// Registers a new signing key using the Selvbetjening API.
    /// Storage of the new key must be handled by the client by implementing
    /// the ISigningCredentialReference.UpdateSigningCredential method.
    /// </summary>
    /// <returns>Returns a result containing the newly registered private key and its expiration date as a DateTime</returns>
    Task<ClientSecretResult> UpdateClientSecret();
}
