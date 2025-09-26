namespace HelseId.Library.Tests.Mocks;

public class AssertionDetailsCreatorMock: IAssertionDetailsCreator
{
    public PayloadClaimParameters PayloadClaimParameters { get; set; } = null!;
    
    public PayloadClaim CreateAssertionDetails(PayloadClaimParameters payloadClaimParameters)
    {
        PayloadClaimParameters = payloadClaimParameters;
        
        return new PayloadClaim("assertion_details", "some_object_value");
    }
}
