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
    private readonly IDPoPProofCreator _dPoPProofCreator;
    private readonly SelvbetjeningConfiguration _configuration;

    public ClientSecretEndpoint(
        IHelseIdClientCredentialsFlow clientCredentialsFlow, 
        IDPoPProofCreator dPoPProofCreator, 
        SelvbetjeningConfiguration configuration)
    {
        _clientCredentialsFlow = clientCredentialsFlow;
        _dPoPProofCreator = dPoPProofCreator;
        _configuration = configuration;
    }
    
    public async Task<HttpRequestMessage> GetClientSecretRequest(string publicKey)
    {
        var tokenResponse = await _clientCredentialsFlow.GetTokenResponseAsync(_configuration.SelvbetjeningScope);
        
        if (tokenResponse is TokenErrorResponse tokenErrorResponse)
        {
            throw new HelseIdException(tokenErrorResponse);
        }

        var accessTokenResponse = (AccessTokenResponse)tokenResponse;
        var dPopProof = await _dPoPProofCreator.CreateDPoPProofForApiCall(_configuration.UpdateClientSecretEndpoint, "POST", accessTokenResponse.AccessToken);
    
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, _configuration.UpdateClientSecretEndpoint);
        httpRequest.Content = new StringContent(publicKey, Encoding.UTF8, mediaType: "application/json");
        httpRequest.Headers.Add("Authorization", $"DPoP {accessTokenResponse.AccessToken}");
        httpRequest.Headers.Add("DPoP", dPopProof);
        return httpRequest;
    }
}
