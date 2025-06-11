namespace HelseId.Standard.Models.Payloads;

// A claim type that can contain an object as the value.
public class PayloadClaim(string name, object value)
{
    public string Name { get; } = name;
    public object Value { get; } = value;
}
