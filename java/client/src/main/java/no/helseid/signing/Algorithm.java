package no.helseid.signing;

import no.helseid.exceptions.HelseIdException;

import java.util.Arrays;

/**
 * Supported algorithms by HelseId
 */
public enum Algorithm {
  /**
   * RSA Signature with SHA-256
   */
  RS256,
  /**
   * RSA Signature with SHA-384
   */
  RS384,
  /**
   * RSA Signature with SHA-512
   */
  RS512,
  /**
   * RSA Signature with SHA-256
   * Unlike RS256, the same JWT header and payload will generate a different signature each time.
   */
  PS256,
  /**
   * RSA Signature with SHA-384
   * Unlike RS384, the same JWT header and payload will generate a different signature each time.
   */
  PS384,
  /**
   * RSA Signature with SHA-512
   * Unlike RS512, the same JWT header and payload will generate a different signature each time.
   */
  PS512,
  /**
   * Elliptic Curve Digital Signature Algorithm with SHA-256
   */
  ES256,
  /**
   * Elliptic Curve Digital Signature Algorithm with SHA-384
   */
  ES384,
  /**
   * Elliptic Curve Digital Signature Algorithm with SHA-512
   */
  ES512;

  /**
   * Parses an algorithm to a supported object
   * @param algorithm string representation of an algorithm
   * @return a supported algorithm
   * @throws HelseIdException if the string representation does not match a supported algorithm
   */
  public static Algorithm parse(String algorithm) throws HelseIdException {
    return Arrays.stream(Algorithm.values())
        .filter(alg -> alg.name().equals(algorithm))
        .findAny()
        .orElseThrow(() -> new HelseIdException("Unsupported algorithm: " + algorithm + ". Supported algorithms: " + Arrays.toString(Algorithm.values())));
  }

  /**
   * A representation of a collection of related algorithms
   */
  public enum Family {
    /**
     * RSA Signature Algorithms
     */
    RSA(RS256, RS384, RS512, PS256, PS384, PS512),
    /**
     * Elliptic Curve Digital Signature Algorithms
     */
    EC(ES256, ES384, ES512);

    /**
     * The algorithms contained in the family
     */
    public final Algorithm[] algorithms;

    /**
     * @param algorithms the algorithms contained in the family
     */
    Family(Algorithm... algorithms) {
      this.algorithms = algorithms;
    }

    /**
     * Checks if the Algorithm family contains the provided algorithm
     * @param algorithm the algorithm which will be checked
     * @return a boolean indicating if the algorithm is within the Algorithm Family
     */
    public boolean contains(Algorithm algorithm) {
      for (Algorithm algorithm1 : this.algorithms) {
        if (algorithm1 == algorithm) return true;
      }
      return false;
    }
  }
}
