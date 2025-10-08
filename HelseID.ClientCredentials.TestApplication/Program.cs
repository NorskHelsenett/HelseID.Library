using HelseId.Library;
using HelseId.Library.Configuration;
using HelseId.Library.ClientCredentials;
using HelseId.Library.ClientCredentials.Interfaces;
using HelseId.Library.Models;
using HelseId.Library.Models.DetailsFromClient;
using HelseId.Library.Selvbetjening;
using HelseId.Library.Selvbetjening.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HelseID.ClientCredentials.TestApplication;

sealed class Program
{
    static async Task Main(string[] args)
    {
        var helseIdConfiguration = new HelseIdConfiguration
        {
            ClientId = "942e63a3-4d68-45e8-8d19-bcf1aae6ab74",
            Scope = "nhn:hgd-persontjenesten-api/full-access",
            StsUrl = "https://helseid-sts.test.nhn.no",
        };

        var key = """
                  {"alg":"PS256","d":"N6Yi6EubRnM-gHjYYdQVYaR79R5savmNdRCZ_Q5dGeZfDnDeAzHYPdQnLIpnWSFGqLWKPZXFG8t1v16wkTshsoo4OYCXI-ZmTN1vzE0MnZGB94ILqpM_c785t0YmOzuxHhD_csMvyk63TW96e0JLjjWBpLTcomsXRzEX4-CsSTTAOne3fGS_jeUdvY4pbWgW_nhTXjlZz-3haQXPrFXlBA-FVUj0cvfzdAfhG20qqpFNBPZvGu9WwaYepRF6MBtf1i9aGDfPWmrJNFKpfLgSe_FZ372zuWRZBGq7-OXHGgJZvBZ8Fk1b1U0omTwq-U2XZPHmRWkaN0Fuh_V2qwFnVSJUElrdEmdaSZBITJcDk94gzlnAdOLERzIa12nDvQbFEyg_pQEEIkvat7ffMVvAC5Tj4WUq_I2pKzzRCkGPJ5_JePRc4uyONCctFx8AOri1OE3hT_Byyfp5g68toUntA2m8fzhk6L5aj2wyhri48RwJH9fQfw-G5RS1qqEJ86caL1VVKDW2RjgQDPh6bvra1Ph8tdEU33EZdCUEbp9GwiwazmCi3wMdqJiyCQT8TUFEKPoMiKRh3FIznQtblcu7gGkYFL6BabPomvPJaGA6SqqXKmvYSZ-GatpnkKm-vmindDNORoskEluAAfAQp7BMjVi2ukBomjqYSG8h2XF7dbE","dp":"PQkBrDx55SIll2KONe304i7BMH7D9F2Pz1QG_Y9jC5OqFWRj6QT4YTU-eAQM84JnmyvAPaKcgXncg5fYR2yNMShPQvTnvl-XfeHwx3bnhA0Yp0VcEGtlquZxJ9t8E6F2NWQO-6VPzl3kHqxGILdNe_bj5QwiTx0ulJqCrMcoINXJzm3JzOtW-4bkBH_7esG_NhJQ6_ispcHmlEvIH5-gVyi9pFaXAh7D9purB6SY55LNVWrnb_iI96zJc6Szz5kqE1bJvofF_TbSWvYScrsD4iEfbKuEdf0oXv_G0kcixh8iZreqGIB9wt_ng0yHt5J0DOQI5WnvCEG0ww54Ja8GWQ","dq":"fx9W3djm_ET5FsxJoDGfA-0bUMmO8AtHgHenE6er2KE0A0mTDS0vPwgqzC-JShmtuu2Q0d8Y6RQuYunkhXyPyPG0sYr4JrAbFjShgrHftA5abClPZo_dkP0goaM6J9HumJ3A9qS6f2OYB7lxCiWSCXrWnsPyyoQHeySMEQAVLe4s-JTMhCyFQZEnlc_XBHAOEfc5_w5gyqbs7OtMOnZFGnF_NrkgyVkmOWEj5oOw3AAVbwTTcAJgnCdr-BP-iz6XEiO3c4CM_VYaHg7UQeeEJEGFfIhaHcq2Vifi8mPYrp_J1vUWTZkboV5tCZrqayp6n1_TFmW77zRh-lf_8MQTyQ","e":"AQAB","key_ops":["sign"],"kty":"RSA","n":"3pzkrUzDe0gA7uEe7y2ZjU2rnRf6hYH3wldFk7e3f-vjgs29BzOKjNvHceQXHn75I9VbIeySeIwoW_MvjGz9aqVVpDnTeUQN3UZnwJJl9BBOZUmh2QMMKCtWh1mTa9VgvItoPKt84-2WJvO7eeECxg9XpjieP5flKLYfLF7yI0yy76uGforp9jqHdM1vJAKhpMPaaZyGlzkiQCXjYCPF9uoBOz4T6AE5tNQn3diPwS7jFJoirgR-1e7Y6CQdlsya-KijwNaho28cOOBavvE4lPGrCwp6BkhixJoT9tbbWpM5RqX7SxL01ndRdtjsOZMpFsmIA12L9551n2Z4G8C4XuX3dm2Q7XvgzYpO1Q6ZMXf4K9lA0wsUpZiPeWS-r2pI2bUakGZ1TtEh06nttuD2zIwBo2vuffHWfEWoctbDBfUyBncICuAPXbntXbVdSLFhB6eJNpsE58eyyutCZ1PZn4OmF7zyE5wpYdW52TOocla3mSlR4H7qDw9H-aWcgYSKYkMc4xm6yXyzZ-OEyQNIXdUN79D_y0bpyoeUGT57l-26NorssQYEjg4hlcG-i6GktVaMJgLNVcvmq1KpBEeuRShav6ALgfNWPRoKYVhzin_P356BsZXdeMd9XYxK4l9_YVdKGbo3hJPFPfRvlOV38j9Qw0035poWHyb5hi24rLk","p":"-PoVb3qBOIDreB1ogTKexXOkFESGR7p3M9j3Uz8kJ38WGgtNXg7ffzaV7gCnHJNo54vs5qVujalZahiZuOH1MSIjTHSN7lWQnG8COKg_6WISG3BjxuILgmA-b2wxUYMrGZAVaTPntgc97ff7XOtqhl0eHWIc7LYftK4eQ4bZqXm_gGRTaUeruLVC0ZFJ0b8VUTGKH3KkGRba6Hyc2PUBOTikpmvVLhQL8NMfkhx_y6rMR-YzOgg8rllQSKGARpKO6BSatf5Z4pD-uMfWlWxMkQSU-20KUUpjDMgZhw7mPQbcLWnpgOK_tgcc5F7jiwNnZyjqcrVGfwouqu48jXuurQ","q":"5ORt3ow5COA0aTNVF8gQsanSb-u99DpKC5z3mOkNvstL6-VRXfs4ZRt8MotFUXP6kxOX2JzV0K_xTtiJ9j7Nfz5s7WMrCq2uF31KSBLPW1l-6dfSiRbGX-z-QtgQK4XjzNPpd8o3iUN5Ug-7YTSKWnu0DRJsnMBHHY3p130qtOSwVNziFkLFSUMmA9mo9QkXNdB0SpzJVHf1OUuWpxoXNw-yLz0IMK82PrR064eydk7BaTFSMpi9m9-XVxYznpT7kvl4XQJTECgAerl_-FFng-pLEwuRQPrd1owHGwqyv7uGytzMxhyB_mkt-_oB--YaCVRRr9qK7e0v41_EpNVzvQ","qi":"6SWImUlChc1f33why7lSSx-vgEZeaKlloV8kWuCSbKJU4feAP_VSK9r4CUu5h6isTZWBJxhsylLmBYMiHvzK7q4fq_AyPSW4mDFPBf0dQgC_MeWLFgs_vpwNt754wliQFG-XD29WSr3ZAUgv27wSlWjryxs__2ITQfn_kH6CZC6lD-VPwsT-YmA04o2sAUIsMORMVLgjvY2oJsuzwwqw_Q4u9OVszoYia27maIMSVKq4cbmsIwatKoEP2neKI3RUWgH-TDCd2lmSVQVvFbbf1r4PaqAgpMVgJGHIwhWvK8G7BMsnmTFbRqmfNO2zSxZcOuBmA0P0bGiO94yF1GKuAQ","kid":"ILinzam84XedziEkc2myaQYuIu2_LAWddHOn0R_vavk"}
                  """;

        var clientCredentialsFlow = ClientCredentialsFlowFactory.GetSingleTenant(helseIdConfiguration, key);

        var tokenResponse = await clientCredentialsFlow.GetTokenResponseAsync(new OrganizationNumbers("979962983", "999988889"));

        if (tokenResponse is AccessTokenResponse accessTokenResponse)
        {
            Console.WriteLine(accessTokenResponse.AccessToken);
        }
        else
        {
            var errorResponse = (TokenErrorResponse)tokenResponse;
            Console.WriteLine(errorResponse.ErrorDescription);
        }


        // HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
        // var helseIdConfiguration = new HelseIdConfiguration
        // {
        //     ClientId = "f2778e88-4c3d-44b5-a4ae-8ae8e6ca0692",
        //     Scope = "nhn:helseid-testapi/api nhn:selvbetjening/client",
        //     StsUrl = "https://helseid-sts.test.nhn.no",
        // };
        //
        // builder.Services.AddHelseIdClientCredentials(helseIdConfiguration)
        //     .AddSelvbetjeningKeyRotation()
        //     .AddFileBasedSigningCredential("jwk.json")
        //     .AddHelseIdMultiTenant()
        //     .AddHelseIdDistributedCaching();
        //
        // builder.Services.AddHostedService<TestService>();
        //
        // var flow = ClientCredentialsFlow.GetSingleTenant(helseIdConfiguration, "");
        //
        // IHost host = builder.Build();
        // host.Run();
    }
}

