namespace HelseId.Library.Selvbetjening.Models;

public class ClientSecretResult
{
    public required DateTimeOffset ExpirationDate { get; init; }
    public required string PrivateJsonWebKey { get; init; }
}
