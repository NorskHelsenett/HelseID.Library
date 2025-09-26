namespace HelseId.Library.SelfService.Configuration;

public class SelvbetjeningConfiguration
{
    public string UpdateClientSecretEndpoint { get; init; } = "https://api.selvbetjening.test.nhn.no/v1/client-secret";
    public string SelvbetjeningScope { get; init; } = "nhn:selvbetjening/client";
}
