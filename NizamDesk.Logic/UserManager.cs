using Microsoft.EntityFrameworkCore;
using Teracura.TestingWebApp.Entities;
using Teracura.TestingWebApp.Entities.Users;
using Teracura.TestingWebApp.Logic.Data;

namespace Teracura.TestingWebApp.Logic;

public class UserManager(IDbContextFactory<AppDbContext> dbFactory)
{
    public async Task<User> LinkExternalUserAsync(ExternalUserInfo info)
    {
        var db = await dbFactory.CreateDbContextAsync();
        if (await ExternalLoginExistsAsync(info.Provider, info.ProviderId))
        {
            var matchingLogin = await db.ExternalLogins.Include(u => u.User)
                .FirstOrDefaultAsync(l => l.Provider == info.Provider && l.ProviderId == info.ProviderId);
            return matchingLogin!.User;
        }

        var email = info.Email;

        if (string.IsNullOrEmpty(email)) throw new InvalidOperationException("Email is required to create a user.");

        User user;
        if (await EmailExistsAsync(email))
        {
            user = (await GetUserAsync(email))!;
        }
        else
        {
            user = new User
            {
                Id = Guid.NewGuid(),
                Name = info.Name!,
                Email = email,
                PasswordHash = null,
                Salt = null
            };
            await RegisterUserAsync(user);
        }

        var login = new ExternalLogin
        {
            Id = Guid.NewGuid(),
            User = user,
            UserId = user.Id,
            Provider = info.Provider,
            ProviderId = info.ProviderId
        };
        await db.ExternalLogins.AddAsync(login);
        await db.SaveChangesAsync();
        return user;
    }

    public async Task RegisterUserAsync(User user)
    {
        var db = await dbFactory.CreateDbContextAsync();
        var currentUser = await GetUserAsync(user.Email);

        if (currentUser is null)
        {
            user.Id = Guid.NewGuid();
            await db.Users.AddAsync(user);
            await db.SaveChangesAsync();
            return;
        }

        if (user.PasswordHash is not null)
        {
            currentUser.PasswordHash = user.PasswordHash;
            currentUser.Salt = user.Salt;
            await db.SaveChangesAsync();
        }
    }

    private async Task<bool> EmailExistsAsync(User user)
    {
        var db = await dbFactory.CreateDbContextAsync();
        return await db.Users.AnyAsync(u => u.Email == user.Email);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        var db = await dbFactory.CreateDbContextAsync();
        return await db.Users.AnyAsync(u => u.Email == email);
    }

    public async Task<bool> ExternalLoginExistsAsync(ExternalLogin login)
    {
        var db = await dbFactory.CreateDbContextAsync();
        return await db.ExternalLogins
            .AnyAsync(l => l.Provider == login.Provider && l.ProviderId == login.ProviderId);
    }

    public async Task<bool> InternalLoginExistsAsync(string email)
    {
        var db = await dbFactory.CreateDbContextAsync();
        return await db.Users.AnyAsync(u => u.Email == email && u.PasswordHash != null);
    }

    public async Task<List<ExternalLogin>> GetExternalLoginsAsync(User user)
    {
        var db = await dbFactory.CreateDbContextAsync();
        return await db.ExternalLogins
            .Where(l => l.UserId == user.Id)
            .ToListAsync();
    }

    public async Task<bool> ExternalLoginExistsAsync(string provider, string providerId)
    {
        var db = await dbFactory.CreateDbContextAsync();
        return await db.ExternalLogins.Include(u => u.User)
            .AnyAsync(l => l.Provider == provider && l.ProviderId == providerId);
    }

    public void DeleteExternalLogin(ExternalLogin login)
    {
        var db = dbFactory.CreateDbContext();
        db.ExternalLogins.Remove(login);
    }

    public void DeleteUser(User user)
    {
        var db = dbFactory.CreateDbContext();
        db.Users.Remove(user);
    }

    public async Task<User?> GetUserAsync(string email)
    {
        var db = await dbFactory.CreateDbContextAsync();
        return await db.Users.SingleOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetUserAsyncIfNotInternal(string email)
    {
        var db = await dbFactory.CreateDbContextAsync();
        return await db.Users.FirstOrDefaultAsync(u => u.Email == email && u.PasswordHash != null);
    }
}