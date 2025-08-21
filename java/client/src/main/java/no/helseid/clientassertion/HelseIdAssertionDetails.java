package no.helseid.clientassertion;

public class HelseIdAssertionDetails {
  final String parentOrganizationNumber;
  final String childOrganizationNumber;
  final String sfmJournalId;

  public HelseIdAssertionDetails(String parentOrganizationNumber, String childOrganizationNumber, String sfmJournalId) {
    this.parentOrganizationNumber = parentOrganizationNumber;
    this.childOrganizationNumber = childOrganizationNumber;
    this.sfmJournalId = sfmJournalId;
  }
}
