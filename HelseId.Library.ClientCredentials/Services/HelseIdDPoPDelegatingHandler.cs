using HelseId.Library.Exceptions;

namespace HelseId.Library.ClientCredentials.Services;

internal class HelseIdDPoPDelegatingHandler : DelegatingHandler
{
    private readonly IHelseIdClientCredentialsFlow _helseIdClientCredentialsFlow;
    private readonly IDPoPProofCreatorForApiRequests _idPoPProofCreator;
    private readonly string _scope;

    public HelseIdDPoPDelegatingHandler(
        IHelseIdClientCredentialsFlow helseIdClientCredentialsFlow,
        IDPoPProofCreatorForApiRequests idPoPProofCreator,
        string scope)
    {
        _helseIdClientCredentialsFlow = helseIdClientCredentialsFlow;
        _idPoPProofCreator = idPoPProofCreator;
        _scope = scope;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.RequestUri == null)
        {
            return await base.SendAsync(request, cancellationToken);
        }

        var tokenResponse = await _helseIdClientCredentialsFlow.GetTokenResponseAsync(_scope);

        if (tokenResponse.IsSuccessful(out var accessTokenResponse))
        {
            var dpopProof = await _idPoPProofCreator.CreateDPoPProofForApiRequest(
                request.Method.ToString(),
                request.RequestUri.ToString(),
                accessTokenResponse);

            request.SetDPoPTokenAndProof(accessTokenResponse, dpopProof);
        }
        else
        {
            var errorResponse = tokenResponse.AsError();
            throw new HelseIdException(errorResponse);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
