using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Teracura.TestingWebApp.Entities;
using Teracura.TestingWebApp.Entities.Users;
using Teracura.TestingWebApp.Logic.Data;

namespace Teracura.TestingWebApp.Logic;

public class ExternalLoginManager
{
    private readonly AppDbContext _db;

    public ExternalLoginManager(AppDbContext db)
    {
        _db = db;
    }

    public async Task<User> GetOrCreateUserAsync(ExternalUserInfo info, string accessToken)
    {
        // 1️⃣ Check if this exact provider + providerId already exists
        var externalLogin = await _db.ExternalLogins
            .Include(el => el.User)
            .FirstOrDefaultAsync(el => el.Provider == info.Provider && el.ProviderId == info.ProviderId);

        if (externalLogin != null)
            return externalLogin.User;

        var email = info.Email;

        if (string.IsNullOrEmpty(email))
        {
            throw new InvalidOperationException("Email is required to create a user.");
        }

        var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        User user;

        if (existingUser != null)
        {
            // Email exists -> link new external login to this existing user
            user = existingUser;
        }
        else
        {
            user = new User
            {
                Id = Guid.NewGuid(),
                Name = info.Name!,
                Email = email,
                PasswordHash = "" // external login only
            };
            _db.Users.Add(user);
        }

        var login = new ExternalLogin
        {
            Id = Guid.NewGuid(),
            User = user,
            UserId = user.Id,
            Provider = info.Provider,
            ProviderId = info.ProviderId
        };
        _db.ExternalLogins.Add(login);

        await _db.SaveChangesAsync();
        return user;
    }
}