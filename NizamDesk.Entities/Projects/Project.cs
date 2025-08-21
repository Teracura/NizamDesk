namespace Teracura.TestingWebApp.Entities.Projects;

public class Project
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public Guid CompanyId { get; set; }
}