package no.helseid.clientassertion;

import no.helseid.configuration.Tenancy;
import no.helseid.exceptions.HelseIdException;

import java.util.ArrayList;
import java.util.List;
import java.util.Map;
import java.util.Objects;
import java.util.stream.Collectors;
import java.util.stream.Stream;

/**
 * Representation of the assertion details
 */
public final class AssertionDetails {
  private final Object value;
  private final String id;

  /**
   * @param value a map or list representation of the assertion details
   * @param id    an identificator for the conent of the assertion details
   */
  private AssertionDetails(Object value, String id) {
    this.value = value;
    this.id = id;
  }

  /**
   * @param identifiers a list of unique information in an assertion details structure
   * @return an identifying string of the content
   */
  private static String createId(String... identifiers) {
    return Stream.of(identifiers)
        .filter(Objects::nonNull)
        .collect(Collectors.joining(":"));
  }

  /**
   * Access the  map/list representation of the resulting assertion details
   * @return the map/list representation of the resulting assertion details
   */
  public Object value() {
    return value;
  }

  /**
   * Access the id of the assertion details
   * @return a string identifying the assertion details
   */
  public String id() {
    return id;
  }

  /**
   * A builder class for AssertionDetails
   */
  public static class Builder {
    private final Tenancy tenancy;
    private String parentOrganizationNumber;
    private String childOrganizationNumber;
    private String sfmJournalId;

    /**
     * Initiates a build process for AssertionDetails
     * @param tenancy the client tenancy
     */
    public Builder(Tenancy tenancy) {
      this.tenancy = tenancy;
    }

    /**
     * Assingn an orgainization number representing the parent organization
     * @param parentOrganizationNumber identifies the parent organization, only relevant for multi tenant clients
     * @return the current builder
     */
    public Builder withParentOrganizationNumber(String parentOrganizationNumber) {
      this.parentOrganizationNumber = parentOrganizationNumber;
      return this;
    }

    /**
     * Assingn an orgainization number representing the child organization
     * @param childOrganizationNumber identifies the child organization
     * @return the current builder
     */
    public Builder withChildOrganizationNumber(String childOrganizationNumber) {
      this.childOrganizationNumber = childOrganizationNumber;
      return this;
    }

    /**
     * Assing a SFM-journal id representing the specific SFM journal
     * @param sfmJournalId identifies the SFM journal, only relevant in relation to use of Sentral Forskrivningsmodul
     * @return the current builder
     */
    public Builder withSfmJournalId(String sfmJournalId) {
      this.sfmJournalId = sfmJournalId;
      return this;
    }

    /**
     * Process the provided values into an AssertionDetails object
     * @return formatted assertion details used in a token request
     * @throws HelseIdException when the builder is misconfigured
     */
    public AssertionDetails build() throws HelseIdException {
      var id = createId(tenancy.name(), parentOrganizationNumber, childOrganizationNumber, sfmJournalId);
      List<Map<String, Object>> assertionDetails = new ArrayList<>();

      if (tenancy == Tenancy.SINGLE_TENANT && childOrganizationNumber != null) {
        assertionDetails.add(HelseIdAuthorizationClaimUtil.createClaimHelseidAuthorizationForSingleTenant(childOrganizationNumber));
      } else if (tenancy == Tenancy.MULTI_TENANT && parentOrganizationNumber != null) {
        assertionDetails.add(HelseIdAuthorizationClaimUtil.createClaimHelseidAuthorizationForMultiTenant(parentOrganizationNumber, childOrganizationNumber));
      }

      if (sfmJournalId != null) {
        assertionDetails.add(SFMJournalIdClaimUtil.createClaimSFMJournalId(sfmJournalId));
      }

      if (assertionDetails.isEmpty()) {
        throw new HelseIdException("The resulting assertion details is empty");
      }

      if (assertionDetails.size() == 1) {
        return new AssertionDetails(assertionDetails.get(0), id);
      }

      return new AssertionDetails(assertionDetails, id);
    }
  }
}
