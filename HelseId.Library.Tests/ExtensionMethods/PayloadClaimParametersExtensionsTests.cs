namespace HelseId.Library.Tests.ExtensionMethods;

[TestFixture]
public class PayloadClaimParametersExtensionsTests
{
    [TestCase(false, false, true)]
    [TestCase(false, true, false)]
    [TestCase(true, false, false)]
    [TestCase(false, true, true)]
    [TestCase(true, false, true)]
    [TestCase(true, true, false)]
    [TestCase(true, true, true)]
    public void HasUseOfClientDetails_sets_true_when_at_least_one_parameter_is_set(bool useOrganizationNumbers, bool useSfmId, bool useTillitsrammeverk)
    {
        var payloadClaimParameters = new PayloadClaimParameters()
        {
            UseOrganizationNumbers = useOrganizationNumbers,
            UseSfmId = useSfmId,
            UseTillitsrammeverk = useTillitsrammeverk,
        };
        
        var result = payloadClaimParameters.HasUseOfClientDetails();

        result.Should().BeTrue();
    }
    
    [Test]
    public void HasUseOfClientDetails_sets_false_when_no_parameters_are_set()
    {
        var payloadClaimParameters = new PayloadClaimParameters();
        
        var result = payloadClaimParameters.HasUseOfClientDetails();

        result.Should().BeFalse();
    }

    [Test]
    public void ParametersCheck_throws_if_invalid_parameters_are_set()
    {
        var payloadClaimParameters = new PayloadClaimParameters
        {
            UseClientDetailsInRequestObject = true
        };

        Action parametersCheck = () => payloadClaimParameters.ParametersCheck();
        
        parametersCheck.Should().Throw<PayloadClaimParametersException>().WithMessage("Both UseClientDetailsInClientAssertion and UseClientDetailsInRequestObject cannot be set.");
    }
}
