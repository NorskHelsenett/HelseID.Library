namespace HelseId.Standard.Models.DetailsFromClient;

public class OrganizationNumbers
{
    public OrganizationNumbers()
    {
        ParentOrganization = "";
        ChildOrganization = "";
    }

    public OrganizationNumbers(string parentOrganization, string childOrganization)
    {
        ParentOrganization = parentOrganization;
        ChildOrganization = childOrganization;
    }
    
    public string ParentOrganization { get; init; }
    public string ChildOrganization { get; init; }
    public bool HasOrganizationNumbers => !string.IsNullOrEmpty(ParentOrganization) || !string.IsNullOrEmpty(ChildOrganization);
}
