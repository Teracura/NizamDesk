namespace NizamDesk.Entities.Roles;

[Flags]
public enum Permissions
{
    None = 0,
    ManageTasks = 1 << 0,
    ManageUsers = 1 << 1,
    ModerateChats = 1 << 2,
    ManageProjects = 1 << 3,
    ManageTickets = 1 << 4,
    Chat = 1 << 5
} //to get the total permission of all roles a user has, use bitwise OR