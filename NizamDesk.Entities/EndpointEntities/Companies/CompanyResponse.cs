namespace NizamDesk.API.EndpointEntities.Companies;

public record CompanyResponse
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
}