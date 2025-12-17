using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using NizamDesk.API.EndpointEntities;
using Shouldly;

namespace NizamDesk.Tests.IntegrationTests;

public class UserTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public UserTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    private UserCreateRequest CreateTempUserRequest(string suffix = "")
        => new()
        {
            Name = $"Test{suffix}",
            Email = $"test{suffix}@example.com",
            Password = "password123"
        };

    [Fact]
    public async Task CreateUser_ShouldSucceed()
    {
        var client = _factory.CreateClient();
        var request = CreateTempUserRequest("Create");

        var response = await client.PostAsJsonAsync("/api/users", request);
        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        var created = await response.Content.ReadFromJsonAsync<UserResponse>();
        created.ShouldNotBeNull();
        created.Name.ShouldBe(request.Name);
        created.Email.ShouldBe(request.Email);
    }

    [Fact]
    public async Task CreateUser_ShouldFail_Duplicate()
    {
        var client = _factory.CreateClient();
        var request = CreateTempUserRequest("Duplicate");

        var response1 = await client.PostAsJsonAsync("/api/users", request);
        response1.StatusCode.ShouldBe(HttpStatusCode.Created);

        var response2 = await client.PostAsJsonAsync("/api/users", request);
        response2.StatusCode.ShouldBe(HttpStatusCode.Conflict);

        var duplicate = await response2.Content.ReadFromJsonAsync<UserResponse>();
        duplicate.ShouldBeNull();
    }
}