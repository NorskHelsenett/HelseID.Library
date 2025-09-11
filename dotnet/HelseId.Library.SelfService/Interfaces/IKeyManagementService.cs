namespace HelseId.Library.SelfService.Interfaces;

public interface IKeyManagementService
{
    Task<(string PublicJwk, string PrivateJwk)> GenerateNewKeyPair();
}
