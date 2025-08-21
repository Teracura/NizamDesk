namespace Teracura.TestingWebApp.Entities.Projects;

public class Ticket
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid CreatorId { get; set; }
    public Guid? AssignedUserId { get; set; } // null if unassigned
    public required string Title { get; set; }
    public string? Description { get; set; }
    public TicketStatus Status { get; set; } = TicketStatus.Open;
}