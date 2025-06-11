using System.Text.Json.Serialization;
using HelseId.Standard.Models.Constants;

namespace HelseId.Standard.Models;

public class DiscoveryDocument
{
    [JsonPropertyName(JsonProperties.TokenEndpoint)]
    public string? TokenEndpoint { get; set; }
    
    [JsonPropertyName(JsonProperties.AuthorizationEndpoint)]
    public string? AuthorizeEndpoint { get; set; }
    
}
