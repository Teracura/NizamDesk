using Microsoft.EntityFrameworkCore;
using Shouldly;
using NizamDesk.Entities.Companies;
using NizamDesk.Entities.Projects;
using NizamDesk.Entities.Roles;
using NizamDesk.Entities.Users;
using NizamDesk.Logic.Data;

namespace NizamDesk.Tests;

public class DatabaseTests
{
    private readonly AppDbContext _dbContext;

    public DatabaseTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb") // in-memory DB
            .Options;

        _dbContext = new AppDbContext(options);
    }

    [Fact]
    public void Should_Create_And_Delete_Database_Data()
    {
        _dbContext.Database.EnsureCreated();

        _dbContext.Companies.Add(new Company
        {
            Id = Guid.NewGuid(),
            Name = "Test Company"
        });
        _dbContext.SaveChanges();

        _dbContext.Companies.Count().ShouldBe(1);

        _dbContext.Database.EnsureDeleted();

        _dbContext.Companies.Count().ShouldBe(0);
    }

    [Fact]
    public void Should_Add_And_Remove_Roles()
    {
        _dbContext.Database.EnsureCreated();

        var perms = Permissions.None | Permissions.ManageTasks | Permissions.ManageUsers | Permissions.ManageProjects |
                    Permissions.Chat;

        var role = new Role
        {
            Id = Guid.NewGuid(),
            Name = "Test Role",
            Permissions = perms,
            CompanyId = Guid.NewGuid(),
            HierarchyLevel = 1
        };

        _dbContext.Roles.Add(role);

        _dbContext.Roles.Remove(role);

        _dbContext.Roles.Find(role.Id).ShouldBeNull();
    }

    [Fact]
    public void Should_Add_And_Remove_Users()
    {
        _dbContext.Database.EnsureCreated();

        var user = new User
        {
            Email = "thisisatestemail@outlook.com",
            Name = "Test User",
            PasswordHash = "password",
            Id = Guid.NewGuid()
        };

        _dbContext.Users.Add(user);

        _dbContext.Users.Remove(user);

        _dbContext.Users.Find(user.Id).ShouldBeNull();
    }

    [Fact]
    public void Should_Add_And_Remove_Companies()
    {
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = "Test Company",
            EntryPassword = "1234"
        };

        _dbContext.Companies.Add(company);

        _dbContext.Companies.Remove(company);

        _dbContext.Companies.Find(company.Id).ShouldBeNull();
    }

    [Fact]
    public void Should_Add_And_Remove_CompanyMemberships()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();

        var membership = new CompanyMembership
        {
            UserId = userId,
            CompanyId = companyId
        };

        _dbContext.CompanyMemberships.Add(membership);

        _dbContext.CompanyMemberships.Remove(membership);

        _dbContext.CompanyMemberships.FirstOrDefault(cm => cm.UserId == userId && cm.CompanyId == companyId)
            .ShouldBeNull();
    }

    [Fact]
    public void Should_Add_And_Remove_Projects()
    {
        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = "Test Project",
            CompanyId = Guid.NewGuid()
        };

        _dbContext.Projects.Add(project);

        _dbContext.Projects.Remove(project);

        _dbContext.Projects.Find(project.Id).ShouldBeNull();
    }

    [Fact]
    public void Should_Add_And_Remove_ProjectMemberships()
    {
        var userId = Guid.NewGuid();
        var projectId = Guid.NewGuid();

        var membership = new ProjectMembership
        {
            UserId = userId,
            ProjectId = projectId
        };

        _dbContext.ProjectMemberships.Add(membership);

        _dbContext.ProjectMemberships.Remove(membership);

        _dbContext.ProjectMemberships.FirstOrDefault(pm => pm.UserId == userId && pm.ProjectId == projectId)
            .ShouldBeNull();
    }

    [Fact]
    public void Should_Add_And_Remove_UserRoles()
    {
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var userRole = new UserRole
        {
            UserId = userId,
            RoleId = roleId
        };

        _dbContext.UserRoles.Add(userRole);

        _dbContext.UserRoles.Remove(userRole);

        _dbContext.UserRoles.FirstOrDefault(ur => ur.UserId == userId && ur.RoleId == roleId).ShouldBeNull();
    }

    [Fact]
    public void Should_Add_And_Remove_Tickets()
    {
        var ticket = new Ticket
        {
            Id = Guid.NewGuid(),
            Title = "Test Ticket",
            ProjectId = Guid.NewGuid(),
            CreatorId = Guid.NewGuid(),
            AssignedUserId = null,
            Description = "Test description",
            Status = TicketStatus.Open
        };

        _dbContext.Tickets.Add(ticket);

        _dbContext.Tickets.Remove(ticket);

        _dbContext.Tickets.Find(ticket.Id).ShouldBeNull();
    }

    [Fact]
    public void Should_Assign_Role_to_User()
    {
        _dbContext.Database.EnsureCreated();

        var user1 = new User { Id = Guid.NewGuid(), Name = "User1", Email = "user1@test.com", PasswordHash = "pass1" };
        var user2 = new User { Id = Guid.NewGuid(), Name = "User2", Email = "user2@test.com", PasswordHash = "pass2" };
        var user3 = new User { Id = Guid.NewGuid(), Name = "User3", Email = "user3@test.com", PasswordHash = "pass3" };

        _dbContext.Users.AddRange(user1, user2, user3);

        var adminRole = new Role
        {
            Id = Guid.NewGuid(),
            Name = "Admin",
            Permissions = Permissions.ManageUsers | Permissions.ManageProjects | Permissions.ManageTasks,
            CompanyId = Guid.NewGuid(),
            HierarchyLevel = 1
        };
        var memberRole = new Role
        {
            Id = Guid.NewGuid(),
            Name = "Member",
            Permissions = Permissions.Chat | Permissions.ManageTasks,
            CompanyId = adminRole.CompanyId,
            HierarchyLevel = 2
        };

        _dbContext.Roles.AddRange(adminRole, memberRole);

        _dbContext.UserRoles.AddRange(
            new UserRole { UserId = user1.Id, RoleId = memberRole.Id },
            new UserRole { UserId = user2.Id, RoleId = adminRole.Id },
            new UserRole { UserId = user3.Id, RoleId = adminRole.Id },
            new UserRole { UserId = user3.Id, RoleId = memberRole.Id }
        );
        _dbContext.SaveChanges();

        var user1Roles = _dbContext.UserRoles.Where(ur => ur.UserId == user1.Id).Select(ur => ur.RoleId).ToList();
        var user2Roles = _dbContext.UserRoles.Where(ur => ur.UserId == user2.Id).Select(ur => ur.RoleId).ToList();
        var user3Roles = _dbContext.UserRoles.Where(ur => ur.UserId == user3.Id).Select(ur => ur.RoleId).ToList();

        user1Roles.ShouldContain(memberRole.Id);
        user1Roles.ShouldNotContain(adminRole.Id);

        user2Roles.ShouldContain(adminRole.Id);
        user2Roles.ShouldNotContain(memberRole.Id);

        user3Roles.ShouldContain(adminRole.Id);
        user3Roles.ShouldContain(memberRole.Id);
    }

    [Fact]
    public void Should_Assign_Project_to_User()
    {
        _dbContext.Database.EnsureCreated();

        var user = new User { Id = Guid.NewGuid(), Name = "User", Email = "user@test.com", PasswordHash = "pass" };
        var company = new Company { Id = Guid.NewGuid(), Name = "Test Company" };
        var project = new Project { Id = Guid.NewGuid(), Name = "Test Project", CompanyId = company.Id };

        _dbContext.Users.Add(user);
        _dbContext.Companies.Add(company);
        _dbContext.Projects.Add(project);
        _dbContext.SaveChanges();

        var membership = new ProjectMembership { UserId = user.Id, ProjectId = project.Id };
        _dbContext.ProjectMemberships.Add(membership);
        _dbContext.SaveChanges();

        _dbContext.ProjectMemberships.Find(user.Id, project.Id).ShouldNotBeNull();
    }

    [Fact]
    public void Should_User_Create_Ticket()
    {
        _dbContext.Database.EnsureCreated();

        var user = new User { Id = Guid.NewGuid(), Name = "User", Email = "user@test.com", PasswordHash = "pass" };
        var company = new Company { Id = Guid.NewGuid(), Name = "Test Company" };
        var project = new Project { Id = Guid.NewGuid(), Name = "Test Project", CompanyId = company.Id };

        _dbContext.Users.Add(user);
        _dbContext.Companies.Add(company);
        _dbContext.Projects.Add(project);

        var ticket = new Ticket
        {
            Id = Guid.NewGuid(),
            Title = "Fix Bug",
            Description = "Critical issue",
            ProjectId = project.Id,
            CreatorId = user.Id
        };

        _dbContext.Tickets.Add(ticket);

        _dbContext.Tickets.Find(ticket.Id).ShouldNotBeNull();
    }

    [Fact]
    public void Should_User_Take_Ticket()
    {
        _dbContext.Database.EnsureCreated();

        var creator = new User
            { Id = Guid.NewGuid(), Name = "Creator", Email = "creator@test.com", PasswordHash = "pass" };
        var assignee = new User
            { Id = Guid.NewGuid(), Name = "Assignee", Email = "assignee@test.com", PasswordHash = "pass" };
        var company = new Company { Id = Guid.NewGuid(), Name = "Test Company" };
        var project = new Project { Id = Guid.NewGuid(), Name = "Test Project", CompanyId = company.Id };

        _dbContext.Users.AddRange(creator, assignee);
        _dbContext.Companies.Add(company);
        _dbContext.Projects.Add(project);

        var ticket = new Ticket
        {
            Id = Guid.NewGuid(),
            Title = "Implement Feature",
            Description = "New module",
            ProjectId = project.Id,
            CreatorId = creator.Id
        };

        _dbContext.Tickets.Add(ticket);

        ticket.AssignedUserId = assignee.Id;
        ticket.Status = TicketStatus.InProgress;
        _dbContext.Tickets.Update(ticket);

        var dbTicket = _dbContext.Tickets.Find(ticket.Id);
        dbTicket!.AssignedUserId.ShouldBe(assignee.Id);
        dbTicket.Status.ShouldBe(TicketStatus.InProgress);
    }

    [Fact]
    public void Should_User_Complete_Ticket()
    {
        _dbContext.Database.EnsureCreated();

        var user = new User { Id = Guid.NewGuid(), Name = "User", Email = "user@test.com", PasswordHash = "pass" };
        var company = new Company { Id = Guid.NewGuid(), Name = "Test Company" };
        var project = new Project { Id = Guid.NewGuid(), Name = "Test Project", CompanyId = company.Id };

        _dbContext.Users.Add(user);
        _dbContext.Companies.Add(company);
        _dbContext.Projects.Add(project);
        _dbContext.SaveChanges();

        var ticket = new Ticket
        {
            Id = Guid.NewGuid(),
            Title = "Write Tests",
            Description = "Finish unit tests",
            ProjectId = project.Id,
            CreatorId = user.Id,
            AssignedUserId = user.Id,
            Status = TicketStatus.InProgress
        };

        _dbContext.Tickets.Add(ticket);
        _dbContext.SaveChanges();

        ticket.Status = TicketStatus.Closed;
        _dbContext.Tickets.Update(ticket);
        _dbContext.SaveChanges();

        _dbContext.Tickets.Find(ticket.Id)!.Status.ShouldBe(TicketStatus.Closed);
    }

    [Fact]
    public void Should_User_Abort_Ticket()
    {
        _dbContext.Database.EnsureCreated();

        var user = new User { Id = Guid.NewGuid(), Name = "User", Email = "user@test.com", PasswordHash = "pass" };
        var company = new Company { Id = Guid.NewGuid(), Name = "Test Company" };
        var project = new Project { Id = Guid.NewGuid(), Name = "Test Project", CompanyId = company.Id };

        _dbContext.Users.Add(user);
        _dbContext.Companies.Add(company);
        _dbContext.Projects.Add(project);
        _dbContext.SaveChanges();

        var ticket = new Ticket
        {
            Id = Guid.NewGuid(),
            Title = "Research Feature",
            Description = "User tried but couldn’t finish",
            ProjectId = project.Id,
            CreatorId = user.Id,
            AssignedUserId = user.Id,
            Status = TicketStatus.InProgress
        };

        _dbContext.Tickets.Add(ticket);
        _dbContext.SaveChanges();

        ticket.Status = TicketStatus.Open;
        ticket.AssignedUserId = null;
        _dbContext.Tickets.Update(ticket);
        _dbContext.SaveChanges();

        var reloaded = _dbContext.Tickets.Find(ticket.Id)!;
        reloaded.Status.ShouldBe(TicketStatus.Open);
        reloaded.AssignedUserId.ShouldBeNull();
    }
}