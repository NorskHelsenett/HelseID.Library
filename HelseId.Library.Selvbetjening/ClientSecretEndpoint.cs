using System.Text;
using HelseId.Library.ClientCredentials.Interfaces;
using HelseId.Library.Configuration;
using HelseId.Library.Exceptions;
using HelseId.Library.Interfaces.JwtTokens;
using HelseId.Library.Models;
using HelseId.Library.Selvbetjening.Interfaces;

namespace HelseId.Library.Selvbetjening;

public class ClientSecretEndpoint : IClientSecretEndpoint
{
    private readonly IHelseIdClientCredentialsFlow _clientCredentialsFlow;
    private readonly IDPoPProofCreatorForApiRequests _idPoPProofCreator;
    private readonly SelvbetjeningConfiguration _selvbetjeningConfiguration;

    public ClientSecretEndpoint(
        IHelseIdClientCredentialsFlow clientCredentialsFlow, 
        IDPoPProofCreatorForApiRequests idPoPProofCreator, 
        HelseIdConfiguration helseIdConfiguration)
    {
        _clientCredentialsFlow = clientCredentialsFlow;
        _idPoPProofCreator = idPoPProofCreator;
        _selvbetjeningConfiguration = helseIdConfiguration.SelvbetjeningConfiguration;
    }
    
    public async Task<HttpRequestMessage> GetClientSecretRequest(string publicKey)
    {
        var tokenResponse = await _clientCredentialsFlow.GetTokenResponseAsync(_selvbetjeningConfiguration.SelvbetjeningScope);
        
        if (tokenResponse is TokenErrorResponse tokenErrorResponse)
        {
            throw new HelseIdException(tokenErrorResponse);
        }

        var accessTokenResponse = (AccessTokenResponse)tokenResponse;
        var dPopProof = await _idPoPProofCreator.CreateDPoPProofForApiRequest(
            HttpMethod.Post, 
            _selvbetjeningConfiguration.UpdateClientSecretEndpoint, 
            accessTokenResponse);
    
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, _selvbetjeningConfiguration.UpdateClientSecretEndpoint);
        httpRequest.Content = new StringContent(publicKey, Encoding.UTF8, mediaType: "application/json");
        httpRequest.Headers.Add("Authorization", $"DPoP {accessTokenResponse.AccessToken}");
        httpRequest.Headers.Add("DPoP", dPopProof);
        return httpRequest;
    }
}
