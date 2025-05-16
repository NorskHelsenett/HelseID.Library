using System.Text.Json.Serialization;
using HelseID.Standard.Models.Constants;

namespace HelseID.Standard.Models;

public class DiscoveryDocument
{
    [JsonPropertyName(JsonProperties.TokenEndpoint)]
    public string? TokenEndpoint { get; set; }
    
    [JsonPropertyName(JsonProperties.AuthorizationEndpoint)]
    public string? AuthorizeEndpoint { get; set; }
    
}
