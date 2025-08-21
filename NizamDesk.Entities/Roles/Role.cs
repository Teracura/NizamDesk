namespace Teracura.TestingWebApp.Entities.Roles;

public class Role
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public Permissions Permissions { get; set; }
    public Guid CompanyId { get; set; }
    public int HierarchyLevel { get; set; } // Higher number = higher role
}