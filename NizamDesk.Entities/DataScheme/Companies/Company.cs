namespace Teracura.TestingWebApp.Entities.Companies;

public class Company
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public byte[]? EntryPassword { get; set; }
    public byte[]? EntrySalt { get; set; }
}