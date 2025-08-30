using Microsoft.EntityFrameworkCore;
using Teracura.TestingWebApp.Entities;
using Teracura.TestingWebApp.Entities.Users;
using Teracura.TestingWebApp.Logic.Data;

namespace Teracura.TestingWebApp.Logic;

public class UserManager(AppDbContext db)
{
    // gets or creates a user based on the external login info
    public async Task<User> LinkExternalUserAsync(ExternalUserInfo info)
    {
        if (await ExternalLoginExistsAsync(info.Provider, info.ProviderId))
        {
            var matchingLogin = await db.ExternalLogins.Include(u => u.User!)
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
        if (await EmailExistsAsync(user)) return;
        await db.Users.AddAsync(user);
        await db.SaveChangesAsync();
    }

    public async Task<bool> EmailExistsAsync(User user)
    {
        return await db.Users.AnyAsync(u => u.Email == user.Email);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await db.Users.AnyAsync(u => u.Email == email);
    }

    public async Task<bool> ExternalLoginExistsAsync(ExternalLogin login)
    {
        return await db.ExternalLogins
            .AnyAsync(l => l.Provider == login.Provider && l.ProviderId == login.ProviderId);
    }

    public async Task<List<ExternalLogin>> GetExternalLoginsAsync(User user)
    {
        return await db.ExternalLogins
            .Where(l => l.UserId == user.Id)
            .ToListAsync();
    }

    public async Task<bool> ExternalLoginExistsAsync(string provider, string providerId)
    {
        return await db.ExternalLogins.Include(u => u.User)
            .AnyAsync(l => l.Provider == provider && l.ProviderId == providerId);
    }

    public void DeleteExternalLogin(ExternalLogin login)
    {
        db.ExternalLogins.Remove(login);
    }

    public void DeleteUser(User user)
    {
        db.Users.Remove(user);
    }

    public async Task<User?> GetUserAsync(string email)
    {
        return await db.Users.FirstOrDefaultAsync(u => u.Email == email);
    }
}