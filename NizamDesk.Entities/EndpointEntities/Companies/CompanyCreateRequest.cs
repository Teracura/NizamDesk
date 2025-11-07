namespace NizamDesk.API.EndpointEntities.Companies;

public record CompanyCreateRequest
{
    public required string Name { get; set; }
    public required string Password { get; set; }
}