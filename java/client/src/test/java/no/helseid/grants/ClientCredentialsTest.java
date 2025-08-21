package no.helseid.grants;

import com.github.tomakehurst.wiremock.WireMockServer;
import com.github.tomakehurst.wiremock.core.WireMockConfiguration;
import com.nimbusds.jose.JOSEException;
import com.nimbusds.jose.JWSAlgorithm;
import com.nimbusds.jose.jwk.JWK;
import com.nimbusds.jose.jwk.gen.RSAKeyGenerator;
import com.nimbusds.jwt.JWTClaimsSet;
import com.nimbusds.jwt.SignedJWT;
import com.nimbusds.oauth2.sdk.AccessTokenResponse;
import com.nimbusds.oauth2.sdk.dpop.DPoPUtils;
import com.nimbusds.oauth2.sdk.http.HTTPRequest;
import com.nimbusds.oauth2.sdk.token.DPoPAccessToken;
import no.helseid.clientassertion.HelseIdAssertionDetails;
import no.helseid.configuration.HelseIdClientConfiguration;
import no.helseid.exceptions.HelseIdException;
import no.helseid.util.HelseIdRequestMatcher;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;

import java.net.URI;
import java.text.ParseException;
import java.util.Map;

import static com.github.tomakehurst.wiremock.client.WireMock.*;
import static no.helseid.configuration.HelseIdTenantClass.SINGLETENANT;
import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNotNull;

class ClientCredentialsTest {
  private static final String DPOP_NONCE = "eyJ7S_zG.eyJH0-Z.HX4w-7v";
  private static final String MOCK_ACCESS_TOKEN = "header.payload.signature";
  private static final JWK JWK;

  static {
    try {
      JWK = new RSAKeyGenerator(2048).algorithm(JWSAlgorithm.PS256).generate();
    } catch (JOSEException e) {
      throw new RuntimeException(e);
    }
  }

  private WireMockServer wms;

  private static String createMetadataString(String baseEndpoint) {
    return """
        {
              "issuer": "BASE_ENDPOINT",
              "token_endpoint": "BASE_ENDPOINT/connect/token",
              "authorization_endpoint": "BASE_ENDPOINT/connect/authorize",
              "pushed_authorization_request_endpoint": "BASE_ENDPOINT/connect/par",
              "end_session_endpoint": "BASE_ENDPOINT/connect/endsession",
              "jwks_uri": "BASE_ENDPOINT/.well-known/openid-configuration/jwks",
              "userinfo_endpoint": "BASE_ENDPOINT/connect/userinfo",
              "subject_types_supported":["public"],
              "id_token_signing_alg_values_supported":["PS256"]
        }
        """.replaceAll("BASE_ENDPOINT", baseEndpoint);
  }

  @BeforeEach
  void setup() {
    wms = new WireMockServer(WireMockConfiguration.options().dynamicPort());
    wms.start();
  }

  @Test
  void ClientCredentials_should_create_token() throws HelseIdException, ParseException {
    // Providing metadata for the test
    wms.stubFor(get(urlPathEqualTo("/.well-known/openid-configuration"))
        .willReturn(okJson(createMetadataString(wms.baseUrl()))));

    // Expected failure with a DPoP proof without nonce
    wms.stubFor(post(urlPathEqualTo("/connect/token"))
        .andMatching(request -> HelseIdRequestMatcher.hasDPoPProofContainingNonce(request, null))
        .willReturn(badRequest()
            .withHeader("DPoP-Nonce", DPOP_NONCE)
            .withHeader("Content-Type", "application/json")
            .withBody(JWTClaimsSet.parse(Map.of(
                    "error", "use_dpop_nonce",
                    "error_description", "Authorization server requires nonce in DPoP proof"
                )).toString()
            )));

    // Expected result with a DPoP proof containing expected nonce
    wms.stubFor(post(urlPathEqualTo("/connect/token"))
        .andMatching(request -> HelseIdRequestMatcher.hasDPoPProofContainingNonce(request, DPOP_NONCE))
        .willReturn(okJson(
            JWTClaimsSet.parse(Map.of(
                "access_token", MOCK_ACCESS_TOKEN,
                "token_type", "DPoP",
                "expires_in", 10
            )).toString()
        )));

    HelseIdClientConfiguration configuration = HelseIdClientConfiguration.create(
        JWK,
        "client-id",
        "nhn:helseid/test",
        wms.baseUrl(),
        SINGLETENANT,
        null
    );

    ClientCredentials clientCredentials = new ClientCredentials(configuration);

    wms.verify(1, getRequestedFor(urlEqualTo("/.well-known/openid-configuration")));

    AccessTokenResponse accessTokenResponse = clientCredentials.getDPoPAccessToken(
        new HelseIdAssertionDetails(
            null,
            "994598759",
            null
        ));

    wms.verify(2, postRequestedFor(urlEqualTo("/connect/token")));

    assertEquals(MOCK_ACCESS_TOKEN, accessTokenResponse.getTokens().getDPoPAccessToken().toString());
  }

  @Test
  void ClientCredentials_should_create_DPoP_proof() throws HelseIdException, ParseException, JOSEException {
    // Providing metadata for the test
    wms.stubFor(get(urlPathEqualTo("/.well-known/openid-configuration"))
        .willReturn(okJson(createMetadataString(wms.baseUrl()))));

    HelseIdClientConfiguration configuration = HelseIdClientConfiguration.create(
        JWK,
        "client-id",
        "nhn:helseid/test",
        wms.baseUrl(),
        SINGLETENANT,
        null
    );

    DPoPAccessToken dPoPAccessToken = new DPoPAccessToken(MOCK_ACCESS_TOKEN);

    ClientCredentials clientCredentials = new ClientCredentials(configuration);

    wms.verify(1, getRequestedFor(urlPathEqualTo("/.well-known/openid-configuration")));

    URI htu = URI.create(wms.baseUrl() + "/test");
    String htm = HTTPRequest.Method.GET.name();
    String dPoPProof = clientCredentials.getDPoPProof(htm, htu, dPoPAccessToken);

    JWTClaimsSet dPoPProofClaims = SignedJWT.parse(dPoPProof).getJWTClaimsSet();

    assertNotNull(dPoPProofClaims.getClaimAsString("jti"));
    assertEquals(htm, dPoPProofClaims.getClaimAsString("htm"));
    assertEquals(htu.toString(), dPoPProofClaims.getClaimAsString("htu"));
    assertNotNull(dPoPProofClaims.getDateClaim("iat"));
    assertEquals(DPoPUtils.computeSHA256(dPoPAccessToken).toString(), dPoPProofClaims.getClaimAsString("ath"));
  }
}