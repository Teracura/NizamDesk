namespace NizamDesk.API.EndpointEntities;

public record UserResponse
{
    public required Guid Id { get; set; }
    public required string Name { get; set; } = null!;
    public required string Email { get; set; } = null!;
}