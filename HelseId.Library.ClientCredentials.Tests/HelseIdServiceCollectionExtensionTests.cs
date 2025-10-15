using HelseId.Library.Interfaces.Configuration;
using HelseId.Library.ClientCredentials.Interfaces;
using HelseId.Library.ClientCredentials.Interfaces.TokenRequests;
using HelseId.Library.ClientCredentials.PayloadClaimCreators;
using HelseId.Library.ClientCredentials.Services.TokenRequests;
using HelseId.Library.Services.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace HelseId.Library.ClientCredentials.Tests;

[TestFixture]
public class HelseIdServiceCollectionExtensionTests
{
    private const string JwkPrivateKeyAsString = """
                                                     {"alg":"RS256", "d":"XdL6YAB9o0qpXzHsxJwXT1cL7fLArydZAFu5Dqu0a0r_DoXgc8W_VAlOboeGwhoUa2XsJLOiXD_xKwWeKozy54PaHdXQxmXGmUElhEkBdJEt82O_bu4MgQffqPIYLTE3GmmYAoz4GKsFqYQQjvDfIU-vSx69l-f0xBYuU7Po_YYmIL83q5xkuBAva3t9Aj9WDAk82YpT7B5bloHffvLkm30cmkHMHhCXgEJTstkwHI9wMFUrWxJrlh8vpwVbQyiDDDNGyHDmfRMr63gjx3ivwwQspiHdUrNtf1-RNundl362L7qypTmAWPi21bd3D8oLoBHbEFG7CfNeBLnjffKAsh8fNCksv_Hn-jE_HtE-VHpywKkcAvZHa-cb8GvkQg54_x-qgFJ1Zi1YQ7id1e0jtpCKB_SRN-kzzgNL8shfwWm9xHs-P4og6xvpp7IrRtBrhcyKGCjaaUX0KBF9RtgApmT-mPq4ZPM6Zdov2ZrLnzUC2prNAsV-pAHcl2PXLqQM74tlzB1fLpqUQeWKamW5quV64O5tBh4R54-FqoYNacL076QHkP96b0YuY4k-Hbu_nZa1IQlfop8AmZPsCJc4R2CmKVtHdvboSd1vtovwrhTkJHNILC5h8HXIKsKlM9ZeZwIJ7STAKWJlSYgMM4hvPoPQ6MKT7NRLh-R4iQHbPDk","dp":"Ukd8lLTFkEBmgL_XGi6JbMBbQMqKnbzeadtvUuYtB91cEwTUnuGRwQ6sEVthP7pGZhVVih57VVk5VAZ1LD56cXA2zdAJQRrCfpJo_5dYr5FcIowx5ZCD2ICWcNLOv77lgTilUyxNsNvgM40niT1aSzVlDNuKOUtjcvJ8SVFjIBnrnL-IZ5NpQDwNTim1m0Po1o1s5EP5RtQ-0yTF8-G-Cgk5H0ClWWVtdD45Im5yUShvNbY-QEAXxPTZHGOsOep4Lm9bg17fbfDtcvtMVMShEjHTwJbUi2Tv2pOzgCGlvyJZ_SKIQmgMuzGcq18R6bHr_qtc3-XLVNYnzXsY7AgELQ","dq":"jBzrMG7LhJQrE1ROjEdnIbRqgnNisMJhOSo4Hi_hAbY9tGL3Posp8_2M_3AOAU9HALPrE2T1nBVjwRaIcOROuM_xzZT9FgWBnlixNcgyIRuQuF7doLfQ5Baz9T872PO6uSajGFKyGyc07ivfyfGsif7QIyVG-ALWfso1-GjzQT-iZc60jKyeeU8NGNPW5yss3xIDI35Md7P7PFIhH-L6gB_lsGD8jxrC8XcXzcHW1UqcSGkw7E7e7heohHXmB_fKa5ySZnczPgR3Sbg5CXG7AVpNxY0OSiJn5AVGMpMu5airOcJ6JVdQbfknW6Ct08lvCTJfxFzNPFNCeeH5i8l7RQ","e":"AQAB","kty":"RSA","n":"4qXP535J5q0tN1hgr9IQhs6HgtPrtiwzLaiPYAJx3WIQDj0Fzy4FQ2XzlKLg24c0Yk7X4kFm3ua0tZ0Va-Iod1XdVDm8SfcGumqKTui_Q8G-7zNz_oq-CIij9_B1wVHtMys4mUnVcKOkoFqHt_9gvXb034CoZt4X2s_hS0OgEVqtWUJDjRHMrBWVtY7ioe0Ed33Kd5qFCJu60rOuZ6FfAXgo1VCpslRCg2egVF3cKNXAoBYAfjIS6jqYgeMlsUNzaNs_FjWf2K-rvfxoPuyJHzHp3a0vIQOukw-522EjszGEctOAzSQd_gJzmH1Hw3gAjGJ3Z8kAKrYdjflSQw1xamml8DE5Y4cAGG9g-xNvWtiRWRperRGXsBn3qfbtWK9vN07pNOgWxqa23lTsZQqWzPPTWltyiux_7TRGGkqxrpGI3XEXlcV7O4xBmgEN3-T9kZHD40Uel9axE9OIv6CrgiuTAFxGLIIxd-fAZ7_jRQ8_Q1AdO78OwVd8LkziKI5H4c0ewWMVpxT4Afwfb7smyKwN4Cjb_TwAFpH767F0Z95i5nhpce_ZelGH3klM44OSSL_HmWjNB1ndQOJbwUjEmvsJy_PwGmtCsqZisk76mV8-5--ptftgfjvxmAIVQHiJtUkbkIQyq-EiZhoo1IZFZPqJ5gs3-GRUGE6SXlG5JaU","p":"8Z0cVaG87pB4Coiud64BAFGUJN0xB0Q0qIA1Y0iP3T-pMfQFRe09z4Hh8ozXrqUzUsqclv-sK9LtrzLri2jBzj-MuJ3qfoscQXv96ZFKEH7E7PCQ0xYER1oT4hleM92mBrk1v7zWmsde2t-2HHUKTpe8Tbs2Q-yZr7pvCGSIeItu0DQubDAvk2li4PMJHmoUuJw5AVFMT7cFM1xOAeCvOMVUc6S_QrjBLvTva8uoLUV-LYgnt3yva3x6tA0JoWv2SIjX3IO208AruYiPhTrpNLqgHGKFrjV9dZ0xhyjJtRNObggXXWWYqhzEP-k2FLykLGgaAbpvBPMLrECVeXApWw","q":"8CSTicEe_kVG0L5BUcjiMDNI0dTJCScEkBj3m3gdXHOhbpByaalsu6sdekT-eCRebMWNTFXqcwnk3K34hk4PYQbwpu-rPF2DGUg1Vbe4nTwQylgeT9Ag9xD3GG5W5rQr2BuBHHBGyn1bJ1uqnKTYZ3IHUu7BMmy9_FX-mSVC-isBeBnHHaqRNk5R3VOeHR-WPEYES3mDKq0matwjaosi3935p7LaqAmnhYYG9-kAD6YjV7Mmvy2xwZ7qG8KH1r6pB3ON4q_fk_gOIKQfxZXZN4lOh8PTOBFd5Gy9feMPoa3fbOkrcuGMzXbKPr61JAASjJNmz94N2kVTEJD0Uo4c_w","qi":"sNycboiTb6exCAaJk-_CVTXNqmjhvDwxProOEdJuD1DPJKqOlM-VU4m9M5zsiUv7EHuvw_A6fX6oxvLrHnsoPmaDfN05GrjxyoCRNtBva4L2OuO8KHtqgqTSJSjmLEM7P7jqriY_pss5ZgylOg7DEeZZGWBuQGqdnTb3RtOeehM0aUTJO6xXE34809tVBLyqkrOd5CBOO8ve2kcgxKrRErcz5-l-TJiE0jFPZej9UmMgz4JGpLBGVS0-jrGJqUdP4HWjEzU14CXxhg5sFZNx6NFCS-4o-DtUkTEHjDKb8u8zBOR6pvsYJ5HF837zomWf39JMJGiFL1XCZWXniVbW9Q","kid":"C71331FC403783934D2E335FD1F75B8B"}
                                                 """;
    private readonly JsonWebKey _jwkPrivateKey = new (JwkPrivateKeyAsString);

