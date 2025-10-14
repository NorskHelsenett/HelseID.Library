using System.Text;
using FluentAssertions;
using HelseId.Library.Configuration;
using HelseId.Library.Exceptions;
using HelseId.Library.Mocks;
using NUnit.Framework;

namespace HelseId.Library.Selvbetjening.Tests;

[TestFixture]
public class ClientSecretEndpointTests
{
    private HelseIdClientCredentialsFlowMock _clientCredentialsFlowMock = null!;
    private DPoPProofCreatorMock _dpoPProofCreatorMock = null!;
    private HelseIdConfiguration _helseIdConfiguration = null!;

    private ClientSecretEndpoint _clientSecretEndpoint = null!;

    private const string DpopProof = "w9f8wwoehfiohwio2jioprjoir";
    private const string SelvbetjeningScope = "2934692346932468932";
    private const string UpdateClientSecretEndpoint = "https://selvbetjening/keyupdater";
    private const string AccessToken = "349857349857435oihj3o4rjeruf9u3r9uf934uf90u";
    private const string PublicKey = """
                                     {
                                         "kty": "RSA",
                                         "e": "AQAB",
                                         "use": "sig",
                                         "kid": "NMvWuCC-2evmITXoCdCMPqLJu0PYm-4diJSsdDWaUNQ",
                                         "alg": "PS256",
                                         "n": "go99vXOxXi_MYt6ycwi9_6pIMdRKtuCHtyy89nyt8a9isw0OYeWgKaQG-o_awMqve6kSNU966DwCCeCUaB140RdONRXnmHjGl_S49w4R7UvHwJMGAXsul2chTS3LJuxH-ijRikJhsTNRZ9Sv91vyeonW2Sa_5UIgAwzsRaACxZG-MKVXKjYI7BaY6iU6NVA0Y0_AhDsfUKwAtMjHZP4H412ZNUXkRKGnoyCQSga0DW-ZuCrUcjYRGxOipGCtanzsinjgq1ndtq6PKUbuAt8Xq69m1dlSbdf2HeQnmvdpy8XHE1cgh4zakKKDGpo8_FswE2smEZHDnsqfApKYe2fLjQ"
                                     }
                                     """;

    [SetUp]
    public void Setup()
    {
        _clientCredentialsFlowMock = new HelseIdClientCredentialsFlowMock(AccessToken);
        _dpoPProofCreatorMock = new DPoPProofCreatorMock(DpopProof);
        _helseIdConfiguration = new HelseIdConfiguration
        {
            ClientId = "client id", Scope = "scope", IssuerUri = "sts",
            SelvbetjeningConfiguration = new SelvbetjeningConfiguration()
            {
                SelvbetjeningScope = SelvbetjeningScope,
                UpdateClientSecretEndpoint = UpdateClientSecretEndpoint,
            }
        };
        
        _clientSecretEndpoint = new ClientSecretEndpoint(
            _clientCredentialsFlowMock,
            _dpoPProofCreatorMock,
            _helseIdConfiguration);
    }

    [Test]
    public async Task GetClientSecretRequest()
    {
        var httpRequestMessage = await _clientSecretEndpoint.GetClientSecretRequest(PublicKey);

        httpRequestMessage.Should().NotBeNull();

        var content = new StringContent(PublicKey, Encoding.UTF8, mediaType: "application/json");
        httpRequestMessage.Content.Should().BeEquivalentTo(content);

        var headersToString = httpRequestMessage.Headers.ToString();

        headersToString.Should().Contain($"Authorization: DPoP {AccessToken}");
        headersToString.Should().Contain($"DPoP: {DpopProof}");
    }

    [Test]
    public async Task GetClientSecretRequest_should_throw_when_token_response_is_token_error_response()
    {
        _clientCredentialsFlowMock.SetTokenErrorResponse = true;
        
        Func<Task> createProofWithInvalidUrl = async () => await _clientSecretEndpoint.GetClientSecretRequest(PublicKey);

        await createProofWithInvalidUrl.Should().ThrowAsync<HelseIdException>().WithMessage(_clientCredentialsFlowMock.ErrorResponse);
    }

    [Test]
    public async Task GetClientSecretRequest_sets_token_response()
    {
        await _clientSecretEndpoint.GetClientSecretRequest(PublicKey);

        _clientCredentialsFlowMock.Scope.Should().Be(SelvbetjeningScope);
    }
    
    [Test]
    public async Task GetClientSecretRequest_sets_dpop_proof()
    {
        await _clientSecretEndpoint.GetClientSecretRequest(PublicKey);

        _dpoPProofCreatorMock.Url.Should().Be(UpdateClientSecretEndpoint);
        _dpoPProofCreatorMock.HttpMethod.Should().Be("POST");
        _dpoPProofCreatorMock.AccessToken.Should().Be(AccessToken);
    }
}
