package no.helseid.signing;

import no.helseid.exceptions.HelseIdException;

import java.security.*;
import java.util.UUID;

/**
 * A Key reference to an RSA key
 */
public final class RSAKeyReference implements KeyReference {
  private static final String ALGORITHM = "RSA";
  private static final int MINIMUM_KEY_SIZE = 2048;
  private final KeyPair keyPair;
  private final Algorithm algorithm;
  private final String keyId;

  /**
   * Creates a static reference to a RSA key pair with a specified algorithm
   *
   * @param algorithm the RSA algorithm
   * @param keyPair   the RSA key pair
   * @param keyId     the id of the key
   * @throws HelseIdException if the algorithm is not compatible with RSA
   */
  public RSAKeyReference(Algorithm algorithm, KeyPair keyPair, String keyId) throws HelseIdException {
    if (!Algorithm.Family.RSA.contains(algorithm)) {
      throw new HelseIdException("Algorithm must be RSA compatible");
    }
    this.algorithm = algorithm;
    this.keyPair = keyPair;
    this.keyId = keyId;
  }

  /**
   * Creates an RSAKeyReference referencing a generated RSA key
   *
   * @param algorithm the RSA algorithm
   * @throws HelseIdException if the algorithm is not compatible with RSA
   * @return a key reference to a generated RSA key pair with the specified algorithm
   */
  public static RSAKeyReference generate(Algorithm algorithm) throws HelseIdException {
    try {
      KeyPairGenerator keyPairGenerator = KeyPairGenerator.getInstance(ALGORITHM);
      keyPairGenerator.initialize(MINIMUM_KEY_SIZE);
      return new RSAKeyReference(algorithm, keyPairGenerator.generateKeyPair(), UUID.randomUUID().toString());

    } catch (NoSuchAlgorithmException e) {
      throw new RuntimeException(e);
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
