package no.helseid.grants;

import no.helseid.cache.ExpiringCache;
import no.helseid.cache.InMemoryExpiringCache;
import no.helseid.clientassertion.AssertionDetails;
import no.helseid.configuration.Client;
import no.helseid.dpop.DPoPProofCreator;
import no.helseid.dpop.DefaultDPoPProofCreator;
import no.helseid.endpoints.token.AccessTokenResponse;
import no.helseid.endpoints.token.TokenResponse;
import no.helseid.exceptions.HelseIdException;
import no.helseid.metadata.MetadataProvider;
import no.helseid.metadata.RemoteMetadataProvider;

import java.net.URI;

/**
 * Client Credentials pattern
 */
public interface ClientCredentials {
  /**
   * Request access token from HelseID without assertion details, the token is cached and referenced thru-out it's lifetime
   * @return the token response from HelseID, might be a AccessTokenResponse or an ErrorResponse
   * @throws HelseIdException if a request to HelseID returned in an unprocessable failure
   */
  TokenResponse getAccessToken() throws HelseIdException;

  /**
   * Request access token from HelseID with assertion details, the token is cached and referenced thru-out it's lifetime
   * @param assertionDetails the assertion details to be included in the client assertion
   * @return the token response from HelseID, might be a AccessTokenResponse or an ErrorResponse
   * @throws HelseIdException if a request to HelseID returned in an unprocessable failure
   */
  TokenResponse getAccessToken(AssertionDetails assertionDetails) throws HelseIdException;

  /**
   * Builder class for Client Credentials
   */
  class Builder {
    private final MetadataProvider metadataProvider;
    private Client client;
    private ExpiringCache<AccessTokenResponse> tokenCache;
    private DPoPProofCreator dPoPProofCreator;

    /**
     * Initialize a builder class for client credentials
     * @param authority the authority to communicate with
     */
    public Builder(final URI authority) {
      this.metadataProvider = RemoteMetadataProvider.getInstance(authority);
    }

    /**
     * Assign the client to represent
     * @param client the client
     * @return the current builder
     */
    public Builder withClient(final Client client) {
      this.client = client;
      return this;
    }

    /**
     * Assign a custom token cache implementation, the default is in memory
     * @param tokenCache a custom token cache implementation
     * @return the current builder
     */
    public Builder setCustomTokenCache(final ExpiringCache<AccessTokenResponse> tokenCache) {
      this.tokenCache = tokenCache;
      return this;
    }

    /**
     * Assign a custom provider for DPoP-proofs, the default uses the key reference of the client
     * @param dPoPProofCreator a custom DPoP proof creator
     * @return the current builder
     */
    public Builder setCustomDPoPProofCreator(final DPoPProofCreator dPoPProofCreator) {
      this.dPoPProofCreator = dPoPProofCreator;
      return this;
    }

    /**
     * Build the client credentials
     * @return a default implementation of client credentials
     * @throws HelseIdException if misconfigured
     */
    public ClientCredentials build() throws HelseIdException {
      if (client == null) {
        throw new HelseIdException("No client is provided");
      }

      if (tokenCache == null) {
        this.tokenCache = new InMemoryExpiringCache<>();
      }

      if (dPoPProofCreator == null) {
        this.dPoPProofCreator = new DefaultDPoPProofCreator(client.keyReference());
      }

      return new DefaultClientCredentials(
          this.client,
          this.metadataProvider,
          this.tokenCache,
          this.dPoPProofCreator
      );
    }
  }
}
