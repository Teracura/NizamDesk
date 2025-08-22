namespace Teracura.TestingWebApp.Entities.Companies;

public class Company
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? EntryPassword { get; set; }
}