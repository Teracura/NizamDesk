namespace Teracura.TestingWebApp.Entities.Users;

public class User
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public string PasswordHash { get; set; } = null;
    public IEnumerable<ExternalLogin>? ExternalLogins { get; set; }
}