using Microsoft.EntityFrameworkCore;
using NizamDesk.API.Data;
using NizamDesk.API.EndpointEntities;
using NizamDesk.API.Services;
using Shouldly;

namespace NizamDesk.Tests.UnitTests;

public class UserTests : TestBase
{
    private readonly UserService _userService;
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    public UserTests()
    {
        _userService = GetService<UserService>();
        _dbContextFactory = GetService<IDbContextFactory<AppDbContext>>();
    }

    private static UserCreateRequest CreateTempUserRequest(string nameSuffix = "")
    {
        return new UserCreateRequest
        {
            Name = $"Test{nameSuffix}",
            Email = $"test{nameSuffix}@example.com",
            Password = "password123"
        };
    }

    [Fact]
    public async Task CreateUser_ShouldSucceed()
    {
        var request = CreateTempUserRequest("Create");
        
        await using var db = await _dbContextFactory.CreateDbContextAsync().ConfigureAwait(true);
        db.Users.RemoveRange(db.Users);
        await db.SaveChangesAsync().ConfigureAwait(true);
        
        var user = await _userService.AddUserAsync(request).ConfigureAwait(true);

        user.ShouldNotBeNull();
        user.Name.ShouldBe(request.Name);
        user.Email.ShouldBe(request.Email);

        var exists = await db.Users.AnyAsync(u => u.Id == user.Id);
        exists.ShouldBeTrue();
    }

    [Fact]
    public async Task GetUser_ShouldReturnExistingUser()
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync().ConfigureAwait(true);
        db.Users.RemoveRange(db.Users);
        await db.SaveChangesAsync().ConfigureAwait(true);
        
        var request = CreateTempUserRequest("Read");
        var created = await _userService.AddUserAsync(request).ConfigureAwait(true);
        created.ShouldNotBeNull();

        var fetched = await _userService.GetUserAsync(created.Id).ConfigureAwait(true);
        fetched.ShouldNotBeNull();
        fetched.Name.ShouldBe(request.Name);
        fetched.Email.ShouldBe(request.Email);
    }

    [Fact]
    public async Task UpdateUser_ShouldModifyFields()
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync().ConfigureAwait(true);
        db.Users.RemoveRange(db.Users);
        await db.SaveChangesAsync().ConfigureAwait(true);
        
        var request = CreateTempUserRequest("Update");
        var created = await _userService.AddUserAsync(request).ConfigureAwait(true);
        created.ShouldNotBeNull();

        var updateRequest = new UserUpdateRequest
        {
            Name = $"{request.Name}Updated",
            Email = $"updated_{request.Email}",
            Password = "newPassword!"
        };

        var updated = await _userService.UpdateUserAsync(created.Id, updateRequest).ConfigureAwait(true);
        updated.ShouldNotBeNull();
        updated.Name.ShouldBe(updateRequest.Name);
        updated.Email.ShouldBe(updateRequest.Email);

        var fetched = await _userService.GetUserAsync(created.Id).ConfigureAwait(true);
        fetched.ShouldNotBeNull();
        fetched.Name.ShouldBe(updateRequest.Name);
        fetched.Email.ShouldBe(updateRequest.Email);
    }

    [Fact]
    public async Task DeleteUser_ShouldRemoveFromDatabase()
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync().ConfigureAwait(true);
        db.Users.RemoveRange(db.Users);
        await db.SaveChangesAsync().ConfigureAwait(true);
        
        var request = CreateTempUserRequest("Delete");
        var created = await _userService.AddUserAsync(request).ConfigureAwait(true);
        created.ShouldNotBeNull();

        var success = await _userService.DeleteUserAsync(created.Id).ConfigureAwait(true);
        success.ShouldBeTrue();

        var fetched = await _userService.GetUserAsync(created.Id).ConfigureAwait(true);
        fetched.ShouldBeNull();
    }
}