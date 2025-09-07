using Microsoft.EntityFrameworkCore;
using NizamDesk.Entities.Companies;
using NizamDesk.Entities.Projects;
using NizamDesk.Entities.Roles;
using NizamDesk.Entities.Users;

namespace NizamDesk.Logic.Data;

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
    public DbSet<ExternalLogin> ExternalLogins { get; set; }
    
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
        
        modelBuilder.Entity<ExternalLogin>(b =>
        {
            b.HasKey(x => x.Id);

            b.HasOne(x => x.User)
                .WithMany(u => u.ExternalLogins)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
 
            b.HasIndex(x => new { x.Provider, x.ProviderId }).IsUnique();

            b.Property(x => x.Provider).IsRequired();
            b.Property(x => x.ProviderId).IsRequired();
        });

        modelBuilder.Entity<User>(b =>
        {
            b.HasIndex(u => u.Email).IsUnique();
            b.Property(u => u.Name).IsRequired();
            b.Property(u => u.Email).IsRequired();
            b.Property(u => u.PasswordHash).IsRequired();
        });
    }
}