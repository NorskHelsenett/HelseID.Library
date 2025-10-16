namespace HelseId.Library.Selvbetjening.Interfaces;

public interface ISelvbetjeningSecretUpdater
{
    /// <summary>
    /// Registers a new signing key using the Selvbetjening API.
    /// Storage of the new key must be handled by the client by implementing
    /// the ISigningCredentialReference.UpdateSigningCredential method.
    /// </summary>
    /// <returns>Returns the expiration date of the new signing key.</returns>
    Task<DateTime> UpdateClientSecret();
}
