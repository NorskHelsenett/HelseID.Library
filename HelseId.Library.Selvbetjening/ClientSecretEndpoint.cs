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
    private readonly IDPoPProofCreatorForApiCalls _dPoPProofCreator;
    private readonly SelvbetjeningConfiguration _selvbetjeningConfiguration;

    public ClientSecretEndpoint(
        IHelseIdClientCredentialsFlow clientCredentialsFlow, 
        IDPoPProofCreatorForApiCalls dPoPProofCreator, 
        HelseIdConfiguration helseIdConfiguration)
    {
        _clientCredentialsFlow = clientCredentialsFlow;
        _dPoPProofCreator = dPoPProofCreator;
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
        var dPopProof = await _dPoPProofCreator.CreateDPoPProofForApiCall(_selvbetjeningConfiguration.UpdateClientSecretEndpoint, "POST", accessTokenResponse.AccessToken);
    
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, _selvbetjeningConfiguration.UpdateClientSecretEndpoint);
        httpRequest.Content = new StringContent(publicKey, Encoding.UTF8, mediaType: "application/json");
        httpRequest.Headers.Add("Authorization", $"DPoP {accessTokenResponse.AccessToken}");
        httpRequest.Headers.Add("DPoP", dPopProof);
        return httpRequest;
    }
}
