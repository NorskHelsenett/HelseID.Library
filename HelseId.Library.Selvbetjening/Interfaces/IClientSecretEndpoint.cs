namespace HelseId.Library.SelfService.Interfaces;

public interface IClientSecretEndpoint
{
    Task<HttpRequestMessage> GetClientSecretRequest(string publicKey);
}
