namespace NizamDesk.API.EndpointEntities.Companies;

public record CompanyUpdateRequest
{
    public string? Name { get; set; }
    public string? Password { get; set; }
}