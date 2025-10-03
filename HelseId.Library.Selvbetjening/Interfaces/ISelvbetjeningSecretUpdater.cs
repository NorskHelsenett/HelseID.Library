namespace HelseId.Library.Selvbetjening.Interfaces;

public interface ISelvbetjeningSecretUpdater
{
    Task<DateTime> UpdateClientSecret();
}