public class TestService : IHostedService
{
    private readonly IHelseIdClientCredentialsFlow _helseIdClientCredentialsFlow;
    private readonly ISelvbetjeningSecretUpdater _selvbetjeningSecretUpdater;

    public TestService(
        IHelseIdClientCredentialsFlow helseIdClientCredentialsFlow,
        ISelvbetjeningSecretUpdater selvbetjeningSecretUpdater)
    {
        _helseIdClientCredentialsFlow = helseIdClientCredentialsFlow;
        _selvbetjeningSecretUpdater = selvbetjeningSecretUpdater;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var organizationNumbersBergen = new OrganizationNumbers
        {
            ParentOrganization = "994598759", // NHN
            ChildOrganization = "920773230" // NHN Bergen
        };

        var organizationNumbersTrondheim = new OrganizationNumbers
        {
            ParentOrganization = "994598759", // NHN
            ChildOrganization = "987402105" // NHN Trondheim
        };

        var accessTokenBergen = await _helseIdClientCredentialsFlow.GetTokenResponseAsync(organizationNumbersBergen);
        Console.WriteLine(((AccessTokenResponse)accessTokenBergen).AccessToken);

        await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);

        await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
        await _selvbetjeningSecretUpdater.UpdateClientSecret();
        await Task.Delay(TimeSpan.FromSeconds(15), cancellationToken);

        var accessTokenTrondheim = await _helseIdClientCredentialsFlow.GetTokenResponseAsync(organizationNumbersTrondheim);
        Console.WriteLine(((AccessTokenResponse)accessTokenTrondheim).AccessToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}
