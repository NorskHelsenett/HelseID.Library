namespace HelseId.Library.Interfaces.Configuration;

public interface ISigningCredentialReference
{
    Task<SigningCredentials> GetSigningCredential();
    Task UpdateSigningCredential(string jsonWebKey);
    
}
