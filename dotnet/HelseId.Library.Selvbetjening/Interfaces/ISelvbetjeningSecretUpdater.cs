namespace HelseId.Library.SelfService.Interfaces;

public interface ISelvbetjeningSecretUpdater
{
    Task<DateTime> UpdateClientSecret();
}
