namespace HelseId.Library.SelfService;

public interface ISelvbetjeningSecretUpdater
{
    Task<DateTime> UpdateClientSecret();
}
