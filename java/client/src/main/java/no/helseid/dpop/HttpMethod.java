package no.helseid.dpop;

/**
 * Representation of HTTP Methods
 */
public final class HttpMethod {
  /**
   * GET method <a href="https://datatracker.ietf.org/doc/html/rfc7231#section-4.3.1">as described in RFC 7231</a>
   */
  public static HttpMethod GET = new HttpMethod("GET");
  /**
   * HEAD method <a href="https://datatracker.ietf.org/doc/html/rfc7231#section-4.3.2">as described in RFC 7231</a>
   */
  public static HttpMethod HEAD = new HttpMethod("HEAD");
  /**
   * POST method <a href="https://datatracker.ietf.org/doc/html/rfc7231#section-4.3.3">as described in RFC 7231</a>
   */
  public static HttpMethod POST = new HttpMethod("POST");
  /**
   * PUT method <a href="https://datatracker.ietf.org/doc/html/rfc7231#section-4.3.4">as described in RFC 7231</a>
   */
  public static HttpMethod PUT = new HttpMethod("PUT");
  /**
   * DELETE method <a href="https://datatracker.ietf.org/doc/html/rfc7231#section-4.3.5">as described in RFC 7231</a>
   */
  public static HttpMethod DELETE = new HttpMethod("DELETE");
  /**
   * CONNECT method <a href="https://datatracker.ietf.org/doc/html/rfc7231#section-4.3.6">as described in RFC 7231</a>
   */
  public static HttpMethod CONNECT = new HttpMethod("CONNECT");
  /**
   * OPTIONS method <a href="https://datatracker.ietf.org/doc/html/rfc7231#section-4.3.7">as described in RFC 7231</a>
   */
  public static HttpMethod OPTIONS = new HttpMethod("OPTIONS");
  /**
   * TRACE method <a href="https://datatracker.ietf.org/doc/html/rfc7231#section-4.3.8">as described in RFC 7231</a>
   */
  public static HttpMethod TRACE = new HttpMethod("TRACE");

  /**
   * The string representation of the method
   */
  public final String value;

  /**
   * Construct a non-standard HTTP request method
   * @param value the method
   */
  public HttpMethod(String value) {
    this.value = value;
  }
}
