using Microsoft.EntityFrameworkCore;
using Teracura.TestingWebApp.Entities.DataScheme.Companies;
using Teracura.TestingWebApp.Entities.DataScheme.Projects;
using Teracura.TestingWebApp.Entities.DataScheme.Users;
using Teracura.TestingWebApp.Entities.Projects;
using Teracura.TestingWebApp.Entities.Roles;
using Teracura.TestingWebApp.Entities.Users;

namespace NizamDesk.API.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // --- USER <-> ROLE ---
        modelBuilder.Entity<UserRole>(b =>
        {
            b.HasKey(ur => new { ur.UserId, ur.RoleId });

            b.HasOne<User>()
                .WithMany()
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne<Role>()
                .WithMany()
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // --- USER <-> COMPANY ---
        modelBuilder.Entity<CompanyMembership>(b =>
        {
            b.HasKey(cm => new { cm.UserId, cm.CompanyId });

            b.HasOne<User>()
                .WithMany()
                .HasForeignKey(cm => cm.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne<Company>()
                .WithMany()
                .HasForeignKey(cm => cm.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // --- USER <-> PROJECT ---
        modelBuilder.Entity<ProjectMembership>(b =>
        {
            b.HasKey(pm => new { pm.UserId, pm.ProjectId });

            b.HasOne<User>()
                .WithMany()
                .HasForeignKey(pm => pm.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne<Project>()
                .WithMany()
                .HasForeignKey(pm => pm.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Ticket>(b =>
        {
            b.HasKey(t => t.Id);

            b.HasOne(t => t.Project)
                .WithMany()
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(t => t.Creator)
                .WithMany()
                .HasForeignKey(t => t.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(t => t.AssignedUser)
                .WithMany()
                .HasForeignKey(t => t.AssignedUserId)
                .OnDelete(DeleteBehavior.SetNull);

            b.Property(t => t.Title)
                .IsRequired();

            b.Property(t => t.Description)
                .IsRequired(false);

            b.Property(t => t.Status)
                .IsRequired()
                .HasDefaultValue(TicketStatus.Open);
            
            //unique title per project
            b.HasIndex(t => new { t.ProjectId, t.Title }).IsUnique();
        });


        // --- EXTERNAL LOGINS ---
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

        // --- USERS ---
        modelBuilder.Entity<User>(b =>
        {
            b.HasIndex(u => u.Email).IsUnique();
            b.Property(u => u.Name).IsRequired();
            b.Property(u => u.Email).IsRequired();
        });

        // --- COMPANIES ---
        modelBuilder.Entity<Company>(b =>
        {
            b.HasIndex(c => c.Name).IsUnique();
            b.Property(c => c.Name).IsRequired();
            b.Property(c => c.EntryPassword).IsRequired();
        });
    }
}