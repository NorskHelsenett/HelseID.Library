namespace HelseId.Library.Tests;

[TestFixture]
public class HttpRequestMessageExtensionsTests
{
    [Test]
    public void SetDPoPTokenAndProof_sets_Authorization_header()
    {
        var httpRequestMessage = new HttpRequestMessage();
        var accessTokenResponse = new AccessTokenResponse
        {
            AccessToken = "access token",
            ExpiresIn = 1
        };
        
        httpRequestMessage.SetDPoPTokenAndProof(accessTokenResponse, "dpopProof");

        httpRequestMessage.Headers.Authorization.Should().NotBeNull();
        httpRequestMessage.Headers.Authorization.Scheme.Should().Be("DPoP");
        httpRequestMessage.Headers.Authorization.Parameter.Should().Be("access token");
    }
    
    [Test]
    public void SetDPoPTokenAndProof_sets_DPoP_header()
    {
        var httpRequestMessage = new HttpRequestMessage();
        var accessTokenResponse = new AccessTokenResponse
        {
            AccessToken = "access token",
            ExpiresIn = 1
        };
        
        httpRequestMessage.SetDPoPTokenAndProof(accessTokenResponse, "proof");

        var hasDPoPHeader = httpRequestMessage.Headers.TryGetValues("DPoP", out var dPoPHeaderValues);
        hasDPoPHeader.Should().BeTrue();
        dPoPHeaderValues.Should().NotBeNull();
        dPoPHeaderValues.Should().HaveCount(1);
        dPoPHeaderValues.Single().Should().Be("proof");
    }
    
    [Test]
    public void Repeated_calls_to_SetDPoPTokenAndProof_should_fail()
    {
        var httpRequestMessage = new HttpRequestMessage();
        var accessTokenResponse = new AccessTokenResponse
        {
            AccessToken = "access token",
            ExpiresIn = 1
        };
        
        httpRequestMessage.SetDPoPTokenAndProof(accessTokenResponse, "proof");
        
        Action repeatedCall = () => httpRequestMessage.SetDPoPTokenAndProof(accessTokenResponse, "proof2");

        repeatedCall.Should().Throw<InvalidOperationException>().WithMessage("Authorization header is already set");
    }
}
