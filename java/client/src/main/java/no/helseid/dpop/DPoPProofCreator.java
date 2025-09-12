package no.helseid.dpop;

import no.helseid.exceptions.HelseIdException;

import java.net.URI;

/**
 * Interface for creation of DPoP-proofs
 */
public interface DPoPProofCreator {

  /**
   * Create a DPoP-proof with or without nonce
   * @param htu the URL of an HTTP request
   * @param htm the HTTP method of an HTTP request
   * @param nonce the nonce provided by HelseID
   * @return a signed DPoP-proof with nonce if provided
   * @throws HelseIdException if creation of a DPoP-proof failed
   */
  String createDPoPProofWithNonce(URI htu, HttpMethod htm, String nonce) throws HelseIdException;

  /**
   * Create a DPoP-proof with an access token
   * @param htu the URL of an HTTP request
   * @param htm the HTTP method of an HTTP request
   * @param accessToken an access token bound to the private key signing the DPoP-proof
   * @return a signed DPoP-proof bound to an access token
   * @throws HelseIdException if creation of a DPoP-proof failed
   */
  String createDPoPProof(URI htu, HttpMethod htm, String accessToken) throws HelseIdException;

  /**
   * Access the key-id of the signing key
   * @return the key-id of the signing key
   */
  String getKeyId();
}
