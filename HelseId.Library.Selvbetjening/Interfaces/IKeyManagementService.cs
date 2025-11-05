using HelseId.Library.Selvbetjening.Models;

namespace HelseId.Library.Selvbetjening.Interfaces;

internal interface IKeyManagementService
{
    PublicPrivateKeyPair GenerateNewKeyPair();
}
