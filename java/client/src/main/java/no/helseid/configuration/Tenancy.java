package no.helseid.configuration;

/**
 * The client tenancy in HelseID
 */
public enum Tenancy {
  /**
   * A single tenant is bound to a single legal entity
   */
  SINGLE_TENANT,
  /**
   * A multi tenant client might represent another legal entity if the right to do so is delegated by the legal entity to the tenant
   */
  MULTI_TENANT
}