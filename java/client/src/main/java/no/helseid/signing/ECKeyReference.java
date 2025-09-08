package no.helseid.signing;

import no.helseid.exceptions.HelseIdException;

import java.security.*;
import java.util.UUID;

/**
 * A Key reference to a elliptic curve key pair
 */
public final class ECKeyReference implements KeyReference {
  private static final String ALGORITHM = "EC";
  private final KeyPair keyPair;
  private final Algorithm algorithm;
  private final String keyId;

  /**
   * Creates a static reference to an elliptic curve key pair with a specified algorithm
   *
   * @param algorithm the elliptic curve algorithm
   * @param keyPair   the elliptic curve key pair
   * @param keyId     the id of the key
   * @throws HelseIdException if the algorithm is not compatible with elliptic curve
   */
  public ECKeyReference(Algorithm algorithm, KeyPair keyPair, String keyId) throws HelseIdException {
    assertAlgorithm(algorithm);

    this.algorithm = algorithm;
    this.keyPair = keyPair;
    this.keyId = keyId;
  }

  /**
   * Creates an elliptic curveKeyReference referencing a generated elliptic curve key
   *
   * @param algorithm the elliptic curve algorithm
   * @throws HelseIdException if the algorithm is not compatible with elliptic curve
   * @return a key reference to a generated elliptic curve key pair with the specified algorithm
   */
  public static ECKeyReference generate(Algorithm algorithm) throws HelseIdException {
    try {
      assertAlgorithm(algorithm);
      var keySize = getKeySizeFromAlgorithm(algorithm);
      KeyPairGenerator keyPairGenerator = KeyPairGenerator.getInstance(ALGORITHM);
      keyPairGenerator.initialize(keySize);

      return new ECKeyReference(algorithm, keyPairGenerator.generateKeyPair(), UUID.randomUUID().toString());
    } catch (NoSuchAlgorithmException e) {
      throw new RuntimeException(e);
    }
  }

  /**
   * @param algorithm an elliptic curve algorithm
   * @return the key size suited for the algorithm
   */
  private static int getKeySizeFromAlgorithm(Algorithm algorithm) {
    if (Algorithm.ES256.equals(algorithm)) {
      return 256;
    } else if (Algorithm.ES384.equals(algorithm)) {
      return 384;
    } else if (Algorithm.ES512.equals(algorithm)) {
      return 512;
    }
    throw new IllegalArgumentException("Unknown algorithm: " + algorithm);
  }

  /**
   * @param algorithm an elliptic curve algorithm
   * @throws HelseIdException if algorithm is not suited for elliptic curve
   */
  private static void assertAlgorithm(Algorithm algorithm) throws HelseIdException {
    if (!Algorithm.Family.EC.contains(algorithm)) {
      throw new HelseIdException("Algorithm must be compatible with elliptic curve");
    }
  }

  @Override
  public PrivateKey getPrivateKey() {
    return keyPair.getPrivate();
  }

  @Override
  public PublicKey getPublicKey() {
    return keyPair.getPublic();
  }

  @Override
  public Algorithm getAlgorithm() {
    return algorithm;
  }

  @Override
  public String getKeyId() {
    return keyId;
  }
}
