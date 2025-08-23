namespace Teracura.TestingWebApp.Entities;

public class OAuthClientConfiguration
{
    public string Name { get; set; } = default!;
    public string ClientId { get; set; } = default!;
    public string ClientSecret { get; set; } = default!;
    public string CallbackPath { get; set; } = default!;
    public string AuthorizationEndpoint { get; set; } = default!;
    public string TokenEndpoint { get; set; } = default!;
    public string UserInformationEndpoint { get; set; } = default!;
    public string IdClaimKey { get; set; } = "id"; // e.g., "sub" for Google, "id" for GitHub
    public string NameClaimKey { get; set; } = "login"; // e.g., "name" or "login"
    public string EmailClaimKey { get; set; } = "email";
    
    // optionals
    public string? EmailEndpoint { get; set; }
    public string[]? Scopes { get; set; } = null;
}