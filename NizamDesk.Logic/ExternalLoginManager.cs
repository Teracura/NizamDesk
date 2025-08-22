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
        // Try to find existing user
        var externalLogin = await _db.ExternalLogins
            .Include(el => el.User)
            .FirstOrDefaultAsync(el => el.Provider == info.Provider && el.ProviderId == info.ProviderId);

        if (externalLogin != null)
            return externalLogin.User;

        // If email is null, fetch from GitHub API
        var email = info.Email;
        if (string.IsNullOrEmpty(email) && info.Provider == "GitHub")
        {
            email = await GetPrimaryGitHubEmailAsync(accessToken);
        }

        // Create new user
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = info.Name!,
            Email = email!,
            PasswordHash = "" // external login
        };

        _db.Users.Add(user);

        // Add ExternalLogin record
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

    public async Task<string?> GetPrimaryGitHubEmailAsync(string accessToken)
    {
        using var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user/emails");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        request.Headers.UserAgent.ParseAdd("TeracuraApp"); // GitHub requires User-Agent

        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var emails = json.RootElement.EnumerateArray();

        foreach (var emailEntry in emails)
        {
            if (emailEntry.GetProperty("primary").GetBoolean() &&
                emailEntry.GetProperty("verified").GetBoolean())
            {
                return emailEntry.GetProperty("email").GetString();
            }
        }

        return null; // fallback if no primary verified email
    }
}