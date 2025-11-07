using System.ComponentModel.DataAnnotations;

namespace Teracura.TestingWebApp.Entities.DataScheme.Companies;

public class Company
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required byte[] EntryPassword { get; set; }
    public required byte[] EntrySalt { get; set; }
}