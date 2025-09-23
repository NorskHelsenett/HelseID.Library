using System.Net.Http.Json;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using HelseId.Library.ClientCredentials.Interfaces;
using HelseId.Library.Exceptions;
using HelseId.Library.Interfaces.Configuration;
using HelseId.Library.Interfaces.JwtTokens;
using HelseId.Library.Models;
using HelseId.Library.SelfService.Configuration;
using HelseId.Library.SelfService.Interfaces;
using HelseId.Library.SelfService.Models;

namespace HelseId.Library.SelfService;

public class SelvbetjeningSecretUpdater : ISelvbetjeningSecretUpdater
{
    private readonly ISigningCredentialReference _signingCredentialReference;
    private readonly IClientSecretEndpoint _clientSecretEndpoint;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IKeyManagementService _keyManagementService;

    public SelvbetjeningSecretUpdater(
        ISigningCredentialReference signingCredentialReference,
        IClientSecretEndpoint clientSecretEndpoint,
        IHttpClientFactory httpClientFactory, 
        IKeyManagementService keyManagementService)
    {
        _signingCredentialReference = signingCredentialReference;
        _clientSecretEndpoint = clientSecretEndpoint;
        _httpClientFactory = httpClientFactory;
        _keyManagementService = keyManagementService;
    }

    public async Task<DateTime> UpdateClientSecret()
    {
        var publicPrivateKeyPair = _keyManagementService.GenerateNewKeyPair();
        
        var httpRequest = await _clientSecretEndpoint.GetClientSecretRequest(publicPrivateKeyPair.PublicKey);

        var httpClient = _httpClientFactory.CreateClient();
        var response = await httpClient.SendAsync(httpRequest);
        
        
        if(!response.IsSuccessStatusCode){
            var content = await response.Content.ReadAsStringAsync();
            throw new HelseIdException(content, "Error from Selvbetjening");
        }
        var result = await response.Content.ReadFromJsonAsync<ClientSecretUpdateResponse>();
        await _signingCredentialReference.UpdateSigningCredential(publicPrivateKeyPair.PrivateKey);
        return result!.Expiration;
        
        //return new DateTime();
    }
}
