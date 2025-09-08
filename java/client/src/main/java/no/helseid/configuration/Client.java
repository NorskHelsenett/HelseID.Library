package no.helseid.configuration;

import no.helseid.signing.KeyReference;

import java.util.List;

/**
 * Representation of a HelseId Client
 * @param clientId the client id listed in HelseId Selv-Service
 * @param keyReference a key reference to
 * @param scope a list of scopes requested for the client
 */
public record Client(String clientId, KeyReference keyReference, List<String> scope) {
}
