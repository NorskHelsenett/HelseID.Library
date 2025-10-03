using System.Text;
using FluentAssertions;
using HelseId.Library.Exceptions;
using HelseId.Library.Mocks;
using HelseId.Library.Selvbetjening.Models;
using HelseId.Library.Tests.Mocks;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace HelseId.Library.Selvbetjening.Tests;

[TestFixture]
public class SelvbetjeningSecretUpdaterTests
{
    private SelvbetjeningSecretUpdater _selvbetjeningSecretUpdater = null!;
    private SigningCredentialsReferenceMock _signingCredentialsReferenceMock = null!;
    private ClientSecretEndpointMock _clientSecretEndpointMock = null!;
    private HttpClientFactoryMock _httpClientFactoryMock = null!;
    private KeyManagmentServiceMock _keyManagmentServiceMock = null!;
    private const string KeyUpdater = "https://selvbetjening/keyupdater";
    private PublicPrivateKeyPair _publicPrivateKeyPair = null!;
    
    [SetUp]
    public void Setup()
    {
        var mockHttpMessageHandler = new MockHttpMessageHandlerWithCount();
        mockHttpMessageHandler
            .When(KeyUpdater)
            .Respond(new StringContent("{\"expiration\": \"2019-08-24T14:15:22Z\"}", Encoding.UTF8, "application/json"));
        
        _signingCredentialsReferenceMock = new SigningCredentialsReferenceMock();
        _httpClientFactoryMock = new HttpClientFactoryMock(mockHttpMessageHandler);
        _publicPrivateKeyPair = new PublicPrivateKeyPair
        {
            PublicKey = "123123",
            PrivateKey = "456456"
        };
        _keyManagmentServiceMock = new KeyManagmentServiceMock(_publicPrivateKeyPair);
        _clientSecretEndpointMock = new ClientSecretEndpointMock(KeyUpdater);
        
        _selvbetjeningSecretUpdater = new SelvbetjeningSecretUpdater(
            _signingCredentialsReferenceMock,
            _clientSecretEndpointMock,
            _httpClientFactoryMock,
            _keyManagmentServiceMock);
    }
    
    [Test]
    public async Task UpdateClientSecret_gets_generate_new_keypair() {
        
        await _selvbetjeningSecretUpdater.UpdateClientSecret();

        _keyManagmentServiceMock.GenerateSet.Should().Be(1);
    }

    [Test]
    public async Task UpdateClientSecret_calls_httpclientfactory_createclient()
    {
        await _selvbetjeningSecretUpdater.UpdateClientSecret();
        
        _httpClientFactoryMock.RequestCount.Should().Be(1);
    }


    [Test]
    public async Task UpdateClientSecret_calls_httpclientfactory_updateclientsecret()
    {
        await _selvbetjeningSecretUpdater.UpdateClientSecret();
        
        _signingCredentialsReferenceMock.Jwk.Should().NotBeEmpty();
        
    }


    [Test]
    public async Task UpdateClientSecret_calls_clientsecretendpoint()
    {
        await _selvbetjeningSecretUpdater.UpdateClientSecret();
        
        _clientSecretEndpointMock.PublicKey.Should().Be(_publicPrivateKeyPair.PublicKey);
    }

    [Test]
    public void UpdateClientSecret_throws_HelseIdException_when_updateclientsecret_response_is_not_success()
    {
        var mockHttpMessageHandler = new MockHttpMessageHandlerWithCount();
        
        mockHttpMessageHandler
            .When(KeyUpdater)
            .Respond(System.Net.HttpStatusCode.BadRequest, "application/json", "{\"error\": \"invalid_request\"}");
        
        var httpClientFactoryMock = new HttpClientFactoryMock(mockHttpMessageHandler);
    
        var selvbetjeningSecretUpdater = new SelvbetjeningSecretUpdater(
            _signingCredentialsReferenceMock,
            _clientSecretEndpointMock,
            httpClientFactoryMock,
            _keyManagmentServiceMock);
    
        var exception = Assert.ThrowsAsync<HelseIdException>(async () => await selvbetjeningSecretUpdater.UpdateClientSecret());
        exception!.Message.Should().Be("{\"error\": \"invalid_request\"}");
        exception.Error.Should().Be("Error from Selvbetjening");
    }
}
