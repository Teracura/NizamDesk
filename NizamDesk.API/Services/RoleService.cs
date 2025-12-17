using Microsoft.EntityFrameworkCore;
using NizamDesk.API.Data;
using Teracura.TestingWebApp.Entities.Models;
using Teracura.TestingWebApp.Entities.Roles;
using Teracura.TestingWebApp.Entities.Users;

namespace NizamDesk.API.Services;

public class RoleService(IDbContextFactory<AppDbContext> dbContextFactory)
{
    public async Task<UserRole?> GrantRoleUserAsync(Guid companyId, Guid userId, Guid roleId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

        var roleExists = await db.Roles.AnyAsync(r => r.Id == roleId && r.CompanyId == companyId).ConfigureAwait(false);
        var userExists = await db.Users.AnyAsync(u => u.Id == userId).ConfigureAwait(false);

        var userRole = new UserRole()
        {
            RoleId = roleId,
            UserId = userId,
        };

        if (!roleExists || !userExists) return null;

        await db.UserRoles.AddAsync(userRole).ConfigureAwait(false);
        await db.SaveChangesAsync().ConfigureAwait(false);
        return userRole;
    }

    public async Task<bool> RemoveRoleUserAsync(Guid companyId, Guid userId, Guid roleId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

        var roleExists = await db.Roles.AsNoTracking().AnyAsync(r => r.Id == roleId && r.CompanyId == companyId).ConfigureAwait(false);
        var userExists = await db.Users.AsNoTracking().AnyAsync(u => u.Id == userId).ConfigureAwait(false);
        
        if (!roleExists || !userExists) return false;

        var userRole = await db.UserRoles.FindAsync(userId, roleId).ConfigureAwait(false);
        if (userRole == null) return false;
        db.UserRoles.Remove(userRole);
        await db.SaveChangesAsync().ConfigureAwait(false);
        return true;
    }

    public async Task<Role?> CreateRoleAsync(Guid companyId, RoleModel model)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

        var companyExists = await db.Companies.AsNoTracking().AnyAsync(c => c.Id == companyId).ConfigureAwait(false);
        var id = Guid.NewGuid();
        var rolesCount = await db.Roles.AsNoTracking().CountAsync(r => r.CompanyId == companyId).ConfigureAwait(false);
        var role = new Role()
        {
            CompanyId = companyId,
            Id = id,
            Name = model.Name,
            Permissions = model.Permissions,
            HierarchyLevel = rolesCount
        };
        if (!companyExists) return null;
        await db.Roles.AddAsync(role).ConfigureAwait(false);
        await db.SaveChangesAsync().ConfigureAwait(false);
        return role;
    }
}