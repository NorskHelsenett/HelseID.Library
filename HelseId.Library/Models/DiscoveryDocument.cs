using System.Text.Json.Serialization;
using HelseId.Library.Models.Constants;

namespace HelseId.Library.Models;

public class DiscoveryDocument
{
    [JsonPropertyName(JsonProperties.TokenEndpoint)]
    public string? TokenEndpoint { get; set; }
    
    [JsonPropertyName(JsonProperties.AuthorizationEndpoint)]
    public string? AuthorizeEndpoint { get; set; }
    
}