    private ServiceCollection _serviceCollection = null!;
    private HelseIdConfiguration _config = null!;

    [SetUp]
    public void SetUp()
    {
        _serviceCollection = new ServiceCollection();
        _config = new HelseIdConfiguration { ClientId = "client id", Scope = "scope", IssuerUri = "sts"};    
    }

    [Test]
    public void AddHelseIdClientCredentials_with_configuration_registers_expected_services()
    {
        _serviceCollection.AddHelseIdClientCredentials(_config);
        
        EnsureSingletonRegistration<IHelseIdClientCredentialsFlow, HelseIdClientCredentialsFlow>();
        EnsureSingletonRegistration<IClientCredentialsTokenRequestBuilder, ClientCredentialsTokenRequestBuilder>();
        EnsureSingletonRegistration<IDPoPProofCreator, DPoPProofCreator>();
        EnsureSingletonRegistration<IDPoPProofCreatorForApiRequests, DPoPProofCreator>();
        EnsureSingletonRegistration<IHelseIdEndpointsDiscoverer, HelseIdEndpointsDiscoverer>();
        EnsureSingletonRegistration<ISigningTokenCreator, SigningTokenCreator>();
        EnsureSingletonRegistration<IPayloadClaimsCreator, ClientAssertionPayloadClaimsCreator>();
        EnsureSingletonRegistration<IAssertionDetailsCreator, AssertionDetailsCreator>(); 
        EnsureSingletonRegistration<IStructuredClaimsCreator, OrganizationNumberCreatorForSingleTenantClient>();
        EnsureSingletonRegistration<IHelseIdConfigurationGetter, RegisteredSingletonHelseIdConfigurationGetter>();
        EnsureSingletonRegistration<TimeProvider>();
        EnsureSingletonRegistration<HelseIdConfiguration>();
    }
    
    [Test, Ignore("TODO")]
    public void AddHelseIdClientCredentials_with_configuration_registers_expected_services_with_jwt_private_key()
    {
        _serviceCollection.AddHelseIdClientCredentials(_config).AddJwkForClientAuthentication(JwkPrivateKeyAsString);
        
        EnsureSingletonRegistration<IHelseIdConfigurationGetter, RegisteredSingletonHelseIdConfigurationGetter>();
        EnsureSingletonRegistration<ISigningCredentialReference, StaticSigningCredentialReference>(); 
    }

    private void EnsureSingletonRegistration<TServiceType, TImplementationType>()
    {
        var registeredService = _serviceCollection.SingleOrDefault(s => s.ServiceType == typeof(TServiceType));
        registeredService.Should().NotBeNull();
        registeredService.Lifetime.Should().Be(ServiceLifetime.Singleton);
        registeredService.ImplementationType.Should().BeSameAs(typeof(TImplementationType));
    }


    private void EnsureSingletonRegistration<TServiceType>()
    {
        var registeredService = _serviceCollection.SingleOrDefault(s => s.ServiceType == typeof(TServiceType));
        registeredService.Should().NotBeNull();
        registeredService.Lifetime.Should().Be(ServiceLifetime.Singleton);
    }
}
