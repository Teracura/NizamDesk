namespace NizamDesk.API.EndpointEntities;

public record UserCreateRequest
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public string? Password { get; set; }
}