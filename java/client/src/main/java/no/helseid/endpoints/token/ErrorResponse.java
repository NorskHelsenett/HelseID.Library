package no.helseid.endpoints.token;

/**
 * A representation of an error response from the token-endpoint in HelseID
 */
public final class ErrorResponse extends TokenResponse {
  private final String error;
  private final String errorDescription;
  private final int statusCode;
  private final String rawResponse;

  /**
   * Create a representation of an error response from the token-endpoint in HelseID
   * @param error the error code returned from HelseID
   * @param errorDescription a description of the error
   * @param statusCode the HTTP status-code returned
   * @param rawResponse the raw body of the HTTO response
   */
  public ErrorResponse(String error, String errorDescription, int statusCode, String rawResponse) {
    this.error = error;
    this.errorDescription = errorDescription;
    this.statusCode = statusCode;
    this.rawResponse = rawResponse;
  }

  /**
   * Access the error code returned from HelseID
   * @return the error code returned from HelseID
   */
  public String error() {
    return error;
  }

  /**
   * Access the description of the error
   * @return a description of the error
   */
  public String errorDescription() {
    return errorDescription;
  }

  /**
   * Access the HTTP status-code returned
   * @return the HTTP status-code returned
   */
  public int statusCode() {
    return statusCode;
  }

  /**
   * Access the raw body of the HTTO response
   * @return the raw body of the HTTO response
   */
  public String rawResponse() {
    return rawResponse;
  }
}

