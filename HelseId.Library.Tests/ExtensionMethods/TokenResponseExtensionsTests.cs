namespace HelseId.Library.Tests.ExtensionMethods;

[TestFixture]
public class TokenResponseExtensionsTests
{
    [Test]
    public void IsSuccessful_returns_false_when_token_response_is_not_access_token()
    {
        var tokenErrorResponse = new TokenErrorResponse()
        {
            Error = "Error",
        };

        var response = tokenErrorResponse.IsSuccessful(out var accessTokenResponse);

        accessTokenResponse.Should().NotBeNull();
        accessTokenResponse.Should().NotBeSameAs(tokenErrorResponse);
        accessTokenResponse.AccessToken.Should().BeEmpty();
        accessTokenResponse.ExpiresIn.Should().Be(0);
        accessTokenResponse.Scope.Should().BeEmpty();
        accessTokenResponse.RejectedScope.Should().BeEmpty();
        response.Should().BeFalse();
    }
    
    [Test]
    public void IsSuccessful_returns_true_when_token_response_is_access_token()
    {
        var tokenResponse = new AccessTokenResponse()
        {
            AccessToken = "sdf",
            ExpiresIn = 1,
            Scope = "scope",
            RejectedScope = "rejected scope"
        };

        var response = tokenResponse.IsSuccessful(out var accessTokenResponse);

        accessTokenResponse.Should().BeSameAs(tokenResponse);
        response.Should().BeTrue();
    }
    
    [Test]
    public void AsError_returns_token_error_response()
    {
        var tokenErrorResponse = new TokenErrorResponse()
        {
            Error = "Error",
        };

        var response = tokenErrorResponse.AsError();

        response.Should().BeSameAs(tokenErrorResponse);
    }
    
    [Test]
    public void AsError_returns_empty_response()
    {
        var tokenResponse = new AccessTokenResponse()
        {
            AccessToken = "sdf",
            ExpiresIn = 1,
            Scope = "scope"
        };

        var response = tokenResponse.AsError();

        response.Should().NotBeSameAs(tokenResponse);
    }
}
