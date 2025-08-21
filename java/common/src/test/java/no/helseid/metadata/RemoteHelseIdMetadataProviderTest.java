package no.helseid.metadata;

import com.github.tomakehurst.wiremock.WireMockServer;
import com.github.tomakehurst.wiremock.client.WireMock;
import com.github.tomakehurst.wiremock.core.WireMockConfiguration;
import no.helseid.exceptions.HelseIdException;
import org.junit.jupiter.api.AfterEach;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;

import java.net.URI;

import static com.github.tomakehurst.wiremock.client.WireMock.*;

class RemoteHelseIdMetadataProviderTest {
  private static final Integer PORT = 25473;
  private static final String OPENID_CONFIGURATION_PATH = "/.well-known/openid-configuration";
  private WireMockServer wireMockServer;

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
    wireMockServer = new WireMockServer(WireMockConfiguration.options().port(PORT));
    wireMockServer.start();
    WireMock.configureFor("localhost", PORT);
  }

  @Test
  void calling_the_expected_endpoint() throws HelseIdException {

    var endpoint = URI.create("http://localhost:" + PORT);

    wireMockServer.stubFor(get(urlPathEqualTo("/.well-known/openid-configuration"))
        .willReturn(aResponse()
            .withStatus(200)
            .withHeader("Content-Type", "application/json")
            .withBody(createMetadataString(endpoint.toString()))));

    new RemoteHelseIdMetadataProvider(endpoint).getMetadata();

    verify(1, getRequestedFor(urlEqualTo(OPENID_CONFIGURATION_PATH)));
  }

  @Test
  void caching_the_response() throws HelseIdException {

    var endpoint = URI.create("http://localhost:" + PORT);

    wireMockServer.stubFor(get(urlPathEqualTo("/.well-known/openid-configuration"))
        .willReturn(aResponse()
            .withStatus(200)
            .withHeader("Content-Type", "application/json")
            .withBody(createMetadataString(endpoint.toString()))));

    RemoteHelseIdMetadataProvider helseIdMetadataProvider = new RemoteHelseIdMetadataProvider(endpoint, 10L);

    helseIdMetadataProvider.getMetadata();
    helseIdMetadataProvider.getMetadata();

    verify(1, getRequestedFor(urlEqualTo(OPENID_CONFIGURATION_PATH)));
  }

  @Test
  void clearing_cached_response_at_expiry() throws InterruptedException, HelseIdException {

    var endpoint = URI.create("http://localhost:" + PORT);

    wireMockServer.stubFor(get(urlPathEqualTo("/.well-known/openid-configuration"))
        .willReturn(aResponse()
            .withStatus(200)
            .withHeader("Content-Type", "application/json")
            .withBody(createMetadataString(endpoint.toString()))));

    RemoteHelseIdMetadataProvider helseIdMetadataProvider = new RemoteHelseIdMetadataProvider(endpoint, 1L);

    helseIdMetadataProvider.getMetadata();

    Thread.sleep(2L);

    helseIdMetadataProvider.getMetadata();

    verify(2, getRequestedFor(urlEqualTo(OPENID_CONFIGURATION_PATH)));
  }

  @AfterEach
  void teardown() {
    if (wireMockServer != null) {
      wireMockServer.stop();
    }
  }
}