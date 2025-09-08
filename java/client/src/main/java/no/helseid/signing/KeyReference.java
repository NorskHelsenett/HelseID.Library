package no.helseid.signing;

import no.helseid.exceptions.HelseIdException;

import java.security.PrivateKey;
import java.security.PublicKey;

/**
 * A reference to a key pair pinned to an algorithm and a key id
 */
public interface KeyReference {
  /**
   * Access the private part of the currently referenced key
   * @return Private part of the referenced key
   * @throws HelseIdException if the reference is unable to provide a private key
   */
  PrivateKey getPrivateKey() throws HelseIdException;

  /**
   * Access the public part of the currently referenced key
   * @return Public part of the referenced key
   * @throws HelseIdException if the reference is unable to provide a public key
   */
  PublicKey getPublicKey() throws HelseIdException;

  /**
   * Access the algorithm in the currently referenced key
   * @return the algorithm in the referenced key
   */
  Algorithm getAlgorithm();

  /**
   * Access the id of the currently referenced key
   * @return the id of the referenced key
   */
  String getKeyId();
}