namespace Teracura.TestingWebApp.Entities.Companies;

public class CompanyMembership
{
    public required Guid CompanyId { get; set; }
    public required Guid UserId { get; set; }
}