package no.helseid.endpoints.token;

import com.github.tomakehurst.wiremock.WireMockServer;
import com.github.tomakehurst.wiremock.core.WireMockConfiguration;
import com.nimbusds.oauth2.sdk.ClientCredentialsGrant;
import com.nimbusds.oauth2.sdk.Scope;
import com.nimbusds.openid.connect.sdk.Nonce;
import no.helseid.clientassertion.ClientAssertion;
import no.helseid.configuration.Client;
import no.helseid.dpop.DPoPProofCreator;
import no.helseid.dpop.DefaultDPoPProofCreator;
import no.helseid.exceptions.HelseIdException;
import no.helseid.signing.Algorithm;
import no.helseid.signing.RSAKeyReference;
import no.helseid.testutil.WireMockUtils;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;

import java.net.URI;
import java.util.Collections;
import java.util.List;
import java.util.Map;

import static no.helseid.testutil.WireMockUtils.TOKEN_ENDPOINT_PATH;
import static org.junit.jupiter.api.Assertions.*;

class TokenEndpointTest {
  private static final String MOCK_ACCESS_TOKEN = "header.payload.signature";
  private static final List<String> SCOPE = Collections.singletonList("nhn:helseid/test");

  private WireMockServer wms;

  @BeforeEach
  void setup() {
    wms = new WireMockServer(WireMockConfiguration.options().dynamicPort());
    wms.start();
  }

  @Test
  public void should_recieve_expected_token_on_client_credentials_grant() throws HelseIdException {
    var keyReference = RSAKeyReference.generate(Algorithm.PS512);
    var client = new Client("client-id", keyReference, SCOPE);
    var dPoPNonce = new Nonce().getValue();

    DPoPProofCreator dpopProofCreator = new DefaultDPoPProofCreator(client.keyReference());
    WireMockUtils.stub_token_with_use_dpop_nonce_response(wms, dPoPNonce);
    WireMockUtils.stub_token_matching_dpop_nonce_returning_mock_access_token(wms, dPoPNonce, MOCK_ACCESS_TOKEN, null);


    TokenResponse tokenResponse = TokenEndpoint.sendRequest(
        URI.create(wms.baseUrl() + TOKEN_ENDPOINT_PATH),
        dpopProofCreator,
        ClientAssertion.createClientAssertionSignedJWT("helseid", client),
        new ClientCredentialsGrant(),
        Scope.parse(SCOPE),
        Collections.singletonList(URI.create("nhn:helseid")),
        Map.of("mykey", Collections.singletonList("myval"))
    );

    assertInstanceOf(AccessTokenResponse.class, tokenResponse);

    AccessTokenResponse accessTokenResponse = (AccessTokenResponse) tokenResponse;
    assertEquals(MOCK_ACCESS_TOKEN, accessTokenResponse.accessToken());
  }


  @Test
  public void should_throw_exception_if_success_response_has_bad_format() throws HelseIdException {
    var keyReference = RSAKeyReference.generate(Algorithm.PS512);
    var client = new Client("client-id", keyReference, SCOPE);
    var dPoPNonce = new Nonce().getValue();
    var badJsonBody = """
    {"hello":"world}
    """;

    DPoPProofCreator dpopProofCreator = new DefaultDPoPProofCreator(client.keyReference());
    WireMockUtils.stub_token_with_use_dpop_nonce_response(wms, dPoPNonce);
    WireMockUtils.stub_token_matching_dpop_nonce_returning_success_status_but_bad_json_body(wms, dPoPNonce, badJsonBody);

    try {
      TokenEndpoint.sendRequest(
          URI.create(wms.baseUrl() + TOKEN_ENDPOINT_PATH),
          dpopProofCreator,
          ClientAssertion.createClientAssertionSignedJWT("helseid", client),
          new ClientCredentialsGrant(),
          Scope.parse(SCOPE),
          null,
          null
      );
      fail();
    } catch (HelseIdException e) {
      assertEquals("Bad format on token response, HTTP status: 200. Content length " + badJsonBody.length(), e.getMessage());
    }
  }

  @Test
  public void should_throw_exception_if_dpop_response_is_missing_nonce() throws HelseIdException {
    var keyReference = RSAKeyReference.generate(Algorithm.PS512);
    var client = new Client("client-id", keyReference, SCOPE);

    DPoPProofCreator dpopProofCreator = new DefaultDPoPProofCreator(client.keyReference());
    WireMockUtils.stub_token_with_use_dpop_nonce_response(wms, null);


    try {
      TokenEndpoint.sendRequest(
          URI.create(wms.baseUrl() + TOKEN_ENDPOINT_PATH),
          dpopProofCreator,
          ClientAssertion.createClientAssertionSignedJWT("helseid", client),
          new ClientCredentialsGrant(),
          Scope.parse(SCOPE),
          null,
          null
      );
      fail();
    } catch (HelseIdException e) {
      assertEquals("Response indicating missing nonce but none was provided.", e.getMessage());
    }
  }

  @Test
  public void should_throw_exception_on_network_error() throws HelseIdException {
    var keyReference = RSAKeyReference.generate(Algorithm.PS512);
    var client = new Client("client-id", keyReference, SCOPE);

    DPoPProofCreator dpopProofCreator = new DefaultDPoPProofCreator(client.keyReference());
    WireMockUtils.stub_token_with_status_code(wms, 418);


    TokenResponse tokenResponse = TokenEndpoint.sendRequest(
        URI.create(wms.baseUrl() + TOKEN_ENDPOINT_PATH),
        dpopProofCreator,
        ClientAssertion.createClientAssertionSignedJWT("helseid", client),
        new ClientCredentialsGrant(),
        Scope.parse(SCOPE),
        null,
        null
    );
    assertInstanceOf(ErrorResponse.class, tokenResponse);

    ErrorResponse errorResponse = (ErrorResponse) tokenResponse;
    assertEquals(418, errorResponse.statusCode());
  }
}