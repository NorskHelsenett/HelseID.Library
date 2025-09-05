namespace HelseId.Library.Interfaces.Configuration;

public interface ISigningCredentialReference
{
    Task<SigningCredentials> GetSigningCredentialReference();
}
