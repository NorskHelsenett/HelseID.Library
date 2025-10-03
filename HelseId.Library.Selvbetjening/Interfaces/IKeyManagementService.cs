using HelseId.Library.Selvbetjening.Models;

namespace HelseId.Library.Selvbetjening.Interfaces;

public interface IKeyManagementService
{
    PublicPrivateKeyPair GenerateNewKeyPair();
}
