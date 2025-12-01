using Teracura.TestingWebApp.Entities.DataScheme.Roles;

namespace Teracura.TestingWebApp.Entities.Models;

public record RoleModel
{
    public required string Name { get; set; }
    public required Permissions Permissions { get; set; }
}