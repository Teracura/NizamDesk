using Teracura.TestingWebApp.Entities.DataScheme.Roles;

namespace Teracura.TestingWebApp.Entities.Roles;

public class Role
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required Permissions Permissions { get; set; }
    public required Guid CompanyId { get; set; }
    public required int HierarchyLevel { get; set; } // lower number = higher role
}