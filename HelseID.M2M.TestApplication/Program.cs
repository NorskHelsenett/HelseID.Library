using HelseID.Standard;
using HelseID.Standard.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace HelseID.M2M.TestApplication;

sealed class Program
{
    static void Main(string[] args)
    {
        JsonWebKey  JwkPrivateKey = new JsonWebKey("""
                                                   {
                                                     "alg": "RS256",
                                                     "d": "wUJFbzCvRwDlDyVyCmLmXQy0Xod81R5Cwk8U1vW2cJg1E88dQurAkgGAYcISUJKGW1haCVn-WZqmJm2WXLTjNHvGIH-sZapWqINVuwrl1FF_hQ-Cf2hRCyV8P-eU0tn_GH0gCRS2_ER5AbtDw26JfkHy9Y3ZRrL9EXjH_ZEd-7fNM_g_UelGe1a0xBLrCf80HkZaO-U10MV-iu88kqsUFb3RdIsbykGgrYVHmr87Y6K1DLYzJESQU3z_rJmubSELE6-HWw_gf1zb-FXhb1M3i1fbFlB0muom_BnbyOvOFRhGV5ngi8tyRBaz9BMbtyWLEEJzLorjz7C4iKfelefjjaUh-BinhpZ35j_Ki0aY5rwXjcyxQiciUHdfDcntzu815Rq5vu2lcL7VHz4mIp-X7Er4PfKqlrIgBp52SVJpWI1JEL8c7vA2ABGM9hqqY_Akh6YJmdMwNUpqE_Madr_cI2X3R9D0AxeGYrhwwYx41izST9X8dPrJ9X9w2UGlOCweHBi3Ok8gGIYvZzbi6cmXMkzo5J0-qCTQYDS2Lb6h4YKEVN7TQpp3PjXOfeZrc4AW8pHVjnirMoI7GGioGDMMEMA2n60I2qMmX-nyb5K5OsbWPDqMKZLBdTk5mfNfQvQy6cF5BR25QjxFGvXH0ThWjFaUWCgSBa0O3azzkcG1-b0",
                                                     "dp": "Y9CvDg6yfbZGyE-ycLasng1NLT1_cfiYkMLXa4c9TN2L7Ta1R4sBc4vX4IvZ_kSD3ubHD4q3vKSGxEawD7x-3odyrkamLZPHmkvafrkIZPZWcNqGTZAwqCpHxmodJsJIOkPWI_xn2uD2yU9aAaZEoxTqA7Oac-kLdDPjLKsvCRJcilQyD4kvodoK0n4YKINJM-taLIgsxwA7ZkH2GBlV49ZCvKLRZGERhEiunxncx4LNKUd5CBbKCBTZse8EpdhFxbzRgUxOQZujXaVOzdZB_IUkfDPFsN5Fjf6rYQmbyp1W7rUlqJQ5sqRgZngjrCMUdKCzMhVN7X34IMzUDv8ppQ",
                                                     "dq": "zcQhwDxvPDIPUMbIyyWw5G-tnMueZS8KlSmSTzUaLC9QDOU9RbSK6SbNSP-zahG13IJwtAJ9TqYlmj9AbVpdrXtctdcZkUn_wX5P-Qz6J_w-0xODjM_YB3ma_qYsh4yYoEm4lgS0JZN2F7fnc1rOXpzBjm-jsJyRbulD1K908dy6ui9kuO4FfoWpXwX7Jixqfz55koIm1umSVjwI6otz7NOLd8gZcapeBFPrCZnCMIVrGBU0DDVAolEF2GMps4a3259VYPYPkDDs7lwSY8Jk1jFUmdkqwXLwaDzlgnpJJI2boHMOxt1QAuMdToLphah5R3HNLh1NTrLQn0Uehiazhw",
                                                     "e": "AQAB",
                                                     "kty": "RSA",
                                                     "n": "4e76k7QF01kw3hhdHyc3iyUn6c465yrLD9KV7m4gNIc3Tm66Iq3P72xq9w5abAXJG9GJUkbhtTG5isFmXLCU4MOlU5T2a7iBqx2dmao72LSsQ7WZQVtIv5JvWYAXpd4rlgTaJfO3Unv9Tn8v5wWgL9ZbplVmR_GWMY5l-7i44PWwLwGZge_KUVGQmKtx7XXsnezG4JfEAPJbO9zfD4CH6AGtRdcAn2r-2-jqk_-uU1BVoWwDdrCJ_DKOyYNDUfkRneTATY5RDdH5flNd-19XW31L1q3dTMqHbcMzFMfiqwyBYX5kFJrDT-W7poIex7jhZfA5by8K1tfwJqdYRGH4Qp5QtBTs76iuANNBUz3tO0vmV8bYez2AWegvRqrZGHPsMPtA5pCjXw1rueJdH5WqpCHrBsnNkOHNVcd8yHLPmXRokwi3cSanOuquLOI5Qh-pqeuTTJAv8QQ3X5aQRnHmZoyVuOP9Qqq05MGRPp6W-7Vdbi6mslDP-FwUnkHb2C-XenxHesqfcbqBOELa-PD6Fj_usVKPcL9HR_J4IK38XFFOT669_Xhpyaq6iRtvlmj1n_fQNvRcGpZIfIAFgf64cIwLAz2vimj5ywXneyDIRv5Wge8VyhfsAe9S01x0dNq-aR16clayKDn48e6fETeTWJJaPK7lvi1-Oc-tlaA7Pfk",
                                                     "p": "8fDDtrup_sHpmpnAQ6arzA6S2zG23OlRwsrPQu1bByDJSAB6Y0RQDIxQB06NjNCyyoetpiUlblgpcLQpy-R9xAgdvw-gcxNYW4hxkQdbnSP0U9vv8V3tGtxN8szjBEsNz0vhs0Gc4Wz01IYGVY6OJhOg3qshAFrjZie49MsIRw_w0dJA6UAXbx19fvoFtKDSyp-w2FnFQHMnzwsoJULiCPB6R77XJZLx3gyQN4ad-s63E7f89055JNIO8rt-cigkL_ilQh-x9m-oX_nBWb_N4gjVdnyoH2Vjlq2os2l209URLPl4bq6AMCfe7SAnuyLA9U_aTiLOF2eAlbgznmUWjw",
                                                     "q": "7xAWBXurm-Qv60KwKF2dqkGbCyNTrQv1Ep8L2ArTVeTMcQb54dQiwLZb-NDaiRIAyBXOsrkWEyh8tru5fvicn-EdAnajGTxEwwKktaUA-ufDw8vy4HOfATXGuA0DFt1L6nZ_n9lFYxd09iEUj0GwvaRM21A7nbOz3Qf6JThCaP8JOK3doOYsr7fufD8E9gcE7CiTcbbpm8BOa6-h2fY3tENCOV76a9_G2RfPOrvIu-tgVPU_2K-r8vSWQobXhGELOYc0XAipUxEAPxGLOq3T-OTQk2SvPnom2mEG_-5V1PG1sxH0WGf53XpUjjsUW1Zj9bidmuhLPqbMBf-ZhbLm9w",
                                                     "qi": "3h-0vyBkKf0WsPhCiIWXm8yPU-J_qgM4JjkrBzwWOU6CpnQyIdoSfStXhas7ehoODnb7sAQ2PEh4RBls2kJrXmzqC_0wsUEFdnfSERsf8eb8Sgu1NF5JgqRU9UBe9-4bvLZAcFpBG_PJyMg8QhCEuqFbjrjNjg9r3EVfeRHyjl7dPEGF5RwIUiGbkyLmXmcqf0L4GhuvqGdd56krht_kvwI-lppMB4OoZDgwfOH0fcKuHJUyc0RyfZ-iS9YZTQI-1AO2gA-RsjCjvtZB2QBdhvqPp2OujkYlcGXukSBtoBsU3elGBqFPRqlPsDMN8dj0bw_xjNxue7fKh9a68CPedA",
                                                     "kid": "B2C61A07EE0661237D19BEE1E0A1463C"}
                                                   """);
        
        
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
        var helseIdConfiguration = HelseIdConfiguration.ConfigurationForJsonWebKey(JwkPrivateKey,
            "RS256",
            "29a8fc45-1029-485c-8608-e9a3e364468f",
            "e-helse:sfm.api/sfm.api",
            "https://helseid-int-sts.test.nhn.no");
        builder.Services.AddHelseId(helseIdConfiguration);
    
        builder.Services.AddHostedService<TestService>();

        IHost host = builder.Build();
        host.Run();
    }
}

public class TestService : IHostedService
{
    private readonly IHelseIdTokenRetriever _helseIdTokenRetriever;

    public TestService(IHelseIdTokenRetriever helseIdTokenRetriever)
    {
        _helseIdTokenRetriever = helseIdTokenRetriever;
    }
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        
        var test = await _helseIdTokenRetriever.GetTokenAsync();
        Console.WriteLine(test);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
} 
