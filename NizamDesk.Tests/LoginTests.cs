using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Shouldly;
using Teracura.TestingWebApp.Entities.Users;
using Teracura.TestingWebApp.Logic;
using Teracura.TestingWebApp.Logic.Cryptography;
using Teracura.TestingWebApp.Logic.Data;

namespace Teracura.TestingWebApp.Tests;

public class LoginTests
{
    private readonly UserManager _userManager = new(GetDbContext());
    private readonly PasswordManager _passwordManager = new();

    private static AppDbContext GetDbContext()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("testsettings.json",
                optional: false)
            .Build();

        string connectionString = config.GetConnectionString("DefaultConnection")!;

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task Password_Hashed_Saving_To_Database()
    {
        await using var context = GetDbContext();

        var hashSalt = _passwordManager.HashPassword("PlainTextPassword");
        var user = new User
        {
            Name = "testuser",
            PasswordHash = hashSalt.Hash,
            Salt = hashSalt.Salt,
            Email = "aaaa@test.com",
            Id = Guid.NewGuid()
        };

        user.PasswordHash.ToString().ShouldNotBe("PlainTextPassword");
        if (await context.Users.FirstOrDefaultAsync(u => u.Email == user.Email) == null)
        {
            context.Users.Add(user);
        }

        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task Password_Verified_When_Retrieved_From_Database()
    {
        using var context = GetDbContext();

        var savedUser = context.Users.First(u => u.Email == "aaaa@test.com");
        var isVerified = _passwordManager.VerifyPassword(savedUser.PasswordHash, savedUser.Salt, "PlainTextPassword");
        isVerified.ShouldBeTrue();
    }

    [Fact]
    public async Task Should_Sign_Up_User()
    {
        var testPasswordInput = "thisisATestPasswoRD1234567890";
        var hashSalt = _passwordManager.HashPassword(testPasswordInput);
        var testUser = new User
        {
            Name = "Teracura",
            Email = "teracura@email.com",
            PasswordHash = hashSalt.Hash,
            Salt = hashSalt.Salt,
            Id = Guid.NewGuid()
        };
        await _userManager.RegisterUserAsync(testUser);
        var user = await _userManager.GetUserAsync(testUser.Email);
        user.PasswordHash.ShouldBe(testUser.PasswordHash);
        user.Salt.ShouldBe(testUser.Salt);
    }
}