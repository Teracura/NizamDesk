using NizamDesk.API.EndpointEntities.Companies;
using NizamDesk.API.Services;
using Teracura.TestingWebApp.Entities.Models;
using Teracura.TestingWebApp.Entities.Roles;

namespace NizamDesk.API.EndPoints;

public abstract class CompanyEndPoints : IEndpointMapper
{
    public static Task Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/companies", async (CompanyService manager, CompanyCreateRequest request) =>
        {
            var createdCompany = await manager.CreateCompanyAsync(request).ConfigureAwait(false);
            return createdCompany is null
                ? Results.BadRequest("Company already exists.")
                : Results.Created($"/api/companies/{createdCompany.Id}", createdCompany);
        });

        app.MapDelete("/api/companies/{companyId:guid}",
            async (CompanyService manager, Guid companyId) =>
                await (manager.DeleteCompanyAsync(companyId)).ConfigureAwait(false)
                    ? Results.NoContent()
                    : Results.NotFound());

        app.MapGet("/api/companies/{companyId:guid}", async (CompanyService manager, Guid companyId) =>
        {
            var response = await manager.GetCompanyAsync(companyId).ConfigureAwait(false);
            return response is null ? Results.NotFound() : Results.Ok(response);
        });

        app.MapGet("/api/companies",
            async (CompanyService manager) => await manager.GetCompaniesAsync().ConfigureAwait(false));

        app.MapPut("/api/companies/{companyId:guid}",
            async (CompanyService manager, Guid companyId, CompanyUpdateRequest request) =>
            {
                var companyResponse = await manager.UpdateCompanyAsync(companyId, request).ConfigureAwait(false);
                return companyResponse is null ? Results.NotFound() : Results.Ok(companyResponse);
            });

        /*
         * <summary>
         * joins a user to a company
         * </summary>
         */
        app.MapPost("/api/companies/{companyId:guid}/members",
            async (CompanyService manager, Guid companyId, JoinCompanyForm form) =>
            {
                var status = await manager.UserJoinCompanyAsync(companyId, form).ConfigureAwait(false);
                return status switch
                {
                    CompanyJoinStatus.Success => Results.Ok(),
                    CompanyJoinStatus.CompanyNotFound or CompanyJoinStatus.UserNotFound => Results.NotFound(),
                    CompanyJoinStatus.UserAlreadyMember => Results.Conflict(),
                    _ => throw new ArgumentOutOfRangeException()
                };
            });

        /*
         * <summary>
         * removes a user from a company
         * </summary>
         */
        app.MapDelete("/api/companies/{companyId:guid}/members/{userId:guid}",
            async (CompanyService manager, Guid companyId, Guid userId) =>
            {
                var status = await manager.UserLeaveCompanyAsync(companyId, userId).ConfigureAwait(false);

                return status ? Results.NoContent() : Results.NotFound();
            });

        /*
         * <summary>
         * grants a role to a user
         * </summary>
         */
        app.MapPost("/api/companies/{companyId:guid}/members/{userId:guid}/roles/{roleId:guid}",
            async (RoleService manager, Guid companyId, Guid userId, Guid roleId) =>
            {
                var userRole = await manager.GrantRoleUserAsync(companyId, userId, roleId).ConfigureAwait(false);
                return userRole is null
                    ? Results.NotFound()
                    : Results.Created($"/api/companies/{companyId}/members/{userId}/roles/{roleId}",
                        userRole);
            }
        );

        /*
         * <summary>
         * removes a role from a user
         * </summary?
         */
        app.MapDelete("/api/companies/{companyId:guid}/members/{userId:guid}/roles/{roleId:guid}",
            async (RoleService service, Guid companyId, Guid userId, Guid roleId) =>
            {
                var success = await service.RemoveRoleUserAsync(companyId, userId, roleId).ConfigureAwait(false);
                return success ? Results.NoContent() : Results.NotFound();
            });

        /*
         * <summary>
         * creates a new role for company
         * </summary>
         */
        app.MapPost("api/companies/{companyId:guid}/roles",
            async (RoleService service, Guid companyId, RoleModel role) =>
            {
                var user = await service.CreateRoleAsync(companyId, role).ConfigureAwait(false);
                return user is null
                    ? Results.NotFound()
                    : Results.Created($"/api/companies/{companyId}/roles/{user.Id}", user);
            });


        return Task.CompletedTask;
    }
}