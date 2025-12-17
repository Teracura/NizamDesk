namespace Teracura.TestingWebApp.Entities.DataScheme.Companies;

public class CompanyMembership
{
    public required Guid CompanyId { get; set; }
    public required Guid UserId { get; set; }
}