using FluentAssertions;
using HelseID.Standard.Services.JwtTokens;

namespace HelseID.Standard.Tests.Services.JwtTokens;

public class JtiClaimCreatorTests
{
    [Test]
    public void CreateJti_sets_different_claims()
    {
        var jtiClaimCreator = new JtiClaimCreator();

        var number1 = jtiClaimCreator.CreateJti();
        var number2 = jtiClaimCreator.CreateJti();
        number1.Should().NotBeNullOrEmpty();
        number1.Should().NotBe(number2);
    }
}
