package no.helseid.endpoints.token;

import java.util.List;

/**
 * A representation of a successful response from the token-endpoint in HelseID
 */
public final class AccessTokenResponse extends TokenResponse {
  private final String accessToken;
  private final String tokenType;
  private final long expiresInSeconds;
  private final List<String> scope;

  /**
   * Create a representation of a successful response from the token-endpoint in HelseID
   * @param accessToken the access token returned from HelseID
   * @param tokenType the token type, case-insensitive
   * @param expiresInSeconds the number of seconds until the token expires
   * @param scope the scopes included in the returned token
   */
  public AccessTokenResponse(
      String accessToken,
      String tokenType,
      long expiresInSeconds,
      List<String> scope
  ) {
    this.accessToken = accessToken;
    this.tokenType = tokenType;
    this.expiresInSeconds = expiresInSeconds;
    this.scope = scope;
  }

  /**
   * Access the access token returned from HelseID
   * @return the access token returned from HelseID
   */
  public String accessToken() {
    return accessToken;
  }

  /**
   * Access the token type, case-insensitive
   * @return the token type, case-insensitive
   */
  public String tokenType() {
    return tokenType;
  }

  /**
   * Access the number of seconds until the token expires
   * @return the number of seconds until the token expires
   */
  public long expiresInSeconds() {
    return expiresInSeconds;
  }

  /**
   * Access the list of the scopes included in the returned token
   * @return a list of the scopes included in the returned token
   */
  public List<String> scope() {
    return scope;
  }
}