/*

var useMultiTenantPattern = new Option<bool>(
    aliases: new [] {"--use-multi-tenant", "-mt"},
    description: "If set, the application will use a client set up for multi-tenancy, i.e. it makes use of an organization number that is connected to the client.",
    getDefaultValue: () => false);

var useChildOrgNumberOption = new Option<bool>(
    aliases: new [] {"--use-child-org-number", "-uc"},
    description: "If set, the application will request an child organization (underenhet) claim for the access token.",
    getDefaultValue: () => false);

var rootCommand = new RootCommand("A client credentials usage sample")
{
    useChildOrgNumberOption, useMultiTenantPattern
};

rootCommand.SetHandler(async (useChildOrgNumberOptionValue, useMultiTenantPatternOptionValue) =>
{
    var clientConfigurator = new ClientConfigurator();
    var client = clientConfigurator.ConfigureClient(useChildOrgNumberOptionValue, useMultiTenantPatternOptionValue);
    var repeatCall = true;
    while (repeatCall)
    {
        repeatCall = await CallApiWithToken(client);
    }
}, useChildOrgNumberOption, useMultiTenantPattern);

await rootCommand.InvokeAsync(args);


private static async Task<bool> CallApiWithToken(Machine2MachineClient client)
{
    await client.CallApiWithToken();
    return ShouldCallAgain();
}

private static bool ShouldCallAgain()
{
    Console.WriteLine("Type 'a' to call the API again, or any other key to exit:");
    var input = Console.ReadKey();
    Console.WriteLine();
    return input.Key == ConsoleKey.A;
}
*/
