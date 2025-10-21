using System.Net.Http.Json;
using HelseId.Library.Exceptions;
using HelseId.Library.Interfaces.Configuration;
using HelseId.Library.Selvbetjening.Interfaces;
using HelseId.Library.Selvbetjening.Models;

namespace HelseId.Library.Selvbetjening;

public class SelvbetjeningSecretUpdater : ISelvbetjeningSecretUpdater
{
    private readonly IClientSecretEndpoint _clientSecretEndpoint;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IKeyManagementService _keyManagementService;

    public SelvbetjeningSecretUpdater(
        IClientSecretEndpoint clientSecretEndpoint,
        IHttpClientFactory httpClientFactory, 
        IKeyManagementService keyManagementService)
    {
        _clientSecretEndpoint = clientSecretEndpoint;
        _httpClientFactory = httpClientFactory;
        _keyManagementService = keyManagementService;
    }

    public async Task<ClientSecretResult> UpdateClientSecret()
    {
        var publicPrivateKeyPair = _keyManagementService.GenerateNewKeyPair();
        
        var httpRequest = await _clientSecretEndpoint.GetClientSecretRequest(publicPrivateKeyPair.PublicKey);

        var httpClient = _httpClientFactory.CreateClient();
        var response = await httpClient.SendAsync(httpRequest);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new HelseIdException(content, "Error from Selvbetjening");
        }

        var result = await response.Content.ReadFromJsonAsync<ClientSecretUpdateResponse>();

        return new ClientSecretResult
        {
            ExpirationDate = result!.Expiration,
            PrivateJsonWebKey = publicPrivateKeyPair.PrivateKey
        };
    }
}
