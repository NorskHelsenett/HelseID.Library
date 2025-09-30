namespace HelseId.Library.Selvbetjening.Interfaces;

public interface IClientSecretEndpoint
{
    Task<HttpRequestMessage> GetClientSecretRequest(string publicKey);
}
