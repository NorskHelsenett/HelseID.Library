using HelseId.Library.SelfService.Models;

namespace HelseId.Library.SelfService.Interfaces;

public interface IKeyManagementService
{
    PublicPrivateKeyPair GenerateNewKeyPair();
}
