package no.helseid.signing;

import com.nimbusds.jose.JOSEException;
import com.nimbusds.jose.jwk.JWK;
import com.nimbusds.jose.jwk.KeyType;
import no.helseid.exceptions.HelseIdException;

import java.security.PrivateKey;
import java.security.PublicKey;
import java.text.ParseException;
import java.util.UUID;

/**
 * A key reference to a JWK
 */
public final class JWKKeyReference implements KeyReference {
  private final JWK jwk;
  private final Algorithm algorithm;
  private final String keyId;

  /**
   * Creates a static reference to a json web key with a specified algorithm
   *
   * @param jwk       the json web key
   * @param algorithm the elliptic curve algorithm
   * @param keyId     the id of the key, a random id is generated if missing
   */
  public JWKKeyReference(JWK jwk, Algorithm algorithm, String keyId) {
    this.jwk = jwk;
    this.algorithm = algorithm;
    this.keyId = keyId == null ? UUID.randomUUID().toString() : keyId;
  }

  /**
   * Creates a static JWKKeyReference from a string representation of a json web key
   *
   * @param jsonWebKey string representation of a json web key
   * @return a static JWKKeyReference
   * @throws HelseIdException if the string json web key is poorly formatted or missing essential fields
   */
  public static JWKKeyReference parse(String jsonWebKey) throws HelseIdException {
    try {
      JWK parsedJwk = JWK.parse(jsonWebKey);

      if (parsedJwk.getKeyID() == null) {
        throw new HelseIdException("Missing key-id in jwk");
      }

      var rawAlgorithm = parsedJwk.getAlgorithm();
      if (rawAlgorithm == null) {
        throw new HelseIdException("Missing algorithm in jwk");
      }

      Algorithm algorithm = Algorithm.parse(rawAlgorithm.getName());
      return new JWKKeyReference(parsedJwk, algorithm, parsedJwk.getKeyID());
    } catch (ParseException e) {
      throw new HelseIdException("Bad string representation of a jwk", e);
    }
  }

  @Override
  public PrivateKey getPrivateKey() throws HelseIdException {
    try {
      if (jwk.getKeyType().equals(KeyType.RSA)) {
        return jwk.toRSAKey().toPrivateKey();
      } else if (jwk.getKeyType().equals(KeyType.EC)) {
        return jwk.toECKey().toPrivateKey();
      }
    } catch (JOSEException e) {
      throw new RuntimeException(e);
    }
    throw new HelseIdException("Unsupported key type in JWK");
  }

  @Override
  public PublicKey getPublicKey() throws HelseIdException {
    try {
      if (jwk.getKeyType().equals(KeyType.RSA)) {
        return jwk.toRSAKey().toPublicKey();
      } else if (jwk.getKeyType().equals(KeyType.EC)) {
        return jwk.toECKey().toPublicKey();
      }
      throw new HelseIdException("Unsupported key type in JWK");
    } catch (JOSEException e) {
      throw new RuntimeException(e);
    }
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
