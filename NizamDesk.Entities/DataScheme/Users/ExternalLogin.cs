using Teracura.TestingWebApp.Entities.Users;

namespace Teracura.TestingWebApp.Entities.DataScheme.Users;

public class ExternalLogin
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; } 
    public required User User { get; set; }

    public required string Provider { get; set; }
    public required string ProviderId { get; set; }
}
