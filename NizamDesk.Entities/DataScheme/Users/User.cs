namespace Teracura.TestingWebApp.Entities.Users;

public class User
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public byte[]? PasswordHash { get; set; }
    public byte[]? Salt { get; set; }
    public IEnumerable<ExternalLogin>? ExternalLogins { get; set; }
}