namespace NizamDesk.API.EndpointEntities.Companies;

public record JoinCompanyForm
{
    public Guid UserId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}