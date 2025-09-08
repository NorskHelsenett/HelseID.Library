package no.helseid.signing;

import com.nimbusds.jose.JOSEException;
import com.nimbusds.jose.JWSAlgorithm;
import com.nimbusds.jose.JWSSigner;
import com.nimbusds.jose.crypto.ECDSASigner;
import com.nimbusds.jose.crypto.RSASSASigner;
import com.nimbusds.jose.jwk.*;
import no.helseid.exceptions.HelseIdException;

import java.security.interfaces.ECPrivateKey;
import java.security.interfaces.ECPublicKey;
import java.security.interfaces.RSAPrivateKey;
import java.security.interfaces.RSAPublicKey;

import static com.nimbusds.jose.jwk.Curve.*;

/**
 * Util class for converting key reference to nimbus
 */
public interface Util {
  /**
   * Internal use only
   * Converts a key reference to a nimbus JWSSigner
   *
   * @param keyReference the key reference to be converted
   * @return a JWSSigner based on the key reference
   * @throws HelseIdException if the conversion is unsuccessful
   */
  static JWSSigner createJWSSignerFromKeyReference(KeyReference keyReference) throws HelseIdException {
    if (no.helseid.signing.Algorithm.Family.RSA.contains(keyReference.getAlgorithm())) {
      return new RSASSASigner(keyReference.getPrivateKey());
    }

    if (Algorithm.Family.EC.contains(keyReference.getAlgorithm())) {
      try {
        return new ECDSASigner((ECPrivateKey) keyReference.getPrivateKey());
      } catch (JOSEException e) {
        throw new HelseIdException("Unsupported algorithm in elliptic-curve private key", e);
      }
    }
    throw new HelseIdException("Unsupported algorithm");
  }

  /**
   * Internal use only
   * Converts a key reference to a nimbus JWK
   * @param keyReference the key reference to be converted
   * @return a JWK based on the key reference
   * @throws HelseIdException if the conversion is unsuccessful
   */
  static JWK createJWKFromKeyReference(KeyReference keyReference) throws HelseIdException {
    if (Algorithm.Family.RSA.contains(keyReference.getAlgorithm())) {
      return new RSAKey.Builder((RSAPublicKey) keyReference.getPublicKey())
          .algorithm(JWSAlgorithm.parse(keyReference.getAlgorithm().name()))
          .privateKey((RSAPrivateKey) keyReference.getPrivateKey())
          .keyID(keyReference.getKeyId())
          .keyUse(KeyUse.SIGNATURE)
          .build();
    }
    if (Algorithm.Family.EC.contains(keyReference.getAlgorithm())) {
      return new ECKey.Builder(findCurveForAlgorithm(keyReference.getAlgorithm()), (ECPublicKey) keyReference.getPublicKey())
          .algorithm(JWSAlgorithm.parse(keyReference.getAlgorithm().name()))
          .privateKey((ECPrivateKey) keyReference.getPrivateKey())
          .keyID(keyReference.getKeyId())
          .keyUse(KeyUse.SIGNATURE)
          .build();
    }
    throw new HelseIdException("Unsupported algorithm");
  }


  /**
   * Internal use only
   * @param algorithm the elliptic curve algorithm
   * @return a Curve matching the provided algorithm
   * @throws HelseIdException if the algorithm does not have any matching curve
   */
  static Curve findCurveForAlgorithm(Algorithm algorithm) throws HelseIdException {
    if (Algorithm.ES256.equals(algorithm)) {
      return P_256;
    } else if (Algorithm.ES384.equals(algorithm)) {
      return P_384;
    } else if (Algorithm.ES512.equals(algorithm)) {
      return P_521;
    } else {
      throw new HelseIdException("Unsupported elliptic curve algorithm");
    }
  }
}
