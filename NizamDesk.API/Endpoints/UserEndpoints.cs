using System.Diagnostics;
using System.Globalization;
using NizamDesk.API.EndpointEntities;
using NizamDesk.API.Services;

namespace NizamDesk.API.EndPoints;

public abstract class UserEndpoints : IEndpointMapper
{
    public static Task Map(IEndpointRouteBuilder app)
    {
        //<summary>
        // adds new user to the database
        //</summary>
        app.MapPost("/api/users", async (UserService manager, UserCreateRequest userRequest) =>
        {
            try
            {
                var createdUser = await manager.AddUserAsync(userRequest).ConfigureAwait(false);

                return createdUser is null
                    ? Results.Conflict("Email already exists.")
                    : Results.Created($"/api/users/{createdUser.Id}", createdUser);
            }
            catch (Exception e)
            {
                Debug.WriteLine("err err err err err");
                Debug.WriteLine(e);
                return Results.BadRequest(e.Message);
            }

        });

        //<summary>
        // deletes user from the database via ID
        //</summary>
        app.MapDelete("/api/users/{userId:guid}", async (UserService manager, Guid userId) =>
        {
            var deleted = await manager.DeleteUserAsync(userId);
            return deleted ? Results.NoContent() : Results.NotFound();
        });

        //<summary>
        // returns user from the database via ID
        //</summary>
        app.MapGet("/api/users/{userId:guid}", async (UserService manager, Guid userId) =>
        {
            var user = await manager.GetUserAsync(userId);
            return user == null ? Results.NotFound() : Results.Ok(user);
        });

        //<summary>
        // returns all user IDs from the database
        //</summary>
        app.MapGet("/api/users", async (UserService manager) =>
        {
            var users = await manager.GetUsersAsync();
            return Results.Ok(users);
        });

        //<summary>
        // updates user in the database via ID
        //</summary>
        app.MapPut("/api/users/{userId:guid}", async (UserService manager, Guid userId, UserUpdateRequest user) =>
        {
            var resultUser = await manager.UpdateUserAsync(userId, user).ConfigureAwait(false);
            return resultUser is null ? Results.NotFound() : Results.Ok(resultUser);
        });
        return Task.CompletedTask;
    }
}