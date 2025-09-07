namespace NizamDesk.Entities.Projects;

public class ProjectMembership
{
    public Guid ProjectId { get; set; }
    public Guid UserId { get; set; }
    public Guid? RoleId { get; set; } //made so all users with a role can enter this project
}