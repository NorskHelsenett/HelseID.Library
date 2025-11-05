namespace HelseId.Library.Selvbetjening.Interfaces;

internal interface IClientSecretEndpoint
{
    Task<HttpRequestMessage> GetClientSecretRequest(string publicKey);
}
