using System.Text.Json.Serialization;

namespace HelseID.Standard.Models;

public class DiscoveryDocument
{
    [JsonPropertyName("token_endpoint")]
    public string? TokenEndpoint { get; set; }
    
    [JsonPropertyName("authorization_endpoint")]
    public string? AuthorizeEndpoint { get; set; }
    
}
