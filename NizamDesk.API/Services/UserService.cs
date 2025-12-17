using Microsoft.EntityFrameworkCore;
using NizamDesk.API.Data;
using NizamDesk.API.EndpointEntities;
using Teracura.TestingWebApp.Entities.Users;

namespace NizamDesk.API.Services;

public class UserService(IDbContextFactory<AppDbContext> dbContextFactory)
{
    public async Task<UserResponse?> GetUserAsync(Guid id)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);
        var user = await db.Users.FindAsync(id).ConfigureAwait(false);
        if (user is null) return null;
        return new UserResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email
        };
    }

    public async Task<List<UserResponse>> GetUsersAsync()
    {
        await using var db = await dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

        return await db.Users
            .Select(u => new UserResponse
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email
            })
            .ToListAsync()
            .ConfigureAwait(false);
    }


    public async Task<UserResponse?> AddUserAsync(UserCreateRequest user)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

        var currentUser = new User
        {
            Id = Guid.NewGuid(),
            Name = user.Name,
            Email = user.Email
        };

        if (!string.IsNullOrEmpty(user.Password))
        {
            var password = PasswordService.HashPassword(user.Password);
            currentUser.PasswordHash = password.Hash;
            currentUser.Salt = password.Salt;
        }

        db.Users.Add(currentUser);

        try
        {
            await db.SaveChangesAsync().ConfigureAwait(false);
        }
        catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UNIQUE") == true)
        {
            return null;
        }

        return new UserResponse
        {
            Id = currentUser.Id,
            Name = currentUser.Name,
            Email = currentUser.Email
        };
    }


    public async Task<bool> DeleteUserAsync(Guid id)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

        var user = await db.Users.FindAsync(id).ConfigureAwait(false);
        if (user == null)
            return false;

        db.Users.Remove(user);
        await db.SaveChangesAsync().ConfigureAwait(false);
        return true;
    }

    /// <summary>
    /// Updates a user's Name and Email in the database by their ID.
    /// </summary>
    /// <param name="id">The ID of the user to update.</param>
    /// <param name="newUser">The user object containing updated Name and Email.</param>
    /// <returns>
    /// True if the update was successful; false if the ID does not match newUser.Id 
    /// or if the user is not found in the database.
    /// </returns>
    public async Task<UserResponse?> UpdateUserAsync(Guid id, UserUpdateRequest newUser)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

        var user = await db.Users.FindAsync(id).ConfigureAwait(false);
        if (user is null) return null;

        user.Email = newUser.Email ?? user.Email;
        user.Name = newUser.Name ?? user.Name;
        if (newUser.Password is not null)
        {
            var password = PasswordService.HashPassword(newUser.Password);
            user.PasswordHash = password.Hash;
            user.Salt = password.Salt;
        }

        await db.SaveChangesAsync().ConfigureAwait(false);

        return new UserResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email
        };
    }
}