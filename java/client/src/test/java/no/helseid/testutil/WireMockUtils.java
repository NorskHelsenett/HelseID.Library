package no.helseid.testutil;

import com.github.tomakehurst.wiremock.WireMockServer;
import net.minidev.json.JSONObject;
import no.helseid.signing.Algorithm;

import java.util.Collections;
import java.util.List;

import static com.github.tomakehurst.wiremock.client.WireMock.*;

public class WireMockUtils {
  public static final String TOKEN_ENDPOINT_PATH = "/connect/token";

  private WireMockUtils() {
    // Providing metadata for the test
  }


  public static void stub_token_with_status_code(WireMockServer wms, int httpStatusCode) {
    wms.stubFor(post(urlPathEqualTo(TOKEN_ENDPOINT_PATH))
        .willReturn(status(httpStatusCode)));
  }

  public static void stub_token_with_use_dpop_nonce_response(WireMockServer wms, String useDPoPNonce) {
    var response = badRequest()
        .withHeader("Content-Type", "application/json")
        .withBody(new JSONObject()
            .appendField("error", "use_dpop_nonce")
            .appendField("error_description", "Authorization server requires nonce in DPoP proof")
            .toJSONString()
        );

    if (useDPoPNonce != null) {
      response.withHeader("DPoP-Nonce", useDPoPNonce);
    }

    wms.stubFor(post(urlPathEqualTo(TOKEN_ENDPOINT_PATH))
        .andMatching(request -> HelseIdRequestMatcher.hasDPoPProofContainingNonce(request, null))
        .willReturn(response));
  }

  public static void stub_token_matching_dpop_nonce_returning_mock_access_token(WireMockServer wms, String expectedDPoPNonce, String mockAccessToken, List<String> scopes) {
    // Expected result with a DPoP proof containing expected nonce
    var response = new JSONObject()
        .appendField("access_token", mockAccessToken)
        .appendField("token_type", "DPoP")
        .appendField("expires_in", 10);

    if (scopes != null && !scopes.isEmpty()) {
      response.appendField("scope", String.join(" ", scopes));
    }

    wms.stubFor(post(urlPathEqualTo(TOKEN_ENDPOINT_PATH))
        .andMatching(request -> HelseIdRequestMatcher.hasDPoPProofContainingNonce(request, expectedDPoPNonce))
        .willReturn(okJson(response.toJSONString())));
  }

  public static void stub_token_matching_dpop_nonce_returning_success_status_but_bad_json_body(WireMockServer wms, String expectedDPoPNonce, String body) {
    wms.stubFor(post(urlPathEqualTo(TOKEN_ENDPOINT_PATH))
        .andMatching(request -> HelseIdRequestMatcher.hasDPoPProofContainingNonce(request, expectedDPoPNonce))
        .willReturn(okJson(body)));
  }

  public static void stub_metadata_with_base_url(WireMockServer wms) {
    stub_metadata_with_base_url(wms, wms.baseUrl());
  }

  public static void stub_metadata_with_base_url(WireMockServer wms, String baseUrl) {
    wms.stubFor(get(urlPathEqualTo("/.well-known/openid-configuration"))
        .willReturn(okJson(
            new JSONObject()
                .appendField("issuer", baseUrl)
                .appendField("token_endpoint", baseUrl + TOKEN_ENDPOINT_PATH)
                .appendField("authorization_endpoint", baseUrl + "/connect/authorize")
                .appendField("pushed_authorization_request_endpoint", baseUrl + "/connect/par")
                .appendField("end_session_endpoint", baseUrl + "/connect/endsession")
                .appendField("jwks_uri", baseUrl + "/.well-known/openid-configuration/jwks")
                .appendField("userinfo_endpoint", baseUrl + "/connect/userinfo")
                .appendField("subject_types_supported", Collections.singletonList("public"))
                .appendField("id_token_signing_alg_values_supported", Collections.singletonList(Algorithm.PS256.name()))
                .toJSONString())
        ));
  }
}
