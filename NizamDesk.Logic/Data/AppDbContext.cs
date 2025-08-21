using Teracura.TestingWebApp.Entities.Companies;
using Teracura.TestingWebApp.Entities.Projects;
using Teracura.TestingWebApp.Entities.Roles;
using Teracura.TestingWebApp.Entities.Users;

namespace Teracura.TestingWebApp.Logic.Data;

using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<Company> Companies { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<CompanyMembership> CompanyMemberships { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectMembership> ProjectMemberships { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

        modelBuilder.Entity<CompanyMembership>()
            .HasKey(cm => new { cm.UserId, cm.CompanyId });

        modelBuilder.Entity<ProjectMembership>()
            .HasKey(pm => new { pm.UserId, pm.ProjectId });
        
        // Company - Projects (one-to-many)
        modelBuilder.Entity<Project>()
            .HasOne<Company>()
            .WithMany()
            .HasForeignKey(p => p.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ticket - Project (one-to-many)
        modelBuilder.Entity<Ticket>()
            .HasOne<Project>()
            .WithMany()
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ticket - User (creator, assigned)
        modelBuilder.Entity<Ticket>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(t => t.CreatorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Ticket>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(t => t.AssignedUserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}