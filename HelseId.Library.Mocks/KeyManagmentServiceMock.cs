using HelseId.Library.SelfService.Interfaces;
using HelseId.Library.SelfService.Models;

namespace HelseId.Library.Mocks;

public class KeyManagmentServiceMock : IKeyManagementService
{
    public int GenerateSet { get; set; }

    public KeyManagmentServiceMock(PublicPrivateKeyPair publicPrivateKeyPair)
    {
        PublicPrivateKeyPair = publicPrivateKeyPair;
    }
    public PublicPrivateKeyPair GenerateNewKeyPair()
    {
        GenerateSet += 1;

        return PublicPrivateKeyPair;
    }

    public PublicPrivateKeyPair PublicPrivateKeyPair { get; set; }
}
