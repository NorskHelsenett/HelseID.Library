namespace HelseID.Standard.Models.Payloads;

public class PayloadClaimParameters
{
    public bool UseOrganizationNumbers { get; set; }
    
    public bool UseSfmId { get; set; }

    public bool UseTillitsrammeverk { get; set; }
    
    public bool UseRequestObjects { get; set; }

    /// <summary>
    /// This parameter sets the usage of assertion_details (organization numbers,
    /// SFM ID for multi-tenant clients, and Tillitsrammeverk) in client assertions.
    /// You cannot (at present) use both UseClientDetailsInClientAssertion and UseClientDetailsInRequestObject
    /// parameters.
    /// </summary>
    public bool UseClientDetailsInClientAssertion { get; set; } = true;
    
    /// <summary>
    /// This parameter sets the usage of authorization_details (organization numbers,
    /// SFM ID for multi-tenant clients, and Tillitsrammeverk) in request objects.
    /// You cannot (at present) use both UseClientDetailsInClientAssertion and UseClientDetailsInRequestObject
    /// parameters.
    /// </summary>
    public bool UseClientDetailsInRequestObject { get; set; }
    
    public string ParentOrganizationNumber { get; set; } = string.Empty;
    public string ChildOrganizationNumber { get; set; } = string.Empty;
    
    public string SfmJournalId { get; set; } = string.Empty;

    public string TokenType { get; set; } = "client-authentication+jwt";
}
