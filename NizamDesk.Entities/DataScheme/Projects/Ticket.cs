using Teracura.TestingWebApp.Entities.Projects;
using Teracura.TestingWebApp.Entities.Users;

namespace Teracura.TestingWebApp.Entities.DataScheme.Projects;

public class Ticket
{
    public Guid Id { get; set; }

    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public Guid CreatorId { get; set; }
    public User Creator { get; set; } = null!;

    public Guid? AssignedUserId { get; set; }
    public User? AssignedUser { get; set; }

    public required string Title { get; set; }
    public string? Description { get; set; }
    public TicketStatus Status { get; set; } = TicketStatus.Open;
}